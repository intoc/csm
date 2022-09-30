using SixLabors.Fonts;
using SixLabors.ImageSharp;

namespace csm.Business.Models;

internal class ThumbnailData {

    public ThumbnailData(ImageData image, Image sheetImage, Font labelFont) {
        Image = image;
        SheetImage = sheetImage;
        LabelFont = labelFont;
    }
    public Font LabelFont { get; set; }
    public ImageData Image { get; set; }
    public Image SheetImage { get; set; }
    public int Index { get; set; }
    public int ImageTotal { get; set; }
    public string File => Image.File;

    public DateTime StartTime { get; set; }

}
