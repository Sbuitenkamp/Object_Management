using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Object_management.Models;
using Object_management.Entity;
using Object_management.Models.FormDataFormats;
using Object_management.Repositories;

namespace Object_management.Pages.Objects;

public class Index : PageModel
{
    private readonly ObjectRepository ObjectRepo = new ObjectRepository();

    public List<ObjectData> ObjectList { get; private set; } = new List<ObjectData>();
    public List<ObjectType> ObjectTypeList { get; private set; } = new List<ObjectType>();
    public List<Sale> SalesList { get; private set; } = new List<Sale>();

    public string Sort = string.Empty;
    public bool SortDesc = false;
    
    public string Warning = nameof(Warning);
    public string Success = nameof(Success);

    public void OnGet(string? sort, bool? desc)
    {
        ObjectList = ObjectRepo.SelectObjects();
        ObjectTypeList = ObjectRepo.SelectObjectTypes();
        SalesList = ObjectRepo.SelectAllSales();

        Sort = sort ?? string.Empty;
        SortDesc = desc ?? false;

        if (Sort != string.Empty) {
            ObjectList = Sort switch {
                "objectNumber" => SortDesc ? ObjectList.OrderByDescending(x => x.object_number).ToList() : ObjectList.OrderBy(x => x.object_number).ToList(),
                "objectType" => SortDesc ? ObjectList.OrderByDescending(x => x.object_type_id).ToList() : ObjectList.OrderBy(x => x.object_type_id).ToList(),
                "price" => SortDesc ? ObjectList.OrderByDescending(x => x.Type.price).ToList() : ObjectList.OrderBy(x => x.Type.price).ToList(),
                "isLoanedOut" => SortDesc ? ObjectList.OrderByDescending(x => x.loaned_out).ToList() : ObjectList.OrderBy(x => x.loaned_out).ToList(),
                "inService" => SortDesc ? ObjectList.OrderByDescending(x => x.in_service).ToList() : ObjectList.OrderBy(x => x.in_service).ToList(),
                "size" => SortDesc ? ObjectList.OrderByDescending(x => x.size).ToList() : ObjectList.OrderBy(x => x.size).ToList(),
                "sales" => SortDesc ? ObjectList.OrderByDescending(x => x.Type.Sales.Count != 0 ? x.Type.Sales.First().days_to_pay : 0).ToList() : ObjectList.OrderBy(x => x.Type.Sales.Count != 0 ? x.Type.Sales.First().days_to_pay : 0).ToList(),
                _ => ObjectList
            };
        }
    }
    
    public IActionResult OnPost([FromBody] Form formData)
    {
        if (formData.ObjectTypes.Count != 0) {
            int result = 0;
            string validationMsg = string.Empty;

            switch (formData.QueryType) {
                case "edit":
                    validationMsg = FormValidator.ValidateObjectType(formData.ObjectTypes.First());
                    if (validationMsg != string.Empty) return new JsonResult(new { warning = validationMsg });

                    result = ObjectRepo.UpdateObjectType(formData.ObjectTypes);
                    return new JsonResult(FormValidator.GenerateResultObject(result));
                case "drop":
                    validationMsg = FormValidator.ValidateObjectType(formData.ObjectTypes.First());
                    if (validationMsg != string.Empty) return new JsonResult(new { warning = validationMsg });
                    
                    result = ObjectRepo.DeleteObjectType(formData.ObjectTypes[0].id);
                    return new JsonResult(FormValidator.GenerateResultObject(result));
            }

            return new JsonResult(new { warning = "Querytype is empty exception" });
        }

        if (formData.Objects.Count != 0) {
            int result = 0;
            string validationMsg = string.Empty;
                
            switch (formData.QueryType) {
                case "edit":
                    validationMsg = FormValidator.ValidateObject(formData.Objects.First());
                    if (validationMsg != string.Empty) return new JsonResult(new { warning = validationMsg });

                    result = ObjectRepo.UpdateObject(formData.Objects);
                    return new JsonResult(FormValidator.GenerateResultObject(result));
                case "drop":
                    validationMsg = FormValidator.ValidateObject(formData.Objects.First());
                    if (validationMsg != string.Empty) return new JsonResult(new { warning = validationMsg });

                    result = ObjectRepo.DeleteObject(formData.Objects[0].object_number);
                    return new JsonResult(FormValidator.GenerateResultObject(result));
            }
            return new JsonResult(new { warning = "Querytype is empty exception" });
        }

        return new JsonResult(new { warning = "Data is empty exception" });
    }
}