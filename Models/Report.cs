namespace e_BookPvt.Models
{
    public class Report
    {
        public string CustomerName { get; set; }
        public string BookName { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
