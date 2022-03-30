using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Object_management.Models;
using Object_management.Entity;
using Object_management.Models.FormDataFormats;

namespace Object_management.Pages.Objects;

public class Index : PageModel
{
    private DbHandler DatabaseHandler = new DbHandler();

    public List<ObjectData> ObjectList { get; private set; } = new List<ObjectData>();
    public List<ObjectType> ObjectTypeList { get; private set; } = new List<ObjectType>();
    public List<Sale> SalesList { get; private set; } = new List<Sale>();

    public void OnGet()
    {
        ObjectReturn objectReturns = DatabaseHandler.SelectObjects();
        ObjectList = objectReturns.Objects;
        ObjectTypeList = objectReturns.ObjectTypes;
        SalesList = objectReturns.Sales;
    }
    
    public void OnPost([FromBody] FormData data) {
        DbHandler Db = new DbHandler();
        int rowCount = Db.Delete(data);
        switch (data.Table.ToLower()) {
            case "objecttype":
                if (rowCount < 0) {
                    Console.WriteLine(rowCount);
                    ViewData["Warning"] = "Deze fietssoort is toegewezen aan een fiets en is mogelijk nog gereserveert!";
                }
                break;
        }

    }
}