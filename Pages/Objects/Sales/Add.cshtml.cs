using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Object_management.Entity;
using Object_management.Repositories;

namespace Object_management.Pages.Objects.Sales;

public class Add : PageModel
{
    private SaleRepository SaleRepo = new SaleRepository();
    
    [BindProperty]
    public int DaysToRent { get; set; }
    [BindProperty]
    public int DaysToPay { get; set; }

    public IActionResult OnPost()
    {
        Sale saleData = new Sale { days_to_rent = DaysToRent, days_to_pay = DaysToPay };
        int rowCount = SaleRepo.AddSale(saleData);        

        return Redirect("/objects/sales/add");
    }
}