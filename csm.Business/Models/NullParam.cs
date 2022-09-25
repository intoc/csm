using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace csm.Business.Models;

[Serializable]
public class NullParam : Param {

    [XmlIgnore]
    public string Text { get; set; } = "Group";

    [XmlAttribute]
    public override string? Value {
        get => Text;
        set {
            Text = value ?? Text;
        }
    }

    public NullParam() : base("null", "none") { }

    public NullParam(string text) : base("null", "none") {
        Text = text;
    }

    public void AddSubParam(Param p) {
        SubParams.Add(p);
    }

    public override void ParseVal(string? value) { }

    protected override void AppendHelpString(StringBuilder help, bool isMarkDown) {
        // Don't append anything
    }
}