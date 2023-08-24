
namespace FinanceManagement.APIs.OutcomingEntries.Dto
{
    public class GetFileDto
    {
        public long Id { get; set; }
        public string FileName { get; set; }
        public byte[] Data { get; set; }
    }
}
