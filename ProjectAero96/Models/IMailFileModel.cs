using System.Drawing;

namespace ProjectAero96.Models
{
    public interface IMailFileModel
    {
        string FileName { get; }
        byte[] Content { get; }
        string ContentType { get; }
    }
}