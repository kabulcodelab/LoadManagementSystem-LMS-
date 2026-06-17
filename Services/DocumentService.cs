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

    // 📤 Upload file
    public async Task Upload(IFormFile file, int loadId)
    {
        var folder = Path.Combine(_env.WebRootPath, "uploads", "loads");

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

        var fullPath = Path.Combine(folder, fileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var doc = new Document
        {
            FileName = file.FileName,
            FilePath = "/uploads/loads/" + fileName,
            FileType = file.ContentType,
            LoadId = loadId
        };

        _context.Documents.Add(doc);
        await _context.SaveChangesAsync();
    }

    // 📥 Get files by Load
    public async Task<List<Document>> GetByLoad(int loadId)
    {
        return await _context.Documents
            .Where(x => x.LoadId == loadId)
            .ToListAsync();
    }

    // 🗑 Delete file
    public async Task Delete(int id)
    {
        var doc = await _context.Documents.FindAsync(id);

        if (doc != null)
        {
            var fullPath = Path.Combine(_env.WebRootPath, doc.FilePath.TrimStart('/'));

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            _context.Documents.Remove(doc);
            await _context.SaveChangesAsync();
        }
    }
}