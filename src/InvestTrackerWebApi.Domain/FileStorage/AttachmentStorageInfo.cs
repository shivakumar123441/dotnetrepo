namespace InvestTrackerWebApi.Domain.FileStorage;
using System;

public class AttachmentStorageInfo
{
    public Guid Id { get; set; }
    public string? FileName { get; set; }
    public string? UploadedFileUrl { get; set; }
}
