namespace csm.Models;
internal class DrawThreadObj {

    public DrawThreadObj(ImageData image, Graphics g) {
        Image = image;
        Graphics = g;
    }

    public ImageData Image { get; set; }
    public Graphics Graphics { get; set; }
    public int Index { get; set; }
    public int ImageTotal { get; set; }
    public int FontSize { get; set; }
    public int BorderWidth { get; set; }
    public string File => Image.File;

}
