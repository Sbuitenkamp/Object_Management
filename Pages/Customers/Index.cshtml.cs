using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Object_management.Entity;
using Object_management.Models;
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

    public JsonResult OnPost([FromBody] Form formData)
    {
        if (formData.Customers.Count != 0) {
            string resultMessage = string.Empty;
            int result = 0;
            object returnData;
            switch (formData.QueryType) {
                case "edit":
                    string validationMsg = FormValidator.ValidateCustomer(formData.Customers.First());
                    if (validationMsg != string.Empty) return new JsonResult(new { warning = validationMsg });

                    result = CustomerRepo.Update(formData.Customers);
                    switch (result) {
                        case < 0:
                            resultMessage = "Er is een fout opgetreden tijdens het bijwerken.";
                            break;
                        case 0:
                            resultMessage = "Geen rijen bijgewerkt.";
                            break;
                        default:
                            resultMessage = $"{result} rij(en) bijgewerkt.";
                            break;
                    }

                    if (result <= 0) returnData = new { warning = resultMessage };
                    else returnData = new { success = resultMessage };

                    return new JsonResult(returnData);
                case "drop":
                    result = CustomerRepo.Delete(formData.Customers[0].id);
                    switch (result) {
                        case < 0:
                            resultMessage = "Er is een fout opgetreden tijdens het bijwerken.";
                            break;
                        case 0:
                            resultMessage = "Geen rijen bijgewerkt.";
                            break;
                        default:
                            resultMessage = $"{result} rij(en) bijgewerkt.";
                            break;
                    }

                    if (result <= 0) returnData = new { warning = resultMessage };
                    else returnData = new { success = resultMessage };

                    return new JsonResult(returnData);
            }
        }
        return new JsonResult(new { warning = "Customers is empty exception" });
    }
}