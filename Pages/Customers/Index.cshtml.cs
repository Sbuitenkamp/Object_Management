using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Object_management.Entity;
using Object_management.Models.FormDataFormats;
using Object_management.Repositories;

namespace Object_management.Pages.Customers;

public class Index : PageModel
{
    public List<Customer> Customers { get; set; } = new List<Customer>();
    private CustomerRepository CustomerRepo = new CustomerRepository();
    
    public string Sort = string.Empty;
    public bool SortDesc = false;
    public void OnGet(string? sort, bool? desc)
    {
        Customers = CustomerRepo.SelectAllCustomers();
        
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

    public void OnPost([FromBody] Form formData)
    {
        if (formData.Customers.Count != 0) {
            switch (formData.QueryType) {
                case "edit":
                    CustomerRepo.Update(formData.Customers);
                    break;
                case "drop":
                    CustomerRepo.Delete(formData.Customers[0].id);
                    break;
            }
        }
    }
}