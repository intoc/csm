using Aspose.Zip.Rar;
using csm.Business.Models;

namespace csm.Business.Logic {
    public class RarFileSource : ArchiveFileSource {

        public RarFileSource(string path) : base(path) { }

        public static bool Supports(string extension) => extension == ".rar";

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
                int progress = entryCompletion.Values.Count(v => v);
                UpdateProgress(new ProgressEventArgs(progress, entryCompletion.Count, _timer?.Elapsed ?? TimeSpan.Zero));
            }
        }
    }
}
