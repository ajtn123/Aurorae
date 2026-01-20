using System.Text.RegularExpressions;

namespace Aurorae.Models.Gallery;

public partial class FolderViewModel : ItemViewModel
{
    [GeneratedRegex(@"\*+")]
    private static partial Regex FilterNormalizer();

    public FolderViewModel(string path, string? filter = null, bool recursive = false) : this(new DirectoryInfo(path), filter, recursive) { }
    public FolderViewModel(DirectoryInfo directory, string? filter = null, bool recursive = false) : base(directory.FullName)
    {
        DirectoryInfo = directory;
        Filter = FilterNormalizer().Replace($"*{filter}*", "*");
        IsRecursive = recursive;
        enumeration = new()
        {
            MatchCasing = MatchCasing.CaseInsensitive,
            AttributesToSkip = FileAttributes.Hidden | FileAttributes.System,
            IgnoreInaccessible = true,
            RecurseSubdirectories = recursive,
        };
    }

    public DirectoryInfo DirectoryInfo { get; }
    public string Filter { get; }
    public bool IsRecursive { get; set; }

    public bool IsRoot => ItemPath == ".";
    public FolderViewModel? Parent => DirectoryInfo.Parent is { } parent ? new(parent) : null;

    private readonly EnumerationOptions enumeration;
    private static readonly LexicographicStringComparer comparer = new();

    public string[] FolderPathArray => Directory.GetDirectories(DirectoryInfo.FullName, Filter, enumeration);
    public string[] FilePathArray => Directory.GetFiles(DirectoryInfo.FullName, Filter, enumeration);
    public IEnumerable<string> FolderPaths => Directory.EnumerateDirectories(DirectoryInfo.FullName, Filter, enumeration).Order(comparer);
    public IEnumerable<string> FilePaths => Directory.EnumerateFiles(DirectoryInfo.FullName, Filter, enumeration).Order(comparer);
    public IEnumerable<DirectoryInfo> FolderInfos => DirectoryInfo.EnumerateDirectories(Filter, enumeration).OrderBy(x => x.Name, comparer);
    public IEnumerable<FileInfo> FileInfos => DirectoryInfo.EnumerateFiles(Filter, enumeration).OrderBy(x => x.Name, comparer);
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
