namespace LoadManagementSystem_LMS_.Models;

public class Document
{
    public int Id { get; set; }

    public int LoadId { get; set; }

    public Load? Load { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.Now;
}