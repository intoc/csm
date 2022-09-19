using System.IO.Compression;

namespace csm.Business.Logic {
    public class ZipFileSource : ArchiveFileSource {

        public ZipFileSource(string path) : base(path) { }

        public static bool Supports(string extension) => extension == ".zip";

        protected override void Extract() {
            ZipFile.ExtractToDirectory(_archiveFilePath, _tempDir.FullName, true);
        }
    }
}
