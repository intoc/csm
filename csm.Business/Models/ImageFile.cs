using IOPath = System.IO.Path;

namespace csm.Business.Models;

public class ImageFile {

    public bool Hidden { get; private set; }

    public string Path { get; private set; }

    public string? Directory =>  IOPath.GetDirectoryName(Path);

    public string? FileName => IOPath.GetFileName(Path);
       
    public string Extension => IOPath.GetExtension(Path);

    public long Bytes { get; private set; }

    public ImageFile(string path, bool hidden = false) {
        this.Path = path;
        Hidden = hidden;
    }

    public ImageFile(FileInfo info) {
        Path = info.FullName;
        Hidden = (info.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
        Bytes = info.Length;
    }
}

