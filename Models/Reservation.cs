using Object_management.Entity;

namespace Object_management.Models;

public class Reservation
{
    // relation info
    public int reservation_number { get; set; }
    
    // customer info
    public int customer_id { get; set; }

    // object info
    public int object_number { get; set; }
    public int object_type_id { get; set; }
    public string object_type_description { get; set; }

    // pricing and payment
    public DateTime start_date { get; set; }
    public DateTime return_date { get; set; }
    public float price { get; set; }
    public string payment_method { get; set; }

    // misc reservation details
    public string comment { get; set; }
    public string residence { get; set; }

    public List<Sale> Sales;
    public List<ObjectData> Objects;
    public Customer CustomerData { get; set; }
}