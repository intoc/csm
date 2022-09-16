using System.Xml;
using System.Xml.Serialization;

namespace csm.Models;

[Serializable]
public class BoolParam : Param {

    private static readonly string[] falses = { "no", "false", "0" };

    [XmlAttribute]
    public bool Val { get; set; }

    public BoolParam() : base() { }

    public BoolParam(string arg, bool val) : base(arg, "true/false") {
        Val = val;
    }

    public void AddSubParam(Param p) {
        SubParams.Add(p);
    }

    public override void ParseVal(string? value) {
        bool orig = Val;
        Val = !falses.Contains(value?.ToLower() ?? "false");
        if (Val != orig) {
            Changed();
        }
    }

    public override string Value() {
        return string.Format("{0}", Val);
    }

    protected override void Load(Param other) {
        if (other is BoolParam otherBool) {
            var orig = Val;
            Val = otherBool.Val;
            if (orig != Val) {
                Changed();
            }
            LoadSubs(other);
        }
    }
}