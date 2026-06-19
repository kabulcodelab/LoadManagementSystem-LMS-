using LoadManagementSystem_LMS_.Data;
using LoadManagementSystem_LMS_.Models;
using Microsoft.EntityFrameworkCore;

namespace LoadManagementSystem_LMS_.Services;

public class DocumentService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public DocumentService(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // ================================================
    // UPLOAD FILE (with DocumentType)
    // ================================================
    public async Task Upload(
        IFormFile file,
        int loadId,
        DocumentType fileType,
        string? uploadedBy = null,
        string? description = null)
    {
        // Ensure upload directory exists
        var folder = Path.Combine(_env.WebRootPath, "uploads", "loads");

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        // Generate unique file name to avoid collisions
        var uniqueFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var fullPath = Path.Combine(folder, uniqueFileName);

        // Save physical file
        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Create document record
        var document = new Document
        {
            LoadId = loadId,
            FileName = file.FileName,
            FilePath = "/uploads/loads/" + uniqueFileName,
            ContentType = file.ContentType,
            FileSize = (int)file.Length,
            FileType = fileType,
            UploadedAt = DateTime.Now,
            UploadedBy = uploadedBy ?? "System",
            Description = description
        };

        // Save to database
        _context.Documents.Add(document);
        await _context.SaveChangesAsync();
    }

    // ================================================
    // GET DOCUMENTS BY LOAD ID
    // ================================================
    public async Task<List<Document>> GetByLoad(int loadId)
    {
        return await _context.Documents
            .Where(d => d.LoadId == loadId)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync();
    }

    // ================================================
    // GET DOCUMENT BY ID
    // ================================================
    public async Task<Document?> GetById(int id)
    {
        return await _context.Documents
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    // ================================================
    // DELETE DOCUMENT (physical file + record)
    // ================================================
    public async Task Delete(int id)
    {
        var doc = await _context.Documents.FindAsync(id);
        if (doc == null) return;

        // Delete physical file if exists
        if (!string.IsNullOrEmpty(doc.FilePath))
        {
            var fullPath = Path.Combine(_env.WebRootPath, doc.FilePath.TrimStart('/'));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        // Delete database record
        _context.Documents.Remove(doc);
        await _context.SaveChangesAsync();
    }

    // ================================================
    // DELETE ALL DOCUMENTS FOR A LOAD (for cascading delete)
    // ================================================
    public async Task DeleteAllByLoad(int loadId)
    {
        var docs = await _context.Documents
            .Where(d => d.LoadId == loadId)
            .ToListAsync();

        foreach (var doc in docs)
        {
            // Delete physical files
            if (!string.IsNullOrEmpty(doc.FilePath))
            {
                var fullPath = Path.Combine(_env.WebRootPath, doc.FilePath.TrimStart('/'));
                if (File.Exists(fullPath))
                    File.Delete(fullPath);
            }
        }

        _context.Documents.RemoveRange(docs);
        await _context.SaveChangesAsync();
    }

    // ================================================
    // GET PAGED DOCUMENTS (with search & filter)
    // ================================================
    public async Task<PagedResult<Document>> GetPaged(
        int page,
        int pageSize = 10,
        string? searchLoadNumber = null,
        DocumentType? fileType = null,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        var query = _context.Documents
            .Include(d => d.Load)
            .AsQueryable();

        // 🔍 Search by Load Number
        if (!string.IsNullOrWhiteSpace(searchLoadNumber))
        {
            query = query.Where(d => d.Load != null && d.Load.LoadNumber.Contains(searchLoadNumber));
        }

        // 🏷️ Filter by File Type
        if (fileType.HasValue)
        {
            query = query.Where(d => d.FileType == fileType.Value);
        }

        // 📅 Filter by Upload Date
        if (fromDate.HasValue)
            query = query.Where(d => d.UploadedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(d => d.UploadedAt <= toDate.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(d => d.UploadedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Document>
        {
            Items = items,
            TotalCount = totalCount,
            PageSize = pageSize,
            CurrentPage = page
        };
    }
}