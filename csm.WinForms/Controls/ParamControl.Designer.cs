namespace csm.WinForms.Controls;

partial class ParamControl
{
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            this.outerPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.outerFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBox = new System.Windows.Forms.CheckBox();
            this.label = new System.Windows.Forms.Label();
            this.text = new System.Windows.Forms.TextBox();
            this.btnFileChooser = new System.Windows.Forms.Button();
            this.units = new System.Windows.Forms.Label();
            this.subBox = new System.Windows.Forms.GroupBox();
            this.subPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.tltpHelp = new System.Windows.Forms.ToolTip(this.components);
            this.outerPanel.SuspendLayout();
            this.outerFlow.SuspendLayout();
            this.subBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // outerPanel
            // 
            this.outerPanel.AutoSize = true;
            this.outerPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.outerPanel.BackColor = System.Drawing.Color.Transparent;
            this.outerPanel.Controls.Add(this.outerFlow);
            this.outerPanel.Controls.Add(this.subBox);
            this.outerPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.outerPanel.Location = new System.Drawing.Point(5, 5);
            this.outerPanel.Margin = new System.Windows.Forms.Padding(0);
            this.outerPanel.Name = "outerPanel";
            this.outerPanel.Size = new System.Drawing.Size(315, 57);
            this.outerPanel.TabIndex = 0;
            // 
            // outerFlow
            // 
            this.outerFlow.AutoSize = true;
            this.outerFlow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.outerFlow.Controls.Add(this.checkBox);
            this.outerFlow.Controls.Add(this.label);
            this.outerFlow.Controls.Add(this.text);
            this.outerFlow.Controls.Add(this.btnFileChooser);
            this.outerFlow.Controls.Add(this.units);
            this.outerFlow.Location = new System.Drawing.Point(0, 0);
            this.outerFlow.Margin = new System.Windows.Forms.Padding(0);
            this.outerFlow.Name = "outerFlow";
            this.outerFlow.Size = new System.Drawing.Size(315, 30);
            this.outerFlow.TabIndex = 4;
            this.outerFlow.WrapContents = false;
            // 
            // checkBox
            // 
            this.checkBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.checkBox.AutoSize = true;
            this.checkBox.Location = new System.Drawing.Point(4, 8);
            this.checkBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBox.Name = "checkBox";
            this.checkBox.Size = new System.Drawing.Size(15, 14);
            this.checkBox.TabIndex = 3;
            this.checkBox.UseVisualStyleBackColor = true;
            this.checkBox.Visible = false;
            this.checkBox.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // label
            // 
            this.label.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(24, 7);
            this.label.Margin = new System.Windows.Forms.Padding(1);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(32, 15);
            this.label.TabIndex = 0;
            this.label.Text = "label";
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // text
            // 
            this.text.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.text.Location = new System.Drawing.Point(61, 2);
            this.text.Margin = new System.Windows.Forms.Padding(4, 0, 4, 3);
            this.text.Name = "text";
            this.text.Size = new System.Drawing.Size(116, 23);
            this.text.TabIndex = 2;
            this.text.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Text_KeyUp);
            this.text.Validating += new System.ComponentModel.CancelEventHandler(this.Text_Validating);
            // 
            // btnFileChooser
            // 
            this.btnFileChooser.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnFileChooser.Location = new System.Drawing.Point(185, 0);
            this.btnFileChooser.Margin = new System.Windows.Forms.Padding(4, 0, 4, 3);
            this.btnFileChooser.Name = "btnFileChooser";
            this.btnFileChooser.Size = new System.Drawing.Size(88, 27);
            this.btnFileChooser.TabIndex = 4;
            this.btnFileChooser.Text = "Choose";
            this.btnFileChooser.UseVisualStyleBackColor = true;
            this.btnFileChooser.Visible = false;
            // 
            // units
            // 
            this.units.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.units.AutoSize = true;
            this.units.Location = new System.Drawing.Point(278, 7);
            this.units.Margin = new System.Windows.Forms.Padding(1, 1, 4, 1);
            this.units.Name = "units";
            this.units.Size = new System.Drawing.Size(33, 15);
            this.units.TabIndex = 1;
            this.units.Text = "units";
            this.units.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // subBox
            // 
            this.subBox.AutoSize = true;
            this.subBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.subBox.BackColor = System.Drawing.Color.Transparent;
            this.subBox.Controls.Add(this.subPanel);
            this.subBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.subBox.Location = new System.Drawing.Point(4, 33);
            this.subBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.subBox.Name = "subBox";
            this.subBox.Padding = new System.Windows.Forms.Padding(0);
            this.subBox.Size = new System.Drawing.Size(0, 21);
            this.subBox.TabIndex = 5;
            this.subBox.TabStop = false;
            this.subBox.Text = "Sub-Settings";
            this.subBox.Visible = false;
            // 
            // subPanel
            // 
            this.subPanel.AutoSize = true;
            this.subPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.subPanel.BackColor = System.Drawing.Color.Transparent;
            this.subPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.subPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.subPanel.Location = new System.Drawing.Point(0, 16);
            this.subPanel.Margin = new System.Windows.Forms.Padding(0);
            this.subPanel.Name = "subPanel";
            this.subPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.subPanel.Size = new System.Drawing.Size(0, 5);
            this.subPanel.TabIndex = 5;
            // 
            // tltpHelp
            // 
            this.tltpHelp.AutoPopDelay = 5000;
            this.tltpHelp.InitialDelay = 100;
            this.tltpHelp.IsBalloon = true;
            this.tltpHelp.ReshowDelay = 100;
            // 
            // ParamControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.outerPanel);
            this.Margin = new System.Windows.Forms.Padding(1);
            this.Name = "ParamControl";
            this.Size = new System.Drawing.Size(320, 62);
            this.outerPanel.ResumeLayout(false);
            this.outerPanel.PerformLayout();
            this.outerFlow.ResumeLayout(false);
            this.outerFlow.PerformLayout();
            this.subBox.ResumeLayout(false);
            this.subBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.FlowLayoutPanel outerPanel;
    private System.Windows.Forms.FlowLayoutPanel outerFlow;
    private System.Windows.Forms.CheckBox checkBox;
    private System.Windows.Forms.Label label;
    private System.Windows.Forms.TextBox text;
    private System.Windows.Forms.Button btnFileChooser;
    private System.Windows.Forms.Label units;
    private System.Windows.Forms.GroupBox subBox;
    private System.Windows.Forms.FlowLayoutPanel subPanel;
    private System.Windows.Forms.ToolTip tltpHelp;



}
