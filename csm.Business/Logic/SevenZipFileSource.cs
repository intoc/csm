using Aspose.Zip.SevenZip;

namespace csm.Business.Logic {
    public class SevenZipFileSource : ArchiveFileSource {
        public SevenZipFileSource(string path) : base(path) {
        }

        public static bool Supports(string extension) => extension == ".7z";

        protected override void Extract() {
            using var archive = new SevenZipArchive(_archiveFilePath);
            archive.ExtractToDirectory(_tempDir.FullName);
        }
    }
}
