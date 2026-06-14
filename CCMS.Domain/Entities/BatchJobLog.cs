using System;
using CCMS.Domain.Common;

namespace CCMS.Domain.Entities;

public class BatchJobLog : BaseEntity
{
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int TotalProcessed { get; set; }
    public int ValidatedCount { get; set; }
    public int NotFoundCount { get; set; }
    public long DurationMilliseconds { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsManualRun { get; set; }
}
