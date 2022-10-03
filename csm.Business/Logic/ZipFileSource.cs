using Serilog;
using System.IO.Compression;

namespace csm.Business.Logic {
    public class ZipFileSource : ArchiveFileSource {

        public ZipFileSource(string path, ILogger logger) : base(path, logger) { }

        public static bool Supports(string extension) => extension == ".zip";

        protected override void Extract() {
            ZipFile.ExtractToDirectory(_archiveFilePath, _tempDir.FullName, true);
        }
    }
}
