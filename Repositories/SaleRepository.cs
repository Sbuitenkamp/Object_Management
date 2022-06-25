using Dapper;
using Object_management.Entity;

namespace Object_management.Repositories;

public class SaleRepository : Repository
{
    public int AddSale(Sale saleToAdd)
    {
        try {
            Connect();
            return Connection.ExecuteScalar<int>("INSERT INTO Sale (days_to_rent, days_to_pay) VALUES (@days_to_rent, @days_to_pay);", new { saleToAdd.days_to_rent, saleToAdd.days_to_pay });
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
    }
    
    public List<Sale> GetSales()
    {
        try {
            Connect();
            return Connection.Query<Sale>("SELECT * FROM Sale;").ToList();
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
    }

    public int UpdateSales(List<Sale> salesToUpdate)
    {
        int rowCount = 0;
        try {
            Connect();
            foreach (Sale sale in salesToUpdate) rowCount += Connection.ExecuteScalar<int>("UPDATE Sale SET Sale.days_to_pay = @days_to_pay, Sale.days_to_rent = @days_to_rent WHERE Sale.id = @id;", new { sale.id, sale.days_to_pay, sale.days_to_rent });
            return rowCount;
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
    }

    public int DeleteSale(Sale saleToDelete)
    {
        int rowCount = 0;
        try {
            Connect();
            rowCount += Connection.ExecuteScalar<int>(@"
                DELETE FROM is_applied_to WHERE sale_id = @id; 
                DELETE FROM Sale WHERE Sale.id = @id;
            ", new { saleToDelete.id });
            return rowCount;
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
    }
}