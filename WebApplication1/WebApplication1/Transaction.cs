namespace WebApplication1;

public class Transaction
{
    public int ID { get; set; }
    public string PayerName { get; set; }
    public ulong CardNumber { get; set; }
    public string Month { get; set; }
    public string Year { get; set; }
    public int CVC { get; set; }
    public DateTime PaymentDate { get; set; }
    public Decimal Amount { get; set; }
}