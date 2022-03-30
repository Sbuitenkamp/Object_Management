using Dapper;
using MySql.Data.MySqlClient;
using Object_management.Entity;
using Object_management.Models.FormDataFormats;

namespace Object_management.Models;

public class DbHandler
{
    private MySqlConnection Connection;
    
    private string DbName;
    private string DbHost;
    private string DbUser;
    private string DbPassword;

    public DbHandler()
    {
        DbName = "Object_management";
        DbHost = "127.0.0.1";
        DbUser = "Sbuitenkamp";
        DbPassword = "pass123";
    }

    private void Connect()
    {
        string connectionString = "SERVER=" + DbHost + ";DATABASE=" + DbName + ";USER=" + DbUser + ";PASSWORD=" + DbPassword + ";SSL Mode=None;Allow User Variables=True;";
        Connection = new MySqlConnection(connectionString);
        Connection.Open();
    }

    private void CloseConnection()
    {
        Connection.Close();
    }

    public ObjectReturn SelectObjects()
    {
        List<ObjectData> objects;
        List<ObjectType> objectTypes;
        List<Sale> sales;
        string objectQuery = "SELECT Object.object_number, Object.loaned_out, Object.in_service, Object.object_type_id, ObjectType.price FROM Object LEFT JOIN ObjectType ON Object.object_type_id = ObjectType.id LEFT JOIN is_applied_to ON ObjectType.id = is_applied_to.object_type_id GROUP BY Object.object_number;";
        string typeQuery = "SELECT id, description, price FROM ObjectType;";
        string saleQuery = "SELECT Sale.id, Sale.days_to_rent, Sale.days_to_pay, is_applied_to.object_type_id FROM Sale LEFT JOIN is_applied_to ON Sale.id = is_applied_to.sale_id ORDER BY Sale.days_to_pay DESC;";
        
        try {
            Connect();
            objects = Connection.Query<ObjectData>(objectQuery).ToList();
            objectTypes = Connection.Query<ObjectType>(typeQuery).ToList();
            sales = Connection.Query<Sale>(saleQuery).ToList();
            return new ObjectReturn(objects, objectTypes, sales);
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
    }

    public List<Reservation> SelectReservations()
    {
        try {
            Connect();
            string reservationQuery = "SELECT `Reservation`.*, Customer.name, Customer.telephone, Customer.email, Customer.adres, Object.object_number, ObjectType.id AS object_type_id, ObjectType.description AS object_type_decription, ObjectType.price, Sale.days_to_rent, Sale.days_to_pay FROM `Reservation` INNER JOIN Customer ON Reservation.customer_id = Customer.id INNER JOIN is_reserved_at ON Reservation.reservation_number = is_reserved_at.reservation_number INNER JOIN Object ON is_reserved_at.object_number = Object.object_number INNER JOIN ObjectType ON Object.object_type_id = ObjectType.id LEFT JOIN is_applied_to ON ObjectType.id = is_applied_to.object_type_id LEFT JOIN Sale ON is_applied_to.sale_id = Sale.id;";
            // return Connection.Query<Reservation, Customer, ObjectData, Sale>(reservationQuery, (reservation, customer, objectData, sale) =>
            // {
            //     reservation.CustomerData = customer;
            //     reservation.Objects = new List<ObjectData> { objectData };
            //     reservation.Sales = new List<Sale> { sale };
            //     return reservation;
            // }).ToList();
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }

        return null;
    }

    public int Insert(FormData data)
    {
        int rowCount = 0;
        try {
            Connect();
            MySqlCommand insertQuery = Connection.CreateCommand();
            Form form = data.Forms[0];
            switch (data.Table.ToLower()) {
                case "objecttype":
                    insertQuery.CommandText = "INSERT IGNORE INTO ObjectType (description, price) VALUES (@desc, @price)";
                    insertQuery.Parameters.AddWithValue("@desc", form.description);
                    insertQuery.Parameters.AddWithValue("@price", form.price);
                    break;
            }

            rowCount += insertQuery.ExecuteNonQuery();
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }

        return rowCount;
    }

    public void Select(FormData data)
    {
        switch (data.Table) {
            case "ObjectType":
                break;
        }
    }

    public int Update(FormData data)
    {
        int rowCount = 0;
        try {
            Connect();
            foreach (var form in data.Forms) {
                if (form is not Form) continue;
                MySqlCommand updateQuery = Connection.CreateCommand();
                switch (data.Table.ToLower()) {
                    case "object":
                        updateQuery.CommandText = "UPDATE Object SET object_type_id=@obj_type, in_service=@in_srvc, loaned_out=@lnd_out WHERE object_number=@obj_nr";
                        updateQuery.Parameters.AddWithValue("@obj_nr", form.id);
                        updateQuery.Parameters.AddWithValue("@obj_type", form.object_type);
                        updateQuery.Parameters.AddWithValue("@in_srvc", form.in_service);
                        updateQuery.Parameters.AddWithValue("@lnd_out", form.loaned_out);
                    break;
                    case "objecttype":
                        updateQuery.CommandText = "UPDATE ObjectType SET id=@obj_type, description=@descr, price=@prce WHERE id=@obj_type";
                        updateQuery.Parameters.AddWithValue("@obj_type", form.id);
                        updateQuery.Parameters.AddWithValue("@descr", form.description);
                        updateQuery.Parameters.AddWithValue("@prce", form.price);
                        foreach (Sale sale in form.sales) {
                            MySqlCommand saleQuery = Connection.CreateCommand();
                            if (sale.is_applied) {
                                // TODO fix this syntax
                                List<Sale> sales = Connection.Query<Sale>("SELECT object_type_id, sale_id AS id FROM is_applied_to WHERE object_type_id =" + sale.object_type_id + " AND sale_id=" + sale.id).ToList();
                                if (sales.Count != 0) continue;
                                saleQuery.CommandText = "INSERT INTO is_applied_to (sale_id, object_type_id) VALUES (@sale_id, @obj_type)";
                            } else {
                                saleQuery.CommandText = "DELETE FROM is_applied_to WHERE object_type_id=@obj_type AND sale_id=@sale_id";
                            }
                            Console.WriteLine(sale.object_type_id);
                            saleQuery.Parameters.AddWithValue("@sale_id", sale.id);
                            saleQuery.Parameters.AddWithValue("@obj_type", sale.object_type_id);
                            rowCount += saleQuery.ExecuteNonQuery();
                        }
                        break;
                }
                rowCount += updateQuery.ExecuteNonQuery();
            }
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
        return rowCount;
    }

    public int Delete(FormData data)
    {
        int rowCount = 0;
        try {
            Connect();
            MySqlCommand deleteCommand = Connection.CreateCommand();
            Form form = data.Forms[0];
            
            switch (data.Table.ToLower()) {
                case "objecttype":
                    // check if objtype isn't reserved
                    MySqlCommand checkReserved = Connection.CreateCommand();
                    checkReserved.CommandText = "SELECT * FROM Object INNER JOIN is_reserved_at ON Object.object_number = is_reserved_at.object_number WHERE Object.object_type_id = @id;";
                    checkReserved.Parameters.AddWithValue("@id", form.id);
                    if (checkReserved.ExecuteScalar() != null) return -1;

                    // set query
                    deleteCommand.CommandText = "DELETE FROM ObjectType WHERE ObjectType.id = @id";
                    deleteCommand.Parameters.AddWithValue("@id", form.id);

                    // delete from the sales
                    MySqlCommand deleteSales = Connection.CreateCommand();
                    deleteSales.CommandText = "DELETE FROM is_applied_to WHERE object_type_id = @id";
                    deleteSales.Parameters.AddWithValue("@id", form.id);

                    rowCount += deleteSales.ExecuteNonQuery();

                    break;
            }
            rowCount += deleteCommand.ExecuteNonQuery();
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
        return rowCount;
    }
}