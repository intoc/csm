using System.Reflection.Metadata.Ecma335;
using System.Xml;
using System.Xml.Serialization;

namespace csm.Models;
[Serializable()]
public class IntParam : Param {

    [XmlIgnore]
    public int IntValue { get; set; }

    [XmlAttribute]
    public override string? Value {
        get => IntValue.ToString();
        set {
            IntValue = int.Parse(value ?? "0");
        }
    }

    [XmlIgnore]
    public int MinVal { get; set; }

    [XmlIgnore]
    public int MaxVal { get; set; }

    public IntParam() : base() { }

    public IntParam(string arg, int val, string units = "Number") : base(arg, units) {
        IntValue = val;
        MinVal = int.MinValue;
        MaxVal = int.MaxValue;
    }

    public override void ParseVal(string? value) {
        int oldVal = IntValue;
        if (value == string.Empty) {
            IntValue = 0;
        } else {
            if (int.TryParse(value, out int outVal)) {
                IntValue = outVal;
            } else {
                throw new ArgumentException(string.Format("Value for {0} must be an Integer.", Desc));
            }
        }
        if (IntValue < MinVal || IntValue > MaxVal) {
            IntValue = oldVal;
            throw new ArgumentOutOfRangeException(string.Format("Value for {0} must be at least {1} and at most {2}.", Desc, MinVal, MaxVal));
        }
        if (IntValue != oldVal) {
            Changed();
        }
    }
}