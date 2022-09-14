namespace csm.Controls;

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
        this.outerPanel.Location = new System.Drawing.Point(4, 4);
        this.outerPanel.Margin = new System.Windows.Forms.Padding(0);
        this.outerPanel.Name = "outerPanel";
        this.outerPanel.Size = new System.Drawing.Size(272, 49);
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
        this.outerFlow.Size = new System.Drawing.Size(272, 26);
        this.outerFlow.TabIndex = 4;
        this.outerFlow.WrapContents = false;
        // 
        // checkBox
        // 
        this.checkBox.AutoSize = true;
        this.checkBox.Location = new System.Drawing.Point(3, 3);
        this.checkBox.Name = "checkBox";
        this.checkBox.Size = new System.Drawing.Size(15, 14);
        this.checkBox.TabIndex = 3;
        this.checkBox.UseVisualStyleBackColor = true;
        this.checkBox.Visible = false;
        this.checkBox.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
        // 
        // label
        // 
        this.label.AutoSize = true;
        this.label.Location = new System.Drawing.Point(22, 1);
        this.label.Margin = new System.Windows.Forms.Padding(1);
        this.label.Name = "label";
        this.label.Size = new System.Drawing.Size(29, 13);
        this.label.TabIndex = 0;
        this.label.Text = "label";
        this.label.TextAlign = System.Drawing.ContentAlignment.BottomRight;
        // 
        // text
        // 
        this.text.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
        this.text.Location = new System.Drawing.Point(55, 1);
        this.text.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
        this.text.Name = "text";
        this.text.Size = new System.Drawing.Size(100, 20);
        this.text.TabIndex = 2;
        this.text.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Text_KeyUp);
        this.text.Validating += new System.ComponentModel.CancelEventHandler(this.Text_Validating);
        // 
        // btnFileChooser
        // 
        this.btnFileChooser.Location = new System.Drawing.Point(161, 0);
        this.btnFileChooser.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
        this.btnFileChooser.Name = "btnFileChooser";
        this.btnFileChooser.Size = new System.Drawing.Size(75, 23);
        this.btnFileChooser.TabIndex = 4;
        this.btnFileChooser.Text = "Choose";
        this.btnFileChooser.UseVisualStyleBackColor = true;
        this.btnFileChooser.Visible = false;
        // 
        // units
        // 
        this.units.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.units.AutoSize = true;
        this.units.Location = new System.Drawing.Point(240, 1);
        this.units.Margin = new System.Windows.Forms.Padding(1, 1, 3, 1);
        this.units.Name = "units";
        this.units.Size = new System.Drawing.Size(29, 13);
        this.units.TabIndex = 1;
        this.units.Text = "units";
        this.units.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
        // 
        // subBox
        // 
        this.subBox.AutoSize = true;
        this.subBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.subBox.BackColor = System.Drawing.Color.Transparent;
        this.subBox.Controls.Add(this.subPanel);
        this.subBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.subBox.Location = new System.Drawing.Point(3, 29);
        this.subBox.Name = "subBox";
        this.subBox.Padding = new System.Windows.Forms.Padding(0);
        this.subBox.Size = new System.Drawing.Size(0, 17);
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
        this.subPanel.Location = new System.Drawing.Point(0, 13);
        this.subPanel.Margin = new System.Windows.Forms.Padding(0);
        this.subPanel.Name = "subPanel";
        this.subPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
        this.subPanel.Size = new System.Drawing.Size(0, 4);
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
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.AutoSize = true;
        this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.Controls.Add(this.outerPanel);
        this.Margin = new System.Windows.Forms.Padding(1);
        this.Name = "ParamControl";
        this.Size = new System.Drawing.Size(276, 53);
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
