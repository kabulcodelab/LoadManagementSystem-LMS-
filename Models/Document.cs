using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadManagementSystem_LMS_.Models;

public class Document
{
    public int Id { get; set; }

    [Required]
    public int LoadId { get; set; }
    [ForeignKey(nameof(LoadId))]
    public Load? Load { get; set; }

    [Required(ErrorMessage = "File name is required")]
    public string FileName { get; set; } = string.Empty;

    public string? FilePath { get; set; }

    public byte[]? FileContent { get; set; }

    public string? ContentType { get; set; }

    public int? FileSize { get; set; }

    [Required(ErrorMessage = "Document type is required")]
    public DocumentType FileType { get; set; } = DocumentType.Others;

    public DateTime? UploadedAt { get; set; } = DateTime.Now; // ✅ تغییر به nullable

    public string? UploadedBy { get; set; }

    public string? Description { get; set; }
}

public enum DocumentType
{
    [Display(Name = "Rate Confirmation")]
    RateConfirmation,

    [Display(Name = "Bill of Lading")]
    BillOfLading,

    [Display(Name = "Others")]
    Others
}