using System;

namespace FinanceManagement.APIs.DashBoards.Dto
{
    public class FinanceReviewExplainDto
    {
        public long Id { get; set; }
        public DateTime CreationTime { get; set; }
        public double IncomingVND { get; set; }
        public double IncomingUSD { get; set; }
        public double IncomingVNDTransaction { get; set; }
        public double IncomingUSDTransaction { get; set; }
        public double OutcomingVND { get; set; }
        public double OutcomingUSD { get; set; }
        public double OutcomingVNDTransaction { get; set; }
        public double OutcomingUSDTransaction { get; set; }
        public double IncomingDiffVND { get; set; }
        public double IncomingDiffUSD { get; set; }
        public double OutcomingDiffVND { get; set; }
        public double OutcomingDiffUSD { get; set; }
        public string IncomingDiffVNDNote { get; set; }
        public string IncomingDiffUSDNote { get; set; }
        public string OutcomingDiffVNDNote { get; set; }
        public string OutcomingDiffUSDNote { get; set; }
    }
}
