using System.ComponentModel.DataAnnotations;

namespace Object_management.Entity;

public class Sale
{
    public int id { get; set; }
    public int days_to_rent { get; set; }
    public int days_to_pay { get; set; }
    public int object_type_id { get; set; }
    public bool is_applied { get; set; }
}