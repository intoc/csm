using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csm.Logic {
    internal class DirectoryFileSource : IFileSource {

        private readonly DirectoryInfo? _directory;

        public DirectoryFileSource(string? path) {
            if (path != null) {
                _directory = new DirectoryInfo(path);
            }
        }

        public bool IsReady => _directory != null;

        public IEnumerable<FileInfo> GetFiles() {
            if (_directory == null) {
                return Enumerable.Empty<FileInfo>();
            }
            return _directory.GetFiles();
        }
    }
}
