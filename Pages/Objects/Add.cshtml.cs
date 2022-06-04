using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Object_management.Entity;
using Object_management.Repositories;

namespace Object_management.Pages.Objects;

public class Add : PageModel
{
    [BindProperty]
    public int ObjectNumber { get; set; }
    [BindProperty]
    public int ObjectTypeId { get; set; }
    [BindProperty]
    public string Size { get; set; }

    public List<ObjectType> ObjectTypes { get; set; }

    private ObjectRepository ObjectRepo = new ObjectRepository();
    public void OnGet()
    {
        ObjectTypes = ObjectRepo.SelectObjectTypes();
    }

    public IActionResult OnPost()
    {
        ObjectData objectData = new ObjectData { object_number = ObjectNumber, object_type_id = ObjectTypeId, size = Size};
        int rowCount = ObjectRepo.CreateObject(objectData);

        return Redirect("/objects/add");
    }
}