using System;

namespace csm.Models {
    public class DrawProgressEventArgs : EventArgs {
        public int Percentage { get; private set; }
        public int Progress { get; private set; }
        public int Total { get; private set; }

        public TimeSpan Time { get; private set; }

        public DrawProgressEventArgs(int progress, int total, TimeSpan elapsed) {
            Progress = progress;
            Total = total;
            Percentage = (int)(Progress / (double)Total * 100.0);
            Time = elapsed;
        }
    }
}
