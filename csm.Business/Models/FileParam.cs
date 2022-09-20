using System.Xml;
using System.Xml.Serialization;

namespace csm.Business.Models;

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