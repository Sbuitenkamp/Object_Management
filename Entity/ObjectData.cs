using System.ComponentModel.DataAnnotations;

namespace Object_management.Entity;

public class ObjectData
{
    public int object_number { get; set; }
    public int object_type_id { get; set; }
    public float price { get; set; }
    public bool loaned_out { get; set; }
    public bool in_service { get; set; }
    public int days_to_rent { get; set; }
    public int days_to_pay { get; set; }
}