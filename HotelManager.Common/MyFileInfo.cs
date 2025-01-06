namespace HotelManager.Common;

public interface IFileInfo
{
    bool Exists { get; }
    Stream OpenRead();
}

public class MyFileInfo : IFileInfo
{
    private readonly FileInfo _fileInfo;

    public MyFileInfo(FileInfo fileInfo)
    {
        _fileInfo = fileInfo;
    }

    public bool Exists => _fileInfo.Exists;
    public Stream OpenRead()
    {
        return _fileInfo.OpenRead();
    }
}