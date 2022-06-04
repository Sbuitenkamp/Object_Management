namespace Object_management.Entity;

public class Sale
{
    public int id { get; set; }
    public int days_to_rent { get; set; }
    public int days_to_pay { get; set; }
    public bool IsApplied { get; set; }
}