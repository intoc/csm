using SixLabors.Fonts;
using SixLabors.ImageSharp;

namespace csm.Business.Models;

internal class ThumbnailData {

    public ThumbnailData(ImageData image, Image sheetImage) {
        Image = image;
        SheetImage = sheetImage;
    }
    public FontFamily FontFamily { get; set; }
    public ImageData Image { get; set; }
    public Image SheetImage { get; set; }
    public int Index { get; set; }
    public int ImageTotal { get; set; }
    public int FontSize { get; set; }
    public string File => Image.File;

}
