namespace Aurorae.Models.Gallery;

public class ItemViewModel
{
    public ItemViewModel(string path)
    {
        if (path.StartsWith(LocalPath.Gallery))
            Location = ItemLocation.Gallery;
        else if (path.StartsWith(LocalPath.Document))
            Location = ItemLocation.Document;

        ItemPath = (Location switch
        {
            ItemLocation.Gallery => Path.GetRelativePath(LocalPath.Gallery, path),
            ItemLocation.Document => Path.GetRelativePath(LocalPath.Document, path),
            _ => "OUTOFBOUNDARYALLOWED"
        }).Replace('\\', '/');
    }

    public ItemLocation Location { get; }
    public string ItemPath { get; }

    public enum ItemLocation { Unknown, Gallery, Document }
}
