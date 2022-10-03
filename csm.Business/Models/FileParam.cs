using csm.Business.Logic;
using Serilog;
using System.Xml;
using System.Xml.Serialization;

namespace csm.Business.Models;

[Serializable]
public class FileParam : Param {
    private string? unParsedVal;

    [XmlIgnore]
    public string? FileName {
        get => File?.FileName ?? unParsedVal;
        set => Path = value;
    }

    [XmlAttribute]
    public override string? Value {
        get => FileName;
        set => FileName = value;
    }

    [XmlIgnore]
    public string? Path {
        get => File?.Path ?? unParsedVal;
        set => ParseVal(value);
    }

    [XmlAttribute]
    public string Ext { get; set; } = string.Empty;

    [XmlIgnore]
    public ImageFile? File { get; set; }

    private readonly IFileSource _fileSource;

    public FileParam() : base() {
        _fileSource = AbstractFileSource.Empty;
    }

    public FileParam(string arg, IFileSource fileSource, string? val = null) : base(arg, "file") {
        _fileSource = fileSource;
        if (val != null) {
            Path = val;
        }
    }

    public override void ParseVal(string? value) {
        unParsedVal = value;

        // Try a full-path parse
        if (_fileSource.FileExists(value)) {
            File = _fileSource.GetFile(value);
            Ext = File?.Extension ?? string.Empty;
        } else {
            // No file
            File = null;
        }
        Changed(unParsedVal != FileName);
    }

    protected override void Load(Param other) {
        if (other is FileParam otherFile) {
            if (LoadFromSettings) {
                ParseVal(otherFile.FileName);
            }
            LoadSubParams(other);
        }
    }
}