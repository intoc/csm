using System.Xml;
using System.Xml.Serialization;

namespace csm.Models;

[Serializable]
public class NullParam : Param {

    [XmlAttribute]
    public string Text { get; set; } = "Group";

    public NullParam() : base("null") { }

    public NullParam(string text) : base("null") {
        Text = text;
    }

    public void AddSubParam(Param p) {
        SubParams.Add(p);
    }

    public override void ParseVal(string? value) {}

    public override string Value() {
        return string.Format("{0}", Text);
    }

    protected override void Load(Param other) {
        if (other is NullParam) {
            LoadSubs(other);
        }
    }
}