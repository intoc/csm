namespace csm.Business.Models;
public class ProgressEventArgs : EventArgs {
    public float Percentage => Progress / (float)Total;
    public int Progress { get; private set; }
    public int Total { get; private set; }

    public TimeSpan Time { get; private set; }

    public string EntityInProgress { get; set; }

    public ProgressEventArgs(int progress, int total, TimeSpan elapsed, string? entity = "Unknown Entity") {
        Progress = progress;
        Total = total;
        Time = elapsed;
        EntityInProgress = entity ?? "Unknown Entity";
    }
}
