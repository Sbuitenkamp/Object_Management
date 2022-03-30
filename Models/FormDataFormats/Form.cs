using Object_management.Entity;

namespace Object_management.Models.FormDataFormats;

public class Form
{
    public int id { get; set; }
    public int object_type { get; set; }
    public bool in_service { get; set; }
    public bool loaned_out { get; set; }
    
    public string description { get; set; }
    public float price { get; set; }
    public List<Sale> sales { get; set; }
}