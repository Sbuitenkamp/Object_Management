using Object_management.Entity;

namespace Object_management.Models.FormDataFormats;

public class Form
{
    public string QueryType { get; set; }
    public int Offset { get; set; }
    public List<ObjectType> ObjectTypes { get; set; } = new List<ObjectType>();
    public List<ObjectData> Objects { get; set; } = new List<ObjectData>();
    public List<Customer> Customers { get; set; } = new List<Customer>();
    public List<Reservation> Reservations { get; set; } = new List<Reservation>();
    public List<Sale> Sales { get; set; } = new List<Sale>();
}