using csm.Models;
using System.Windows.Forms;

namespace csm.Controls {
    public partial class ParamsPanel : UserControl {


        public ParamsPanel() {
            InitializeComponent();
        }

        public void AddParamControl(Param p) {
            ParamControl pc = new ParamControl(p);
            pramPanel.Controls.Add(pc);
        }

    }
}
