using csm.Logic;
using Serilog;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace csm.Models;

[Serializable]
public class FileParam : Param {
    private string? unParsedVal;

    [XmlIgnore]
    public string? FileName {
        get {
            if (File != null) {
                return new FileInfo(File.Path).Name;
            } else {
                return unParsedVal;
            }
        }
        set {
            Path = value;
        }
    }

    [XmlAttribute]
    public override string? Value {
        get => FileName;
        set {
            FileName = value;
        }
    }

    [XmlIgnore]
    public string? Path {
        get {
            if (File != null) {
                return File.Path;
            } else {
                return unParsedVal;
            }
        }
        set {
            ParseVal(value);
        }
    }

    [XmlAttribute]
    public string Ext { get; set; } = string.Empty;

    [XmlIgnore]
    public ImageFile? File { get; set; }

    [XmlIgnore]
    public DirectoryInfo? Directory { get; set; }

    public FileParam() : base() { }

    public FileParam(string arg, string? val, DirectoryInfo? dir = null) : base(arg, "file") {
        Directory = dir;
        ParseVal(val);
    }

    public async Task<bool> Guess(IFileSource source, string[] patterns) {
        string? origFile = Path;
        bool changed = false;
        Log.Information("Guessing {0} using match patterns: {1}", Desc, string.Join(", ", patterns));
        var files = (await source.GetFilesAsync($"*{Ext}")).ToList();
        try {
            var regexes = patterns.Select(p => new Regex(p));
            ImageFile? match = regexes.Select(r =>
                files.FirstOrDefault(f => r.IsMatch(f.Path))).FirstOrDefault();
            if (match != null) {
                changed = origFile != match.Path;
                if (changed) {
                    File = match;
                    Log.Information("Matched {0} on {1}", Desc, File.Path);
                    Changed();
                } else {
                    Log.Information("Matched on the same cover file as before");
                }
                return changed;
            }
        } catch (Exception ex) {
            Log.Error(ex, "Error occurred during pattern matching");
        }
        if (files.Any()) {
            Log.Information("No match found for {0}, using first file in the directory.", Desc);
            File = files.First();
        }
        changed = origFile != Path;
        Changed(changed);
        return changed;
    }

    public override void ParseVal(string? value) {
        // Use the current directory if the path is not null,
        // the path is not (supposedly) in the current directory,
        // and the current directory exists
        unParsedVal = value;
        
        // Try a full-path parse
        if (System.IO.File.Exists(value)) {
            FileInfo f = new(value);
            File = new ImageFile(value);
            Directory = f.Directory;
        } else {
            // No file
            File = null;
        }
        Changed(unParsedVal != FileName);
    }

    protected override void Load(Param other) {
        if (other is FileParam otherFile) {
            if (!ExcludeFromLoading) {
                ParseVal(otherFile.FileName);
                Ext = otherFile.Ext ?? Ext;
            }
            LoadSubParams(other);
        }
    }
}