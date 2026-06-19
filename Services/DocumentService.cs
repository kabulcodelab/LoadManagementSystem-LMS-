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
    // UPLOAD FILE (unchanged)
    // ================================================
    public async Task Upload(
        IFormFile file,
        int loadId,
        DocumentType fileType,
        string? uploadedBy = null,
        string? description = null)
    {
        var folder = Path.Combine(_env.WebRootPath, "uploads", "loads");
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        var uniqueFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var fullPath = Path.Combine(folder, uniqueFileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

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

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();
    }

    // ================================================
    // GET DOCUMENTS BY LOAD ID (metadata only – no binary)
    // ================================================
    public async Task<List<Document>> GetByLoad(int loadId)
    {
        return await _context.Documents
            .Where(d => d.LoadId == loadId)
            .Select(d => new Document
            {
                Id = d.Id,
                FileName = d.FileName,
                FileType = d.FileType,
                FileSize = d.FileSize,
                UploadedAt = d.UploadedAt,
                ContentType = d.ContentType,
                FilePath = d.FilePath,
                // FileContent is NOT selected – keeps it light
            })
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync();
    }

    // ================================================
    // GET DOCUMENT WITH BINARY CONTENT (for viewing)
    // ================================================
    public async Task<Document?> GetDocumentWithContent(int id)
    {
        return await _context.Documents
            .Where(d => d.Id == id)
            .Select(d => new Document
            {
                Id = d.Id,
                FileName = d.FileName,
                FileContent = d.FileContent,
                ContentType = d.ContentType
            })
            .FirstOrDefaultAsync();
    }

    // ================================================
    // GET DOCUMENT BY ID (full entity – legacy, keep as is)
    // ================================================
    public async Task<Document?> GetById(int id)
    {
        return await _context.Documents
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    // ================================================
    // DELETE DOCUMENT (unchanged)
    // ================================================
    public async Task Delete(int id)
    {
        var doc = await _context.Documents.FindAsync(id);
        if (doc == null) return;

        if (!string.IsNullOrEmpty(doc.FilePath))
        {
            var fullPath = Path.Combine(_env.WebRootPath, doc.FilePath.TrimStart('/'));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        _context.Documents.Remove(doc);
        await _context.SaveChangesAsync();
    }

    // ================================================
    // DELETE ALL DOCUMENTS FOR A LOAD (unchanged)
    // ================================================
    public async Task DeleteAllByLoad(int loadId)
    {
        var docs = await _context.Documents
            .Where(d => d.LoadId == loadId)
            .ToListAsync();

        foreach (var doc in docs)
        {
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
    // GET PAGED DOCUMENTS (metadata only, already fine)
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

        if (!string.IsNullOrWhiteSpace(searchLoadNumber))
        {
            query = query.Where(d => d.Load != null && d.Load.LoadNumber.Contains(searchLoadNumber));
        }

        if (fileType.HasValue)
        {
            query = query.Where(d => d.FileType == fileType.Value);
        }

        if (fromDate.HasValue)
            query = query.Where(d => d.UploadedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(d => d.UploadedAt <= toDate.Value);

        var totalCount = await query.CountAsync();

        // Project to metadata only (omit FileContent)
        var items = await query
            .OrderByDescending(d => d.UploadedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(d => new Document
            {
                Id = d.Id,
                FileName = d.FileName,
                FileType = d.FileType,
                FileSize = d.FileSize,
                UploadedAt = d.UploadedAt,
                ContentType = d.ContentType,
                FilePath = d.FilePath,
                Load = d.Load
            })
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