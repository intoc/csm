using System.IO.Compression;

namespace csm.Logic {
    public class ZipFileSource : ArchiveFileSource {

        public ZipFileSource(string path, object lockObject) : base(path, lockObject) { }

        protected override void Extract() {
            if (_extracted) {
                // Extraction may have happened while we were waiting for the lock
                return;
            }
            Console.WriteLine("ZipFileSource - Extracting zip file to {0}", _tempDir.FullName);
            ZipFile.ExtractToDirectory(_archiveFilePath, _tempDir.FullName, true);
            _extracted = true;
            Console.WriteLine("ZipFileSource - Extraction complete");
        }
    }
}
