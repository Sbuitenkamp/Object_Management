using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Object_management.Entity;
using Object_management.Models;
using Object_management.Repositories;

namespace Object_management.Pages.Customers;

[BindProperties]
public class Add : PageModel
{
    public string CustomerName { get; set; }
    public string Email { get; set; }
    public string TelePhone { get; set; }
    public string Adres { get; set; }
    public string ReturnPage { get; set; }
    public string Warning { get; set; } = string.Empty;

    private CustomerRepository CustomerRepo = new CustomerRepository();

    public void OnGet(string? returnPage, string? warning)
    {
        if (returnPage != null) ReturnPage = returnPage;
        if (warning != null) Warning = warning;
    }

    public IActionResult OnPost()
    {
        Customer newCustomer = new Customer {
            name = CustomerName,
            email = Email,
            telephone = TelePhone,
            adres = Adres
        };
        return new JsonResult(new {});

        // string validationMsg = FormValidator.ValidateCustomer(newCustomer);
        // if (validationMsg != string.Empty) return RedirectToPage(new { warning = validationMsg });
        
        int newCustomerId = CustomerRepo.Create(newCustomer);
        if (ReturnPage != null) return RedirectToPage("/" + ReturnPage, new { newCustomerId });
        return Redirect("/customers");

    }
}