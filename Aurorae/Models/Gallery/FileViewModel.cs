using Aurorae.Controllers;

namespace Aurorae.Models.Gallery;

public class FileViewModel : ItemViewModel
{
    public FileViewModel(string path) : this(new FileInfo(path)) { }
    public FileViewModel(FileInfo file) : base(file.FullName)
    {
        FileInfo = file;
    }

    public FileInfo FileInfo { get; }

    public FolderViewModel? Parent => FileInfo.Directory is { } parent ? new(parent) : null;

    public string ContentType => ResourceController.GetContentType(FileInfo.Name);
    public bool IsImage => ContentType.StartsWith("image");
    public bool IsText => ContentType.StartsWith("text");
    public bool IsLink => FileInfo.Extension.Equals(".url", StringComparison.OrdinalIgnoreCase);
    public string IconName
        => IsImage ? "image"
          : IsText ? "text-left"
          : IsLink ? "link"
                   : "file";

    public (FileViewModel? Prev, FolderViewModel? Parent, FileViewModel? Next) GetNeighbors()
    {
        var parent = Parent;
        if (parent == null)
            return (null, null, null);

        var peers = parent.FilePathArray;
        Array.Sort(peers, LexicographicStringComparer.Comparison);
        var selfIndex = peers.IndexOf(FileInfo.FullName);
        FileViewModel? prev = null, next = null;
        if (selfIndex > 0)
            prev = new(peers[selfIndex - 1]);
        if (selfIndex < peers.Length - 1)
            next = new(peers[selfIndex + 1]);
        return (prev, parent, next);
    }
}
