using Object_management.Entity;

namespace Object_management.Models.FormDataFormats;

public class CalculatedPrice
{
    public List<Sale> AppliedSales = new List<Sale>();
    public double FullPrice;
}