using System;

namespace CCMS.Application.DTOs.Court
{
    public class AttachmentDto
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadedDate { get; set; }
    }
}
