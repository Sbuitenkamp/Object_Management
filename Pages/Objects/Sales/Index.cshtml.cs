using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Object_management.Entity;
using Object_management.Models;
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

    public JsonResult OnPost([FromBody] Form formData)
    {
        if (formData.Sales.Count == 0) return new JsonResult(new { warning = "Sales is empty exception" });
        int result = 0;
        string validationMsg = string.Empty;
        FormValidator formValidator = new FormValidator();

        switch (formData.QueryType) {
            case "edit":
                validationMsg = formValidator.ValidateSale(formData.Sales.First());
                if (validationMsg != string.Empty) return new JsonResult(new { warning = validationMsg });
                
                result = SaleRepo.UpdateSales(formData.Sales);
                return new JsonResult(formValidator.GenerateResultObject(result, "Sale"));
            case "drop":
                result = SaleRepo.DeleteSale(formData.Sales.First());
                return new JsonResult(formValidator.GenerateResultObject(result, "Sale"));
        }
        return new JsonResult(new { warning = "Querytype is empty exception" });
    }
}