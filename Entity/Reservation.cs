using Object_management.Models.FormDataFormats;

namespace Object_management.Entity;

public class Reservation
{
    public int reservation_number { get; set; }
    
    // pricing and payment
    public DateTime start_date { get; set; }
    public DateTime return_date { get; set; }
    public string payment_method { get; set; }
    public bool paid { get; set; }

    // misc reservation details
    public string residence { get; set; }
    public string comment { get; set; }

    // relation info
    public List<ObjectData> Objects { get; set; } = new List<ObjectData>();
    public List<ObjectAmount> ObjectAmounts { get; set; } = new List<ObjectAmount>();
    public Customer CustomerData { get; set; }
}