﻿using SixLabors.ImageSharp;
using static System.Formats.Asn1.AsnWriter;

namespace csm.Business.Models;

public class ImageData {
    public int Width { get; set; }
    public int Height { get; set; }
    public double Ratio { get; private set; }
    public bool IsLandscape { get; private set; }
    public int X { get; set; }
    public int Y { get; set; }
    public string File { get; set; }
    public Size OriginalSize { get; private set; }
    public bool Include { get; set; } = true;
    public bool InclusionPinned { get; set; } = false;

    public Rectangle Bounds => new(X, Y, Width, Height);

    public Point Origin => new(X, Y);

    public string Orientation => IsLandscape ? "Landscape" : "Portrait";

    public string FileName => Path.GetFileName(File);

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
        Width = (int)(Width * factor);
        Height = (int)(Height * factor);
    }

    public void Pad(int padding) {
        Width -= (padding * 2);
        Height -= (padding * 2);
        X += padding;
        Y += padding;
    }

    public override string ToString() {
        return FileName;
    }

}
