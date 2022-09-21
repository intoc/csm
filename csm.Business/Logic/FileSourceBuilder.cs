namespace csm.Business.Logic {
    public class FileSourceBuilder : IFileSourceBuilder {
        public IFileSource Build(string? path) {
            if (path == null) {
                throw new ArgumentNullException(nameof(path));
            }
            if (Directory.Exists(path)) {
                return new DirectoryFileSource(path);
            }
            if (!File.Exists(path)) {
                throw new FileNotFoundException("No source directory or file found with the given path", path);
            }
            var info = new FileInfo(path);
            if (ZipFileSource.Supports(info.Extension)) {
                return new ZipFileSource(path);
            }
            if (RarFileSource.Supports(info.Extension)) {
                return new RarFileSource(path);
            }
            if (SevenZipFileSource.Supports(info.Extension)) {
                return new SevenZipFileSource(path);
            }
            throw new NotImplementedException($"({info.Extension}) file source not supported.");
        }
    }
}
