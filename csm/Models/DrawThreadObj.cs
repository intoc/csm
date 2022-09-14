using System.Drawing;

namespace csm.Models; 
internal class DrawThreadObj {
    public ImageData Image { get; set; }
    public int Index { get; set; }
    public int ImageTotal { get; set; }
    public string File { get; set; }
    public int FontSize { get; set; }
    public int BorderWidth { get; set; }
    public Graphics G { get; set; }
}
