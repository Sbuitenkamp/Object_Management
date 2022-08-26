using Dapper;
using Object_management.Entity;
using Object_management.Models;

namespace Object_management.Repositories;

public class ObjectRepository : Repository
{
    public int CreateObject(ObjectData objectData)
    {
        int rowCount = 0;

        try {
            Connect();
            rowCount += Connection.Execute("INSERT IGNORE INTO Object (object_number, object_type_id) VALUES (@object_number, @object_type_id)", new { objectData.object_number, objectData.object_type_id });
        } catch (Exception e) {
            rowCount = -1;
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
        
        return rowCount;
    }
    
    public int CreateObjectType(ObjectType objectType)
    {
        int rowCount = 0;
        
        try {
            Connect();
            rowCount += Connection.Execute("INSERT IGNORE INTO ObjectType (description, price) VALUES (@desc, @price)", new { desc = objectType.description, objectType.price });
        } catch (Exception e) {
            rowCount = -1;
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
        
        return rowCount;
    }
    
    public List<ObjectData> SelectObjects()
    {
        string objectQuery = @"
            SELECT Object.object_number, Object.object_type_id, Object.in_service, Object.loaned_out = (
                SELECT COUNT(ReservationDate.object_number) AS amnt
                FROM ReservationDate
                WHERE ReservationDate.day = @today AND ReservationDate.object_number = Object.object_number
            ) AS loaned_out, Object.size,
            ObjectType.*, Sale.*
            FROM Object  
                LEFT JOIN ObjectType ON Object.object_type_id = ObjectType.id 
                LEFT JOIN is_applied_to ON ObjectType.id = is_applied_to.object_type_id
                LEFT JOIN Sale ON is_applied_to.sale_id = Sale.id;
            ";
        
        try {
            Connect();
            return Connection.Query<ObjectData, ObjectType, Sale, ObjectData>(objectQuery, (data, type, sale) =>
            {
                data.Type = type;
                if (sale != null) data.Type.Sales.Add(sale);
                return data;
            }, new { today = DateTime.Today }).GroupBy(objectData => objectData.object_number).Select(g => {
                // group the sales into the objectType
                ObjectData groupedData = g.First();
                if (groupedData.Type.Sales.Count == 0) return groupedData;
                
                List<Sale> listSale = g.Select(p => p.Type.Sales.Single()).ToList();
                groupedData.Type.Sales = listSale.DistinctBy(a => a.id).ToList();
            
                return groupedData;
            }).ToList();
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
    }

    public List<ObjectType> SelectObjectTypes()
    {
        try {
            Connect();
            return Connection.Query<ObjectType, Sale, ObjectType>("SELECT ObjectType.*, Sale.* FROM ObjectType LEFT JOIN is_applied_to ON ObjectType.id = is_applied_to.object_type_id LEFT JOIN Sale ON is_applied_to.sale_id = Sale.id;", (type, sale) => {
                if (sale != null) type.Sales.Add(sale);
                return type;
            }).GroupBy(objectType => objectType.id).Select(g => {
                // group the sales into the objectType
                ObjectType groupedType = g.First();
                if (groupedType.Sales.Count == 0) return groupedType;
                
                List<Sale> listSale = g.Select(p => p.Sales.Single()).ToList();
                groupedType.Sales = listSale.DistinctBy(a => a.id).ToList();
            
                return groupedType;
            }).ToList();
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
    }

    public List<Sale> SelectAllSales()
    {
        try {
            Connect();
            return Connection.Query<Sale>("SELECT Sale.id, Sale.days_to_rent, Sale.days_to_pay, is_applied_to.object_type_id FROM Sale LEFT JOIN is_applied_to ON Sale.id = is_applied_to.sale_id ORDER BY Sale.days_to_pay DESC;").ToList();
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
    }

    public int UpdateObject(List<ObjectData> objects)
    {
        int rowCount = 0;

        try {
            Connect();
            foreach (ObjectData objectData in objects) {
                string query = "UPDATE Object SET object_number=@obj_number, object_type_id=@obj_type, in_service=@service, size=@size WHERE object_number=@obj_number";

                rowCount += Connection.Execute(query, new
                {
                    obj_number = objectData.object_number,
                    obj_type = objectData.Type.id,
                    service = objectData.in_service,
                    objectData.size
                });
            }
        } catch (Exception e) {
            rowCount = -1;
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }

        return rowCount;
    }

    public int UpdateObjectType(List<ObjectType> types)
    {
        int rowCount = 0;
        try {
            Connect();
            foreach (ObjectType type in types) {
                string query = "UPDATE ObjectType SET id=@obj_type, description=@descr, price=@prce WHERE id=@obj_type";

                rowCount += Connection.Execute(query, new 
                {
                    obj_type = type.id,
                    descr = type.description,
                    prce = type.price
                });
                
                foreach (Sale sale in type.Sales) {
                    string saleQuery = sale.IsApplied ? "INSERT IGNORE INTO is_applied_to (sale_id, object_type_id) VALUES (@sale_id, @obj_type)" : "DELETE FROM is_applied_to WHERE object_type_id=@obj_type AND sale_id=@sale_id";
                    rowCount += Connection.Execute(saleQuery, new { sale_id = sale.id, obj_type = type.id });
                }
            }
        } catch (Exception e) {
            rowCount = -1;
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
        
        return rowCount;
    }

    public int DeleteObject(int id)
    {
        int rowCount = 0;

        try {
            Connect();
            rowCount += Connection.Execute("DELETE FROM Object WHERE object_number=@id", new { id });
        } catch (Exception e) {
            rowCount = -1;
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }

        return rowCount;
    }

    public int DeleteObjectType(int id)
    {
        int rowCount = 0;

        try {
            Connect();
            rowCount += Connection.Execute("DELETE FROM ObjectType WHERE id=@id", new { id });
        } catch (Exception e) {
            rowCount = -1;
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
        
        return rowCount;
    }
}