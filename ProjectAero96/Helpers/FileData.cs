using Microsoft.Extensions.FileProviders;

namespace ProjectAero96.Helpers
{
    public abstract class FileData : IFileInfo
    {
        public abstract string Name { get; }
        public abstract byte[] Content { get; }
        public abstract string ContentType { get; }
        public long Length => Content.LongLength;
        public DateTimeOffset LastModified => DateTimeOffset.UtcNow;

        public string PhysicalPath => string.Empty;
        public bool Exists => true;
        public bool IsDirectory => false;

        public Stream CreateReadStream() => new MemoryStream(Content);
    }
}
