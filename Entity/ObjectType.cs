namespace Object_management.Entity;

public class ObjectType
{
    private List<Sale> _sales = new List<Sale>();
    public int id { get; set; }
    public float price { get; set; }
    public string description { get; set; }

    public List<Sale> Sales
    {
        get => _sales;
        set {
            _sales = value.OrderByDescending(x => x.days_to_pay).ToList();
        }
    }

}