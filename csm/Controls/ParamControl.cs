using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using csm.Models;

namespace csm.Controls; 
public partial class ParamControl : UserControl {
    private Param parameter;

    public ParamControl() {
        InitializeComponent();
        parameter = new NullParam();
    }
    public ParamControl(Param p) : this() {

        parameter = p;
        label.Text = p.Desc;
        text.Text = p.Value();
        units.Text = p.Units;

        if (parameter.Note != null) {
            tltpHelp.SetToolTip(label, parameter.Note);
        }

        if (parameter is BoolParam boolParam) {
            SetupBool(boolParam);
        } else if (parameter is IntParam intParam) {
            SetupInt(intParam);
        } else if (parameter is StringParam strParam) {
            SetupString(strParam);
        } else if (parameter is FileParam) {
            SetupFile();
        } else if (parameter is NullParam nullParam){
            // Just a group
            outerFlow.Visible = false;
            subBox.Visible = true;
            subBox.Text = nullParam.Text;
            foreach (Param sub in nullParam.SubParams) {
                subPanel.Controls.Add(new ParamControl(sub));
            }
        }
        p.ParamChanged += new ParamChangedEventHandler((param) => {
            Debug.WriteLine("ParamControl Update {0}: {1}", param.Arg, param.Value());
            RefreshValue(param);
        });
    }

    private void SetupBool(BoolParam boolParam) {
        text.Visible = false;
        units.Visible = false;
        checkBox.Visible = true;
        checkBox.Checked = boolParam.Val;
        if (boolParam.SubParams.Any()) {
            subBox.Visible = true;
            foreach (Param sub in boolParam.SubParams) {
                subPanel.Controls.Add(new ParamControl(sub));
            }
            if (!checkBox.Checked) {
                EnableSubParams(false);
            }
        }
    }

    private void SetupInt(IntParam intParam) {
        text.Width = 10 * ((int)Math.Log10(intParam.MaxVal) + 1);
    }

    private void SetupString(StringParam strParam) {
        if (strParam.MaxChars <= 0) {
            return;
        }
        int valWidth = (int)text.CreateGraphics().MeasureString(parameter.Value(), text.Font).Width;
        int charWidth = (int)text.CreateGraphics().MeasureString("M", text.Font).Width;
        int maxWidth = charWidth * strParam.MaxChars;
        text.Width = Math.Max(valWidth, maxWidth);
    }

    private void SetupFile() {
        btnFileChooser.Visible = true;
        btnFileChooser.Click += new EventHandler(BtnFileChooser_Click);
        text.Width = 250;
    }

    void BtnFileChooser_Click(object? sender, EventArgs e) {
        OpenFileDialog ofd = new() {
            InitialDirectory = ((FileParam)parameter).Directory?.ToString()
        };

        if (ofd.ShowDialog() == DialogResult.OK) {
            parameter.ParseVal(ofd.FileName);
            text.Text = parameter.Value();
        }
    }

    private void EnableSubParams(bool enabled) {
        subPanel.Enabled = enabled;
    }

    public void RefreshValue(Param p) {
        if (p is BoolParam param) {
            checkBox.Checked = param.Val;
        } else {
            text.Text = p.Value();
        }
    }

    public void Reset(List<Param> prams) {
        foreach (Param p in prams) {
            if (p.Arg == parameter.Arg) {
                parameter = p;
                RefreshValue(parameter);
                foreach (ParamControl sub in subPanel.Controls) {
                    sub.Reset(p.SubParams);
                }
            }
        }
    }

    private void CheckBox_CheckedChanged(object sender, EventArgs e) {
        if (parameter is BoolParam boolParam) {
            boolParam.ParseVal(checkBox.Checked.ToString());
        }
        EnableSubParams(checkBox.Checked);
    }

    private void CheckValueChanged() {
        try {
            parameter.ParseVal(text.Text);
        } catch (Exception ex) {
            MessageBox.Show(ex.Message, "Validation Error");
            RefreshValue(parameter);
        }
    }

    private void Text_Validating(object sender, CancelEventArgs e) {
        CheckValueChanged();
    }

    private void Text_KeyUp(object sender, KeyEventArgs e) {
        CheckValueChanged();
        e.Handled = true;
    }

}
