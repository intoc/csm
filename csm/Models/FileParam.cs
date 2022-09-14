
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace csm.Models; 
[Serializable]
public class FileParam : Param {
    private string unParsedVal;

    [XmlAttribute]
    public string Val {
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

    [XmlIgnore]
    public string Path {
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

    private string ext;
    [XmlAttribute]
    public string Ext {
        get => ext ?? string.Empty;
        set => ext = value;
    }

    [XmlIgnore]
    public FileInfo File { get; set; }

    [XmlIgnore]
    public DirectoryInfo Directory { get; set; }

    public FileParam() : base() { }

    public FileParam(string arg, string val, DirectoryInfo dir = null) : base(arg, "file") {
        Directory = dir;
        ParseVal(val);
    }

    public bool Guess(string[] patterns) {
        string origFile = Path;
        bool changed = false;
        if (Directory == null) {
            return false;
        }
        Console.WriteLine("Guessing {0} using match patterns: {1}", Desc, string.Join(", ", patterns));
        FileInfo[] files = Directory.GetFiles($"*{Ext}");
        try {
            var regexes = patterns.Select(p => new Regex(p));
            FileInfo match = regexes.Select(r =>
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
        if (changed) {
            Changed();
        }
        return changed;
    }

    public override void ParseVal(string path) {
        // Use the current directory if the path is not null,
        // the path is not (supposedly) in the current directory,
        // and the current directory exists
        unParsedVal = path;

        if (path != null && !path.Contains(@"\") && Directory != null) {
            File = null;

            FileInfo[] files = Directory.GetFiles($"*{Ext}");
            //Console.WriteLine("Parsing on {0}, dir={1}", path, Directory);
            File = files.FirstOrDefault(f => path.Length > 0 &&
                    (f.ToString() == path ||
                     f.ToString().ToLower().Contains(path.ToLower())));
        } else {
            // Try a full-path parse
            if (System.IO.File.Exists(path)) {
                File = new FileInfo(path);
                Directory = File.Directory;
            } else {
                // No file
                File = null;
            }
        }
        Changed();
    }

    public override string Value() {
        return Val;
    }

    protected override void Load(Param other) {
        if (other is FileParam otherFile) {
            Val = otherFile.Val ?? Val;
            Path = otherFile.Path ?? Path;
            Ext = otherFile.Ext ?? Ext;
            Changed();
            LoadSubs(other);
        }
    }
}