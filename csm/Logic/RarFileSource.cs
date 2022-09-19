﻿using Aspose.Zip.Rar;

namespace csm.Logic {
    public class RarFileSource : ArchiveFileSource {

        public RarFileSource(string path) : base(path) {
        }

        public static bool Supports(string extension) => extension == ".rar";

        protected override void Extract() {
            Console.WriteLine("RarFileSource - Extracting rar file to {0}", _tempDir.FullName);
            using var archive = new RarArchive(_archiveFilePath);
            archive.ExtractToDirectory(_tempDir.FullName);
            Console.WriteLine("RarFileSource - Extraction complete");
        }
    }
}
