using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Object_management.Models;
using Object_management.Models.FormDataFormats;

namespace Object_management.Pages;

public class HandlePost : PageModel
{
    private DbHandler DatabaseHandler = new DbHandler();

    public void OnPost([FromBody] FormData data)
    {
        switch (data.Type.ToLower()) {
            case "edit":
                DatabaseHandler.Update(data);
                break;
            case "fetch":
                DatabaseHandler.Select(data);
                break;
            case "drop":
                DatabaseHandler.Delete(data);
                break;
        }
    }
}