namespace csm.Controls;

partial class ParamsPanel
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
        this.mainPanel = new System.Windows.Forms.FlowLayoutPanel();
        this.pramPanel = new System.Windows.Forms.FlowLayoutPanel();
        this.mainPanel.SuspendLayout();
        this.SuspendLayout();
        // 
        // mainPanel
        // 
        this.mainPanel.AutoSize = true;
        this.mainPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.mainPanel.Controls.Add(this.pramPanel);
        this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this.mainPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
        this.mainPanel.Location = new System.Drawing.Point(0, 0);
        this.mainPanel.Name = "mainPanel";
        this.mainPanel.Size = new System.Drawing.Size(606, 26);
        this.mainPanel.TabIndex = 11;
        // 
        // pramPanel
        // 
        this.pramPanel.AutoSize = true;
        this.pramPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
        this.pramPanel.Location = new System.Drawing.Point(3, 3);
        this.pramPanel.MinimumSize = new System.Drawing.Size(600, 20);
        this.pramPanel.Name = "pramPanel";
        this.pramPanel.Size = new System.Drawing.Size(600, 20);
        this.pramPanel.TabIndex = 9;
        // 
        // ParamsPanel
        // 
        this.AutoSize = true;
        this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.Controls.Add(this.mainPanel);
        this.Name = "ParamsPanel";
        this.Size = new System.Drawing.Size(606, 26);
        this.mainPanel.ResumeLayout(false);
        this.mainPanel.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.FlowLayoutPanel mainPanel;
    private System.Windows.Forms.FlowLayoutPanel pramPanel;

}
