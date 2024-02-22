using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Object_management.Entity;
using Object_management.Models;
using Object_management.Models.FormDataFormats;
using Object_management.Repositories;

namespace Object_management.Pages.Customers;

public class Index : PageModel
{
    private CustomerRepository CustomerRepo = new CustomerRepository();
    
    public List<Customer> Customers { get; set; } = new List<Customer>();
    public string Sort = string.Empty;
    public bool SortDesc = false;
    public void OnGet(string? sort, bool? desc)
    {
        Customers = CustomerRepo.SelectAllCustomers(0);
        
        Sort = sort ?? string.Empty;
        SortDesc = desc ?? false;

        if (Sort != string.Empty) {
            switch (Sort) {
                case "name":
                    Customers = SortDesc ? Customers.OrderByDescending(x => x.name).ToList() : Customers.OrderBy(x => x.name).ToList();
                    break;
                case "telephone":
                    Customers = SortDesc ? Customers.OrderByDescending(x => x.telephone).ToList() : Customers.OrderBy(x => x.telephone).ToList();
                    break;
                case "email":
                    Customers = SortDesc ? Customers.OrderByDescending(x => x.email).ToList() : Customers.OrderBy(x => x.email).ToList();
                    break;
                case "adres":
                    Customers = SortDesc ? Customers.OrderByDescending(x => x.adres).ToList() : Customers.OrderBy(x => x.adres).ToList();
                    break;
            }
        }
    }

    public JsonResult OnPost([FromBody] Form formData)
    {
        int result = 0;
        string validationMsg = string.Empty;
        FormValidator formValidator = new FormValidator();

        switch (formData.QueryType) {
            case "select":
                if (formData.Offset == null) return new JsonResult(new { warning = "Offset is empty exception" });
                List<Customer> res = CustomerRepo.SelectAllCustomers(formData.Offset);
                if (res.Count == 0) return new JsonResult(new { success = "Er zijn geen klanten meer om te laden" });
                return new JsonResult(new { customers = res, offset = formData.Offset + res.Count });
            case "edit":
                if (formData.Customers.Count == 0) return new JsonResult(new { warning = "Customers is empty exception" });
                validationMsg = formValidator.ValidateCustomer(formData.Customers.First());
                if (validationMsg != string.Empty) return new JsonResult(new { warning = validationMsg });

                result = CustomerRepo.Update(formData.Customers);
                return new JsonResult(formValidator.GenerateResultObject(result, "Customer"));
            case "drop":
                if (formData.Customers.Count == 0) return new JsonResult(new { warning = "Customers is empty exception" });
                result = CustomerRepo.Delete(formData.Customers[0].id);
                return new JsonResult(formValidator.GenerateResultObject(result, "Customer"));
        }
        return new JsonResult(new { warning = "Querytype is empty exception" });
    }
}