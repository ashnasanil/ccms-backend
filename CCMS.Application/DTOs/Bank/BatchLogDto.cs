using System;

namespace CCMS.Application.DTOs.Bank
{
    public class BatchLogDto
    {
        public Guid Id { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public int TotalProcessed { get; set; }
        public int ValidatedCount { get; set; }
        public int NotFoundCount { get; set; }
        public long DurationMilliseconds { get; set; }
        public bool IsManualRun { get; set; }
    }
}
