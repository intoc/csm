using SixLabors.ImageSharp;

namespace csm.Business.Models;
internal class DrawThreadObj {

    public DrawThreadObj(ImageData image, Image sheetImage) {
        Image = image;
        SheetImage = sheetImage;
    }

    public ImageData Image { get; set; }
    public Image SheetImage { get; set; }
    public int Index { get; set; }
    public int ImageTotal { get; set; }
    public int FontSize { get; set; }
    public int BorderWidth { get; set; }
    public string File => Image.File;

}
