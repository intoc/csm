
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace csm.Models; 
public delegate void ParamChangedEventHandler(Param source);

[Serializable()]
[XmlInclude(typeof(BoolParam))]
[XmlInclude(typeof(IntParam))]
[XmlInclude(typeof(StringParam))]
[XmlInclude(typeof(FileParam))]
[XmlInclude(typeof(NullParam))]
public abstract class Param {

    public event ParamChangedEventHandler ParamChanged;

    [XmlAttribute]
    public string Arg { get; set; }

    [XmlIgnore]
    public string Units { get; set; }

    [XmlIgnore]
    public bool ExcludeFromLoading { get; set; }

    [XmlElement("Param")]
    public List<Param> SubParams { get; set; }

    private static ResourceSet Resources => ParamsResources.ResourceManager.GetResourceSet(System.Threading.Thread.CurrentThread.CurrentCulture, true, true);
    public string Desc => Resources.GetString($"{Arg[1..]}_desc");
    public string Note => Resources.GetString($"{Arg[1..]}_note");

    [XmlIgnore]
    public bool PreventEvents { get; set; }

    protected Param() {
        SubParams = new List<Param>();
    }

    protected Param(string arg, string units = null) : this() {
        Arg = arg;
        Units = units;
    }

    protected abstract void Load(Param other);
    public abstract void ParseVal(string value);
    public abstract string Value();

    public void Load(IEnumerable<Param> fromList) {
        if (ExcludeFromLoading) {
            return;
        }
        var p = fromList.FirstOrDefault(prm => prm.Arg == Arg);
        if (p != null) {
            Load(p);
        }
    }

    protected void LoadSubs(Param other) {
        foreach (var p in SubParams) {
            p.Load(other.SubParams);
        }
    }

    protected void Changed() {
        if (!PreventEvents) {
            ParamChanged?.Invoke(this);
        }
    }

    /// <summary>
    /// Parses an arg-value pair and sets it internally if matched. Will also check for a match on Sub Parameters and parse if matched.
    /// </summary>
    /// <param name="argAndValue"></param>
    /// <returns>True if the command matched</returns>
    public bool Parse(string argAndValue) {
        if (argAndValue.StartsWith($"{Arg}=")) {
            ParseVal(argAndValue[(argAndValue.IndexOf('=') + 1)..]);
            return true;
        }
        return SubParams.Any(p => p.Parse(argAndValue));
    }

    /// <summary>
    /// Prints help message for this parameter. Assumes no non-default assignment.
    /// </summary>
    /// <returns>The help message</returns>
    public string GetHelp(bool markDown) {
        string arg = markDown ? $"`{Arg}`" : $"{Arg}:";
        string unitsDefaults = $"[{Units}, Default={Value() ?? "[empty]"}, {(ExcludeFromLoading ? "Not loaded from settings" : string.Empty)}]";
        var desc = Desc;
        string newLine = string.Empty;
        if (markDown) {
            unitsDefaults = $"*{unitsDefaults}*";
            desc = $"**{Desc}**";
            newLine = @"\";
        }
        StringBuilder help = new();
        help.AppendLine(Arg == "null" ? string.Empty : $"{arg} {desc} {unitsDefaults}. {Note}{newLine}");
        foreach (Param p in SubParams) {
            help.Append($"{p.GetHelp(markDown)}");
        }
        return help.ToString();
    }

    public override string ToString() {
        var value = Value();
        if (string.IsNullOrEmpty(value)) {
            value = "[empty]";
        }
        return $"{Desc}: {value} ({Units})";
    }
}