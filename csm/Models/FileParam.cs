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
                return File.Name;
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
                return File.FullName;
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
    public FileInfo? File { get; set; }

    [XmlIgnore]
    public DirectoryInfo? Directory { get; set; }

    public FileParam() : base() { }

    public FileParam(string arg, string? val, DirectoryInfo? dir = null) : base(arg, "file") {
        Directory = dir;
        ParseVal(val);
    }

    public bool Guess(string[] patterns) {
        string? origFile = Path;
        bool changed = false;
        if (Directory == null) {
            return false;
        }
        Console.WriteLine("Guessing {0} using match patterns: {1}", Desc, string.Join(", ", patterns));
        FileInfo[] files = Directory.GetFiles($"*{Ext}");
        try {
            var regexes = patterns.Select(p => new Regex(p));
            FileInfo? match = regexes.Select(r =>
                files.FirstOrDefault(f => r.IsMatch(f.ToString())))
                    .FirstOrDefault();
            if (match != null) {
                File = match;
                changed = origFile != Path;
                if (changed) {
                    Console.WriteLine("Matched {0} on {1}", Desc, File.FullName);
                    Changed();
                }
                return changed;
            }
        } catch (Exception ex) {
            Console.Error.WriteLine("Error occurred during pattern matching: {0}", ex.Message);
        }
        if (files.Length > 0) {
            Console.WriteLine("No match found for {0}, using first file in the directory.", Desc);
            File = files[0];
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
        if (!string.IsNullOrEmpty(value) && !value.Contains('\\') && Directory != null) {
            var files = Directory.GetFiles($"*{Ext}");
            File = files.FirstOrDefault(f =>
                   f.ToString() == value || f.ToString().ToLower().Contains(value.ToLower()));
        } else {
            // Try a full-path parse
            if (System.IO.File.Exists(value)) {
                File = new FileInfo(value);
                Directory = File.Directory;
            } else {
                // No file
                File = null;
            }
        }
        Changed(unParsedVal != FileName);
        
    }

    protected override void Load(Param other) {
        if (other is FileParam otherFile) {
            ParseVal(otherFile.FileName);
            Ext = otherFile.Ext ?? Ext;
            LoadSubParams(other);
        }
    }
}