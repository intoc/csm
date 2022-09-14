using System;
using System.Drawing;
using System.Linq;

namespace csm.Models {
    public class ImageData {

        public bool Include = true;

        public int Width { get; set; }
        public int Height { get; set; }
        public double Ratio { get; private set; }
        public bool IsLandscape { get; private set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string File { get; set; }

        public Rectangle Bounds {
            get {
                return new Rectangle(X, Y, Width, Height);
            }
        }

        public string Orientation {
            get {
                return IsLandscape ? "Landscape" : "Portrait";
            }
        }
        public string FileName {
            get {
                return File.Split('\\').Last();
            }
        }
        public Size OriginalSize { get; private set; }
        public bool IsLogo { get; set; }

        public ImageData(Size s, string filename) {
            Width = s.Width;
            Height = s.Height;
            OriginalSize = s;
            Ratio = Width / (double)Height;
            IsLandscape = Ratio > 1.0;
            File = filename;
            IsLogo = false;
        }

        /// <summary>
        /// Scales the image to the given height in pixels
        /// </summary>
        /// <param name="height">The height to scale to</param>
        /// <returns>The scale factor</returns>
        public double ScaleToHeight(double height) {
            double factor = height / OriginalSize.Height;
            Width = (int)Math.Round(OriginalSize.Width * factor);
            Height = (int)height;

            if (Width < 0 || Height < 0) {
                throw new Exception(string.Format("Bad Scale! Factor: {0}, OriginalSize: {1}, Width: {2}, Height: {3}", factor, OriginalSize, Width, Height));
            }
            return factor;
        }

        public override string ToString() {
            return FileName;
        }

    }
}
