using ArquiDesk.Application.Interfaces;
using ArquiDesk.Domain.Entities;
using Microsoft.AspNetCore.Hosting;

namespace ArquiDesk.Infrastructure.Services;

public class FileStorageService(IWebHostEnvironment environment) : IFileStorageService
{
    public string[] AllowedExtensions { get; } = [".pdf", ".jpg", ".jpeg", ".png", ".dwg", ".docx"];

    public async Task<TicketAttachment> SaveTicketAttachmentAsync(Guid ticketId, string userId, Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException("Tipo de arquivo não permitido.");
        }

        var uploadPath = Path.Combine(environment.WebRootPath, "uploads", "tickets", ticketId.ToString());
        Directory.CreateDirectory(uploadPath);

        var storedName = $"{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(uploadPath, storedName);
        await using var fileStream = File.Create(fullPath);
        await stream.CopyToAsync(fileStream, cancellationToken);

        return new TicketAttachment
        {
            TicketId = ticketId,
            OriginalFileName = fileName,
            StoredFileName = $"/uploads/tickets/{ticketId}/{storedName}",
            ContentType = contentType,
            SizeInBytes = fileStream.Length,
            UploadedByUserId = userId,
            CreatedBy = userId
        };
    }
}
