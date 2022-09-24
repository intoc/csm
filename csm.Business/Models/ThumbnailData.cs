using SixLabors.Fonts;
using SixLabors.ImageSharp;

namespace csm.Business.Models;

internal class ThumbnailData {

    public ThumbnailData(ImageData image, Image sheetImage, Font labelFont, TextOptions labelTextOptions) {
        Image = image;
        SheetImage = sheetImage;
        LabelTextOptions = labelTextOptions;
        LabelFont = labelFont;
    }
    public Font LabelFont { get; set; }
    public TextOptions LabelTextOptions { get; set; }
    public ImageData Image { get; set; }
    public Image SheetImage { get; set; }
    public int Index { get; set; }
    public int ImageTotal { get; set; }
    public string File => Image.File;

}
