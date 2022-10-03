using Aspose.Zip.SevenZip;
using Serilog;

namespace csm.Business.Logic {
    public class SevenZipFileSource : ArchiveFileSource {
        public SevenZipFileSource(string path, ILogger logger) : base(path, logger) {
        }

        public static bool Supports(string extension) => extension == ".7z";

        protected override void Extract() {
            using var archive = new SevenZipArchive(_archiveFilePath);
            archive.ExtractToDirectory(_tempDir.FullName);
        }
    }
}
