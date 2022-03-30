namespace Object_management.Models.FormDataFormats;

public class FormData
{
    public string Table { get; set; }
    public string Type { get; set; }
    public List<Form> Forms { get; set; }
}