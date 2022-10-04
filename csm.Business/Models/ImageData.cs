using SixLabors.ImageSharp;

namespace csm.Business.Models;

public class ImageData {
    public int Width { get; set; }
    public int Height { get; set; }
    public double Ratio { get; private set; }
    public bool IsLandscape { get; private set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Right => X + Width;
    public int Bottom => Y + Height;
    public string File { get; set; }
    public Size OriginalSize { get; private set; }
    public bool Include { get; set; } = true;
    public bool InclusionPinned { get; set; } = false;

    public Rectangle Bounds => new(X, Y, Width, Height);

    public Point Origin => new(X, Y);

    public string Orientation => IsLandscape ? "Landscape" : "Portrait";

    public string FileName => Path.GetFileName(File);
    public string FileNameNoExtension => Path.GetFileNameWithoutExtension(File);

    public string OriginalSizeString => $"{OriginalSize.Width}x{OriginalSize.Height}";

    // Used by the File List
    public string SizeString => $"{OriginalSizeString} {(OriginalSize.Width != Width ? $"[{Width}x{Height}]" : string.Empty)}".Trim();

    public ImageData(string fileName) {
        File = fileName;
    }

    public ImageData(Size s, string filename) {
        File = filename;
        InitSize(s);
    }

    public void InitSize(Size s) {
        Width = s.Width;
        Height = s.Height;
        OriginalSize = s;
        Ratio = Width / (double)Height;
        IsLandscape = Ratio > 1.0;
    }

    /// <summary>
    /// Scales the image to the given height in pixels
    /// </summary>
    /// <param name="height">The height to scale to</param>
    /// <returns>The scale factor</returns>
    public double ScaleToHeight(int height) {
        double factor = height / (double)OriginalSize.Height;
        Width = (int)Math.Round(OriginalSize.Width * factor);
        Height = height;

        if (Width < 0 || Height < 0) {
            throw new InvalidOperationException(string.Format("Bad Scale! Factor: {0}, OriginalSize: {1}, Width: {2}, Height: {3}", factor, OriginalSize, Width, Height));
        }
        return factor;
    }

    public void Scale(double factor) {
        Width = (int)Math.Round(Width * factor);
        Height = (int)Math.Round(Height * factor);
    }

    public void Pad(int padding, bool padRight = false, bool padBottom = false) {
        Width -= (padding + (padRight ? padding : 0));
        Height -= (padding + (padBottom ? padding : 0));
        X += padding;
        Y += padding;
    }

    public void MoveTo(Point p) {
        X = p.X;
        Y = p.Y;
    }

    public override string ToString() {
        return FileName;
    }

}
