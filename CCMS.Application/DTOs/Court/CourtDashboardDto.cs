namespace CCMS.Application.DTOs.Court
{
    public class CourtDashboardDto
    {
        public int TotalCases { get; set; }
        public int PendingCases { get; set; }
        public int AccountValidatedCases { get; set; }
        public int AccountNotFoundCases { get; set; }
        public int FreezeAppliedCases { get; set; }
        public int BalanceProvidedCases { get; set; }
    }
}
