namespace ProjectAero96.Data.Entities
{
    public class TempInvoice : Invoice
    {
        public User User { get; set; } = null!;
    }
}
