using Object_management.Entity;

namespace Object_management.Models;

public class ObjectReturn
{
    public List<ObjectData> Objects { get; private set; }
    public List<ObjectType> ObjectTypes { get; private set; }
    public List<Sale> Sales { get; private set; }

    public ObjectReturn(List<ObjectData> objects, List<ObjectType> objectTypes, List<Sale> sales)
    {
        Objects = objects;
        ObjectTypes = objectTypes;
        Sales = sales;
    }
}