
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
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

    [XmlElement("Param")]
    public List<Param> SubParams { get; set; }

    private ResourceSet Resources => ParamsResources.ResourceManager.GetResourceSet(System.Threading.Thread.CurrentThread.CurrentCulture, true, true);
    public string Desc => Resources.GetString($"{Arg.Substring(1)}_desc");
    public string Note => Resources.GetString($"{Arg.Substring(1)}_note");

    [XmlIgnore]
    public bool PreventEvents { get; set; }

    public Param() {
        SubParams = new List<Param>();
    }

    public Param(string arg, string units = null) : this() {
        Arg = arg;
        Units = units;
    }

    protected abstract void Load(Param other);
    public abstract void ParseVal(string value);
    public abstract string Value();

    public void Load(IEnumerable<Param> fromList) {
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
            ParseVal(argAndValue.Substring(argAndValue.IndexOf('=') + 1));
            return true;
        }
        foreach (Param p in SubParams) {
            if (p.Parse(argAndValue)) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Prints help message for this parameter. Assumes no non-default assignment.
    /// </summary>
    /// <returns>The help message</returns>
    public string GetHelp(bool markDown) {
        string arg = markDown ? $"`{Arg}`" : $"{Arg}:";
        string unitsDefaults = $"[{Units}, Default={Value() ?? "[empty]"}]";
        var desc = Desc;
        string newLine = string.Empty;
        if (markDown) {
            unitsDefaults = $"*{unitsDefaults}*";
            desc = $"**{Desc}**";
            newLine = @"\";
        }
        string help = Arg == "null" ? string.Empty : $"{arg} {desc} {unitsDefaults}. {Note}{newLine}\n";
        foreach (Param p in SubParams) {
            help += $"{p.GetHelp(markDown)}";
        }
        return help;
    }

    public override string ToString() {
        var value = Value() ?? "[none]";
        if (value.Equals(string.Empty)) {
            value = "[empty]";
        }
        return $"{Desc}: {value} ({Units})";
    }
}