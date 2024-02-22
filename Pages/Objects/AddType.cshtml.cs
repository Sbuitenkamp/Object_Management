using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Object_management.Entity;
using Object_management.Models.FormDataFormats;
using Object_management.Repositories;

namespace Object_management.Pages.Objects;

public class AddType : PageModel
{
    [BindProperty]
    public string Description { get; set; }
    [BindProperty]
    public float Price { get; set; }

    private ObjectRepository ObjectRepo { get; set; }

    public void OnPost()
    {
        ObjectRepo = new ObjectRepository();
        ObjectType objectTypeToInsert = new ObjectType {
            description = Description,
            price = Price
        };

        int rowCount = ObjectRepo.CreateObjectType(objectTypeToInsert);
        
        if (rowCount == 0) ViewData["warning"] = "Er bestaat al een fietssoort met die beschrijving.";
        else ViewData["confirmation"] = "Fietssoort toegevoegd.";
    }
}