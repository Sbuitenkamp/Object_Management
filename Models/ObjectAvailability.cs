using Object_management.Entity;

namespace Object_management.Models;

public class ObjectAvailability
{
    public ObjectType AvailableObjectType { get; set; }
    public int MondayAvailable { get; set; }
    public int TuesdayAvailable { get; set; }
    public int WednesdayAvailable { get; set; }
    public int ThursdayAvailable { get; set; }
    public int FridayAvailable { get; set; }
    public int SaturdayAvailable { get; set; }
    public int SundayAvailable { get; set; }
}