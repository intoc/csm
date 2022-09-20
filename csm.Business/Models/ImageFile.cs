namespace csm.Business.Models {
    public class ImageFile {

        public bool Hidden { get; private set; }

        public string Path { get; private set; }

        public string? Directory => Path.Contains('\\') ? Path[..(Path.LastIndexOf('\\') + 1)] : @".\";

        public string? FileName => Path.Contains('\\') ? Path[(Path.LastIndexOf('\\') + 1)..] : Path;

        public string? Extension => Path.Contains('.') ? Path[Path.LastIndexOf('.')..] : null;

        public ImageFile(string path, bool hidden = false) {
            Path = path;
            Hidden = hidden;
        }

        public ImageFile(FileInfo info) {
            Path = info.FullName;
            Hidden = (info.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
        }
    }
}
