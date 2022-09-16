using System.Resources;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace csm.Models;
public delegate void ParamChangedEventHandler(Param source);

[Serializable]
[XmlInclude(typeof(BoolParam))]
[XmlInclude(typeof(IntParam))]
[XmlInclude(typeof(StringParam))]
[XmlInclude(typeof(FileParam))]
[XmlInclude(typeof(NullParam))]
public abstract class Param {

    /// <summary>
    /// Fired when relevant state changes
    /// </summary>
    public event ParamChangedEventHandler ParamChanged = delegate { };

    /// <summary>
    /// The command-line parameter string for this Parameter
    /// </summary>
    [XmlAttribute]
    public string CmdParameter { get; set; } = string.Empty;

    /// <summary>
    /// The units of <see cref="Value"/>
    /// </summary>
    [XmlIgnore]
    public string? Units { get; set; }

    /// <summary>
    /// Whether to exclude this parameter from the settings file loading process
    /// </summary>
    [XmlIgnore]
    public bool ExcludeFromLoading { get; set; }

    /// <summary>
    /// Sub-parameters of this parameter
    /// </summary>
    [XmlElement("Param")]
    public List<Param> SubParams { get; set; }

    /// <summary>
    /// The parameter value
    /// </summary>
    [XmlAttribute]
    public abstract string? Value { get; set; }

    private static ResourceSet? Resources => ParamsResources.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true);
    public string? Desc => Resources?.GetString($"{CmdParameter[1..]}_desc");
    public string? Note => Resources?.GetString($"{CmdParameter[1..]}_note");

    /// <summary>
    /// Default constructor
    /// </summary>
    protected Param() {
        SubParams = new List<Param>();
    }

    /// <summary>
    /// Creates a <see cref="Param"/>
    /// </summary>
    /// <param name="cmdParameter">The command-line parameter string</param>
    /// <param name="units">The units of <see cref="Value"/></param>
    protected Param(string cmdParameter, string? units = null) : this() {
        CmdParameter = cmdParameter;
        Units = units;
    }

    /// <summary>
    /// Parses the value from a command line parameter=value string
    /// </summary>
    /// <param name="cmdParamAndValue">The parameter=value string</param>
    /// <returns>The value</returns>
    public static string GetValueFromCmdParamAndValue(string cmdParamAndValue) {
        return cmdParamAndValue[(cmdParamAndValue.IndexOf('=') + 1)..];
    }

    /// <summary>
    /// Parses a string value into this parameter's properties
    /// </summary>
    /// <param name="value">The string</param>
    public abstract void ParseVal(string? value);

    /// <summary>
    /// Appends information about this parameter and <see cref="SubParams"/> to a <see cref="StringBuilder"/>
    /// </summary>
    /// <param name="help">The <see cref="StringBuilder"/></param>
    /// <param name="isMarkDown">Whether to format the appended string in MarkDown syntax</param>
    protected virtual void AppendHelpString(StringBuilder help, bool isMarkDown) {
        string? value = Value;
        value = string.IsNullOrEmpty(value) ? "[none]" : value;
        if (isMarkDown) {
            help.AppendLine($"| `{CmdParameter}` | {Desc} | {Units} | {value} | {Note} {(ExcludeFromLoading ? "(Not loaded from settings)" : string.Empty)} |");
        } else {
            string unitsDefaults = $"[{Units}, Default={value}{(ExcludeFromLoading ? " (Not loaded from settings)" : string.Empty)}]";
            help.AppendLine(CmdParameter == "null" ? string.Empty : $"{CmdParameter}: {Desc} {unitsDefaults}. {Note}");
        }
    }

    /// <summary>
    /// Load the values from another <see cref="Param"/> into this one
    /// </summary>
    /// <param name="other"></param>
    protected virtual void Load(Param other) {
        if (!ExcludeFromLoading) {
            ParseVal(other.Value);
        }
        LoadSubParams(other);
    }

    /// <summary>
    /// Load any matching sub-parameter values of another <see cref="Param"/>into this instance's <see cref="SubParams"/>
    /// </summary>
    /// <param name="other">The other <see cref="Param"/></param>
    protected void LoadSubParams(Param other) {
        foreach (var p in SubParams) {
            p.Load(other.SubParams);
        }
    }

    /// <summary>
    /// Fire the <see cref="ParamChanged"/> event
    /// </summary>
    /// <param name="changed">Will only fire if true</param>
    protected void Changed(bool changed = true) {
        if (changed) {
            ParamChanged?.Invoke(this);
        }
    }

    /// <summary>
    /// Load
    /// </summary>
    /// <param name="fromList"></param>
    public void Load(IEnumerable<Param> fromList) {
        var p = fromList.FirstOrDefault(prm => prm.CmdParameter == CmdParameter);
        if (p != null) {
            Load(p);
        }
    }

    /// <summary>
    /// Parses an param=value pair and sets it internally if matched. Will also check for a match on Sub Parameters and parse if matched.
    /// </summary>
    /// <param name="cmdParamAndValue"></param>
    /// <returns>True if the command line parameter matched and value was successfully parsed</returns>
    public bool Parse(string cmdParamAndValue) {
        if (cmdParamAndValue.StartsWith($"{CmdParameter}=")) {
            ParseVal(GetValueFromCmdParamAndValue(cmdParamAndValue));
            return true;
        }
        return SubParams.Any(p => p.Parse(cmdParamAndValue));
    }

    /// <summary>
    /// Prints help message for this parameter. Assumes no non-default assignment.
    /// </summary>
    /// <returns>The help message</returns>
    public string GetHelp(bool markDown) {
        StringBuilder help = new();
        AppendHelpString(help, markDown);
        foreach (Param p in SubParams) {
            help.Append($"{p.GetHelp(markDown)}");
        }
        return help.ToString();
    }

    public override string ToString() {
        var value = Value;
        if (string.IsNullOrEmpty(value)) {
            value = "[empty]";
        }
        return $"{Desc}: {value} ({Units})";
    }
}