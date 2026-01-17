namespace Aurorae.Models.Gallery;

public class FolderViewModel : ItemViewModel
{
    public FolderViewModel(string path) : this(new DirectoryInfo(path)) { }
    public FolderViewModel(DirectoryInfo directory) : base(directory.FullName)
    {
        DirectoryInfo = directory;
    }

    public DirectoryInfo DirectoryInfo { get; }

    public bool IsRoot => ItemPath == ".";
    public FolderViewModel? Parent => DirectoryInfo.Parent is { } parent ? new(parent) : null;

    public string[] FolderPathArray => Directory.GetDirectories(DirectoryInfo.FullName, "*", enumeration);
    public string[] FilePathArray => Directory.GetFiles(DirectoryInfo.FullName, "*", enumeration);
    public IEnumerable<string> FolderPaths => Directory.EnumerateDirectories(DirectoryInfo.FullName, "*", enumeration).Order();
    public IEnumerable<string> FilePaths => Directory.EnumerateFiles(DirectoryInfo.FullName, "*", enumeration).Order();
    public IEnumerable<DirectoryInfo> FolderInfos => DirectoryInfo.EnumerateDirectories("*", enumeration).OrderBy(x => x.Name);
    public IEnumerable<FileInfo> FileInfos => DirectoryInfo.EnumerateFiles("*", enumeration).OrderBy(x => x.Name);
    public IEnumerable<FolderViewModel> Folders => FolderInfos.Select(dir => new FolderViewModel(dir));
    public IEnumerable<FileViewModel> Files => FileInfos.Select(file => new FileViewModel(file));

    private readonly EnumerationOptions enumeration = new()
    {
        AttributesToSkip = FileAttributes.Hidden | FileAttributes.System,
        IgnoreInaccessible = true,
    };

    public (FolderViewModel? Prev, FolderViewModel? Parent, FolderViewModel? Next) GetNeighbors()
    {
        var parent = Parent;
        if (parent == null)
            return (null, null, null);

        var peers = parent.FolderPathArray;
        Array.Sort(peers);
        var selfIndex = peers.IndexOf(DirectoryInfo.FullName);
        FolderViewModel? prev = null, next = null;
        if (selfIndex > 0)
            prev = new(peers[selfIndex - 1]);
        if (selfIndex < peers.Length - 1)
            next = new(peers[selfIndex + 1]);
        return (prev, parent, next);
    }
}
