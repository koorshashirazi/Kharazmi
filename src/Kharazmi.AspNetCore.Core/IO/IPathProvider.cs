namespace Kharazmi.AspNetCore.Core.IO
{
    public interface IPathProvider {
        string MapPath(string path);
    }
}