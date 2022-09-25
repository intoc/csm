﻿using Aspose.Zip.Rar;

namespace csm.Business.Logic {
    public class RarFileSource : ArchiveFileSource {

        public RarFileSource(string path) : base(path) { }

        public static bool Supports(string extension) => extension == ".rar";

        protected override void Extract() {
            using var archive = new RarArchive(_archiveFilePath);
            foreach (var entry in archive.Entries) {
                entryCompletion[entry.Name] = false;
                entry.ExtractionProgressed += ArchiveFileSource_ExtractionProgressed;
            }
            archive.ExtractToDirectory(_tempDir.FullName);
        }
    }
}
