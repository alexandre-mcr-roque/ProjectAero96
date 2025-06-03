using System.Drawing;

namespace ProjectAero96.Models
{
    public interface IMailFileModel
    {
        string FileName { get; set; }
        byte[] FileData { get; set; }
        string ContentType { get; set; }
    }
}