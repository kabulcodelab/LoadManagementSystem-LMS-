namespace LoadManagementSystem_LMS_.Models;

public class Document
{
    public int Id { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public string FileType { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; } = DateTime.Now;

    // 🔗 Link to Load
    public int LoadId { get; set; }
    public Load? Load { get; set; }
}