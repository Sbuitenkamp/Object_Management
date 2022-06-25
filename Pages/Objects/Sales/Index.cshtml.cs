using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Object_management.Entity;
using Object_management.Models.FormDataFormats;
using Object_management.Repositories;

namespace Object_management.Pages.Objects.Sales;

public class Index : PageModel
{
    private SaleRepository SaleRepo = new SaleRepository();
    
    public List<Sale> SalesList { get; private set; } = new List<Sale>();
    
    public void OnGet()
    {
        SalesList = SaleRepo.GetSales();
    }

    public void OnPost([FromBody] Form formData)
    {
        if (formData.Sales.Count == 0) return;
        switch (formData.QueryType) {
            case "edit":
                SaleRepo.UpdateSales(formData.Sales);
                break;
            case "drop":
                SaleRepo.DeleteSale(formData.Sales.First());
                break;
        }
    }
}