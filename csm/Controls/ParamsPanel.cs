using csm.Business.Models;

namespace csm.WinForms.Controls;
public partial class ParamsPanel : UserControl {


    public ParamsPanel() {
        InitializeComponent();
    }

    public void AddParamControl(Param p) {
        ParamControl pc = new(p);
        pramPanel.Controls.Add(pc);
    }

}
