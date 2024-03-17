namespace TimeShareProject.Models
{
    public class PaypalOrderRequest
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public bool Status { get; set; }
    }
}
