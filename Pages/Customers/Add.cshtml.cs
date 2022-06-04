using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Object_management.Entity;
using Object_management.Repositories;

namespace Object_management.Pages.Customers;

[BindProperties]
public class Add : PageModel
{
    public string CustomerName { get; set; }
    public string Email { get; set; }
    public int TelePhone { get; set; }
    public string Adres { get; set; }
    public string ReturnPage { get; set; }

    private CustomerRepository CustomerRepo = new CustomerRepository();

    public void OnGet(string? returnPage)
    {
        if (returnPage != null) ReturnPage = returnPage;
    }

    public IActionResult OnPost()
    {
        Customer newCustomer = new Customer {
            name= CustomerName,
            email = Email,
            telephone = TelePhone,
            adres = Adres
        };
        int newCustomerId = CustomerRepo.Create(newCustomer);
        if (ReturnPage != null) return RedirectToPage("/" + ReturnPage, new { newCustomerId });
        return Redirect("/customers");
    }
}