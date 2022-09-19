
using Aspose.Zip.SevenZip;

namespace csm.Logic {
    public class SevenZipFileSource : ArchiveFileSource {
        public SevenZipFileSource(string path) : base(path) {
        }

        public static bool Supports(string extension) => extension == ".7z";

        protected override void Extract() {
            Console.WriteLine("SevenZipFileSource - Extracting 7z file to {0}", _tempDir.FullName);
            using var archive = new SevenZipArchive(_archiveFilePath);
            archive.ExtractToDirectory(_tempDir.FullName);
            Console.WriteLine("SevenZipFileSource - Extraction complete");
        }
    }
}
