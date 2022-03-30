using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Object_management.Models;
using Object_management.Models.FormDataFormats;

namespace Object_management.Pages.Objects;

public class AddType : PageModel
{
    [BindProperty]
    public string Description { get; set; }
    [BindProperty]
    public float Price { get; set; }

    private DbHandler Db { get; set; }

    public void OnPost()
    {
        Db = new DbHandler();
        Form form = new Form() {
            description = Description,
            price = Price
        };
        FormData data = new FormData() {
            Table = "ObjectType",
            Forms = new List<Form> { form }
        };

        if (Db.Insert(data) == 0) ViewData["warning"] = "Er bestaat al een fietssoort met die beschrijving.";
        else ViewData["confirmation"] = "Fietssoort toegevoegd.";
    }
}