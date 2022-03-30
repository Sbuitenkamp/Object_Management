using System.ComponentModel.DataAnnotations;

namespace Object_management.Entity;

public class ObjectType
{
    public int id { get; set; }
    public float price { get; set; }
    public string description { get; set; }
}