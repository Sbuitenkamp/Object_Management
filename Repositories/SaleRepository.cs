using Dapper;
using Object_management.Entity;

namespace Object_management.Repositories;

public class SaleRepository : Repository
{
    public int AddSale(Sale saleToAdd)
    {
        int rowCount = 0;
        try {
            Connect();
            rowCount += Connection.Execute("INSERT INTO Sale (days_to_rent, days_to_pay) VALUES (@days_to_rent, @days_to_pay);", new { saleToAdd.days_to_rent, saleToAdd.days_to_pay });
        } catch (Exception e) {
            rowCount = -1;
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }

        return rowCount;
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
            foreach (Sale sale in salesToUpdate) rowCount += Connection.Execute("UPDATE Sale SET Sale.days_to_pay = @days_to_pay, Sale.days_to_rent = @days_to_rent WHERE Sale.id = @id;", new { sale.id, sale.days_to_pay, sale.days_to_rent });
        } catch (Exception e) {
            rowCount = -1;
            Console.WriteLine(e);
            throw;
        } finally {
            CloseConnection();
        }
        
        return rowCount;
    }

    public int DeleteSale(Sale saleToDelete)
    {
        int rowCount = 0;
        try {
            Connect();
            rowCount += Connection.Execute(@"
                DELETE FROM is_applied_to WHERE sale_id = @id; 
                DELETE FROM Sale WHERE Sale.id = @id;
            ", new { saleToDelete.id });
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