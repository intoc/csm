using System.Xml;
using System.Xml.Serialization;

namespace csm.Business.Models;
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
        if (value == "-") {
            if (MinVal >= 0) {
                throw new ArgumentOutOfRangeException($"Value for {Desc} must be a positive Integer.");
            } else {
                IntValue = 0;
            }
        } else if (value == string.Empty) {
            IntValue = 0;
        } else {
            if (int.TryParse(value, out int outVal)) {
                IntValue = outVal;
            } else {
                throw new ArgumentException($"Value for {Desc} must be an Integer.");
            }
        }
        if (IntValue < MinVal || IntValue > MaxVal) {
            IntValue = oldVal;
            throw new ArgumentOutOfRangeException($"Value for {Desc} must be at least {MinVal} and at most {MaxVal}.");
        }
        Changed(IntValue != oldVal);
    }
}