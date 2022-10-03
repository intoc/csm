using Serilog;

namespace csm.Business.Logic {
    public class FileSourceBuilder : IFileSourceBuilder {
        public IFileSource Build(string? path, ILogger logger) {
            if (path == null) {
                throw new ArgumentNullException(nameof(path));
            }
            if (Directory.Exists(path)) {
                return new DirectoryFileSource(path, logger);
            }
            if (!File.Exists(path)) {
                throw new FileNotFoundException("No source directory or file found with the given path", path);
            }
            var info = new FileInfo(path);
            if (ZipFileSource.Supports(info.Extension)) {
                return new ZipFileSource(path, logger);
            }
            if (RarFileSource.Supports(info.Extension)) {
                return new RarFileSource(path, logger);
            }
            if (SevenZipFileSource.Supports(info.Extension)) {
                return new SevenZipFileSource(path, logger);
            }
            throw new NotImplementedException($"({info.Extension}) file source not supported.");
        }
    }
}
