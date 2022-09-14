
using System;
using System.Xml;
using System.Xml.Serialization;

namespace csm.Models;     
[Serializable()]
public class StringParam : Param {

    [XmlAttribute]
    public string Val { get; set; }
    
    [XmlIgnore]
    public int MaxChars { get; set; }

    public StringParam() : base() { }

    public StringParam(string arg, string val, string units)
        : base(arg, units) {
        Val = val;
    }

    public override void ParseVal(string value) {
        bool same = value == Val;
        Val = value;
        if (!same) {
            Changed();
        }
    }

    public override string Value() {
        return Val;
    }

    protected override void Load(Param other) {
        if (other is StringParam otherString) {
            string orig = Val;
            Val = otherString.Val;
            if (Val != orig) {
                Changed();
            }
            LoadSubs(other);
        }
    }
}