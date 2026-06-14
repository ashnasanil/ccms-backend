namespace CCMS.Application.Features.Bank.Dashboard.DTOs
{
    public class DashboardSummaryDto
    {
        public int AwaitingAction { get; set; }
        public int PendingBatch { get; set; }
        public int Completed { get; set; }
        public int AutoResolved { get; set; }
    }
}
