using Dapper;
using Object_management.Entity;

namespace Object_management.Repositories;

public class CustomerRepository: Repository
{
    public int Create(Customer customerData)
    {
        try {
            Connect();
            return Connection.QuerySingle<int>("INSERT INTO Customer (name, telephone, email, adres) VALUES (@name, @telephone, @email, @adres); SELECT LAST_INSERT_ID();", new 
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
    
    public List<Customer> SelectAllCustomers()
    {
        try {
            Connect();
            return Connection.Query<Customer>("SELECT * FROM Customer").ToList();
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
                rowCount += Connection.ExecuteScalar<int>("UPDATE Customer SET name = @name, telephone = @telephone, email = @email, adres = @adres WHERE id = @id", new 
                {
                    customer.id,
                    customer.name,
                    customer.telephone,
                    customer.email,
                    customer.adres
                });
            }
        } catch (Exception e) {
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
            return Connection.ExecuteScalar<int>("DELETE FROM Customer WHERE id = @customerId", new { customerId });
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
    }
}