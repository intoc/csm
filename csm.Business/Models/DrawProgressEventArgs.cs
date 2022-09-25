namespace csm.Business.Models;
public class ProgressEventArgs : EventArgs {
    public float Percentage => Progress / (float)Total;
    public int Progress { get; private set; }
    public int Total { get; private set; }

    public TimeSpan Time { get; private set; }

    public ProgressEventArgs(int progress, int total, TimeSpan elapsed) {
        Progress = progress;
        Total = total;
        Time = elapsed;
    }
}
