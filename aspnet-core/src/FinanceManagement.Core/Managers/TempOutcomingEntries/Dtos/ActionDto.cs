namespace FinanceManagement.Managers.TempOutcomingEntries.Dtos
{
    public class ActionDto
    {
        public long StatusTransitionId { get; set; }
        public long WorkflowId { get; set; }
        public long FromStatusId { get; set; }
        public long ToStatusId { get; set; }
        public string Name { get; set; }
    }
}