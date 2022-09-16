using csm.Models;

namespace csm.Controls;
public partial class ParamsPanel : UserControl {


    public ParamsPanel() {
        InitializeComponent();
    }

    public void AddParamControl(Param p) {
        ParamControl pc = new(p);
        pramPanel.Controls.Add(pc);
    }

}
