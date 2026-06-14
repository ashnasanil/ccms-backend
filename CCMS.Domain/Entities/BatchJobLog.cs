// FILE: Entities/BatchJobLog.cs
using System;
using CCMS.Domain.Common;

namespace CCMS.Domain.Entities
{
    public class BatchJobLog : BaseEntity
    {
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public int ProcessedCount { get; set; }
        public int ValidatedCount { get; set; }
        public int NotFoundCount { get; set; }
        public long DurationMs { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
