
using System;
using System.Xml;
using System.Xml.Serialization;

namespace csm.Models; 
[Serializable()]
public class IntParam : Param {

    [XmlAttribute]
    public int Val { get; set; }

    [XmlIgnore]
    public int MinVal { get; set; }

    [XmlIgnore]
    public int MaxVal { get; set; }

    public IntParam() : base() { }

    public IntParam(string arg, int val, string units = "Number") : base(arg, units) {
        Val = val;
        MinVal = int.MinValue;
        MaxVal = int.MaxValue;
    }

    public override void ParseVal(string value) {
        int oldVal = Val;
        if (value == string.Empty) {
            Val = 0;
        } else {
            if (int.TryParse(value, out int outVal)) {
                Val = outVal;
            } else {
                throw new ArgumentException(string.Format("Value for {0} must be an Integer.", Desc));
            }
        }
        if (Val < MinVal || Val > MaxVal) {
            Val = oldVal;
            throw new ArgumentOutOfRangeException(string.Format("Value for {0} must be at least {1} and at most {2}.", Desc, MinVal, MaxVal));
        }
        if (Val != oldVal) {
            Changed();
        }
    }

    public override string Value() {
        return string.Format("{0}", Val);
    }

    protected override void Load(Param other) {
        if (other is IntParam otherInt) {
            Val = otherInt.Val;
            Changed();
            LoadSubs(other);
        }
    }

}