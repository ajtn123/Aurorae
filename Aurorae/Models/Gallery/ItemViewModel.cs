using Aurorae.Utils;

namespace Aurorae.Models.Gallery;

public class ItemViewModel
{
    public ItemViewModel(string path)
    {
        this.path = path;
    }

    private readonly string path;
    public string ItemPath => Path.GetRelativePath(LocalPath.Gallery, path).Replace('\\', '/');
}
