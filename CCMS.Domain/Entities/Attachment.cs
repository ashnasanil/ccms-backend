// FILE: Entities/Attachment.cs
using System;
using CCMS.Domain.Common;

namespace CCMS.Domain.Entities
{
    public class Attachment : BaseEntity
    {
        public Guid CaseId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string StoragePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        
        public Case Case { get; set; } = null!;
    }
}
