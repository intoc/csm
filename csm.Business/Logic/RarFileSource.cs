using Aspose.Zip.Rar;
using csm.Business.Models;
using Serilog;

namespace csm.Business.Logic {
    public class RarFileSource : ArchiveFileSource {

        public RarFileSource(string path, ILogger logger) : base(path, logger) { }

        public static bool Supports(string extension) => extension == ".rar";

        private int _progressStep;

        protected override void Extract() {
            using var archive = new RarArchive(_archiveFilePath);
            foreach (var entry in archive.Entries) {
                entryCompletion[entry.Name] = false;
                entry.ExtractionProgressed += RarFileSource_ExtractionProgressed;
            }
            archive.ExtractToDirectory(_tempDir.FullName);
        }

        protected void RarFileSource_ExtractionProgressed(object? sender, Aspose.Zip.ProgressEventArgs e) {
            if (sender is RarArchiveEntry entry && entry.UncompressedSize == e.ProceededBytes && !entryCompletion[entry.Name]) {
                entryCompletion[entry.Name] = true;

                // Update counters
                int resolution = 10;
                int progress = entryCompletion.Values.Count(v => v);

                double progressFraction = progress / (double)entryCompletion.Count;
                int step = (int)Math.Floor(progressFraction * resolution);

                // Send a limited number of progress updates to the listeners
                if (step > _progressStep) {
                    ++_progressStep;
                    // Send progress to listeners
                    UpdateProgress(new ProgressEventArgs(progress, entryCompletion.Count, _timer?.Elapsed ?? TimeSpan.Zero, FullPath));
                }
            }
        }
    }
}
