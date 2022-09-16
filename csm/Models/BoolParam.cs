using System.Xml;
using System.Xml.Serialization;

namespace csm.Models;

[Serializable]
public class BoolParam : Param {

    private static readonly string[] falses = { "no", "false", "0" };

    [XmlIgnore]
    public bool BoolValue { get; set; }

    [XmlAttribute]
    public override string? Value {
        get => BoolValue.ToString().ToLower();
        set {
            BoolValue = bool.Parse(value ?? "false");
        }
    }

    public BoolParam() : base() { }

    public BoolParam(string arg, bool val) : base(arg, "true/false") {
        BoolValue = val;
    }

    public void AddSubParam(Param p) {
        SubParams.Add(p);
    }

    public override void ParseVal(string? value) {
        bool orig = BoolValue;
        BoolValue = !falses.Contains(value?.ToLower() ?? "false");
        Changed(BoolValue != orig);
    }
}