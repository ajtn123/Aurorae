namespace Aurorae.Models.Gallery;

public class FolderViewModel : ItemViewModel
{
    public FolderViewModel(string path, bool recursive = false) : this(new DirectoryInfo(path), recursive) { }
    public FolderViewModel(DirectoryInfo directory, bool recursive = false) : base(directory.FullName)
    {
        DirectoryInfo = directory;
        IsRecursive = recursive;
        enumeration = new()
        {
            AttributesToSkip = FileAttributes.Hidden | FileAttributes.System,
            IgnoreInaccessible = true,
            RecurseSubdirectories = recursive,
        };
    }

    public DirectoryInfo DirectoryInfo { get; }
    public bool IsRecursive { get; set; }

    public bool IsRoot => ItemPath == ".";
    public FolderViewModel? Parent => DirectoryInfo.Parent is { } parent ? new(parent) : null;

    private readonly EnumerationOptions enumeration;
    private static readonly LexicographicStringComparer comparer = new();

    public string[] FolderPathArray => Directory.GetDirectories(DirectoryInfo.FullName, "*", enumeration);
    public string[] FilePathArray => Directory.GetFiles(DirectoryInfo.FullName, "*", enumeration);
    public IEnumerable<string> FolderPaths => Directory.EnumerateDirectories(DirectoryInfo.FullName, "*", enumeration).Order(comparer);
    public IEnumerable<string> FilePaths => Directory.EnumerateFiles(DirectoryInfo.FullName, "*", enumeration).Order(comparer);
    public IEnumerable<DirectoryInfo> FolderInfos => DirectoryInfo.EnumerateDirectories("*", enumeration).OrderBy(x => x.Name, comparer);
    public IEnumerable<FileInfo> FileInfos => DirectoryInfo.EnumerateFiles("*", enumeration).OrderBy(x => x.Name, comparer);
    public IEnumerable<FolderViewModel> Folders => FolderInfos.Select(dir => new FolderViewModel(dir));
    public IEnumerable<FileViewModel> Files => FileInfos.Select(file => new FileViewModel(file));

    public (FolderViewModel? Prev, FolderViewModel? Parent, FolderViewModel? Next) GetNeighbors()
    {
        var parent = Parent;
        if (parent == null)
            return (null, null, null);

        var peers = parent.FolderPathArray;
        Array.Sort(peers, LexicographicStringComparer.Comparison);
        var selfIndex = peers.IndexOf(DirectoryInfo.FullName);
        FolderViewModel? prev = null, next = null;
        if (selfIndex > 0)
            prev = new(peers[selfIndex - 1]);
        if (selfIndex < peers.Length - 1)
            next = new(peers[selfIndex + 1]);
        return (prev, parent, next);
    }
}
