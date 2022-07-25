using Dapper;
using Object_management.Entity;

namespace Object_management.Repositories;

public class CustomerRepository : Repository
{
    public int Create(Customer customerData)
    {
        try {
            Connect();
            return Connection.Execute("INSERT INTO Customer (name, telephone, email, adres) VALUES (@name, @telephone, @email, @adres); SELECT LAST_INSERT_ID();", new 
            {
                customerData.name,
                customerData.telephone,
                customerData.email,
                customerData.adres
            });
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
    }
    
    public List<Customer> SelectAllCustomers(int offset)
    {
        try {
            Connect();
            return Connection.Query<Customer>("SELECT * FROM Customer LIMIT @offset, 3", new { offset }).ToList();
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
    }

    public int Update(List<Customer> customers)
    {
        int rowCount = 0;
        try {
            Connect();
            foreach (Customer customer in customers) {
                rowCount += Connection.Execute("UPDATE Customer SET name = @name, telephone = @telephone, email = @email, adres = @adres WHERE id = @id", new 
                {
                    customer.id,
                    customer.name,
                    customer.telephone,
                    customer.email,
                    customer.adres
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

    public int Delete(int customerId)
    {
        try {
            Connect();
            return Connection.Execute("DELETE FROM Customer WHERE id = @customerId", new { customerId });
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
    }
}