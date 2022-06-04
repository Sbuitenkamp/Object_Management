namespace Object_management.Entity;

public class ObjectData
{
    public int object_number { get; set; }
    public int object_type_id { get; set; }
    public bool loaned_out { get; set; }
    public bool in_service { get; set; }
    public string? size { get; set; } = string.Empty;

    public ObjectType Type { get; set; } = new ObjectType();
    public List<Sale> AppliedSales { get; private set; } = new List<Sale>();

    public double CalculatePrice(DateTime startDate, DateTime returnDate)
    {
        double totalDays = (returnDate - startDate).TotalDays;
        double countableDays = totalDays; // days to search for applicable sales
        
        // get the applicable sales based on total days rented
        AppliedSales = Type.Sales.Where(sale => {
            if (!(countableDays / sale.days_to_rent >= 1)) return false;
            countableDays -= sale.days_to_rent;
            return true;
        }).ToList();

        double fullPrice = Type.price * totalDays;
        if (AppliedSales.Count == 0) return fullPrice; // no sales return full price
        
        // apply the sales
        fullPrice = 0;
        foreach (Sale sale in AppliedSales) {
            fullPrice += Type.price * sale.days_to_pay;
            totalDays -= sale.days_to_rent;
        }
        
        // add up any remaining days that don't fit the sales
        if (totalDays > 0) fullPrice += Type.price * totalDays;
        return fullPrice;
    }
}