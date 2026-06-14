using System;

namespace CCMS.Domain.Entities
{
    public class Attachment
    {
        public int Id { get; set; }
        public int CaseId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadedDate { get; set; }
    }
}
