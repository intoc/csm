using System.Xml;
using System.Xml.Serialization;

namespace csm.Models;
[Serializable()]
public class StringParam : Param {

    [XmlIgnore]
    public string? ParsedValue { get; set; }

    [XmlAttribute]
    public override string? Value {
        get => ParsedValue;
        set {
            ParsedValue = value;
        }
    }

    [XmlIgnore]
    public int MaxChars { get; set; }

    public StringParam() : base() { }

    public StringParam(string arg, string val, string units)
        : base(arg, units) {
        ParsedValue = val;
    }

    public override void ParseVal(string? value) {
        bool same = value == ParsedValue;
        ParsedValue = value;
        if (!same) {
            Changed();
        }
    }

}