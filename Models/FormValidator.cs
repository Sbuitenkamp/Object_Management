using System.Net.Mail;
using System.Text.RegularExpressions;
using Object_management.Entity;
using Object_management.Repositories;

namespace Object_management.Models;

public class FormValidator
{
    private CustomerRepository CustomerRepo = new CustomerRepository();
    private ObjectRepository ObjectRepo = new ObjectRepository();
    private ReservationRepository ReservationRepo = new ReservationRepository();
    private SaleRepository SaleRepo = new SaleRepository();
    
    public string ValidateReservation(Reservation reservation)
    {
        string returnMessage = string.Empty;
        if (reservation.CustomerData.id == null) returnMessage = "Er is geen klant geselecteerd.";
        else if (reservation.ObjectAmounts.Count == 0) returnMessage = "Er zijn geen fietssoorten geselecteerd.";
        else if (reservation.start_date >= reservation.return_date) returnMessage = "De begindatum kan niet later dan of gelijk zijn aan de einddatum.";
        else if (reservation.residence == null) returnMessage = "Er is geen verblijflocatie opgegeven voor de klant.";
        return returnMessage;
    }

    public string ValidateObject(ObjectData obj)
    {
        string returnMessage = string.Empty;
        if (obj.object_number == null) returnMessage = "Fietsnummer is leeg.";
        else if (obj.object_type_id == null) returnMessage = "De fiets moet een soort hebben.";
        return returnMessage;
    }

    public string ValidateObjectType(ObjectType objectType)
    {
        string returnMessage = string.Empty;
        if (objectType.description == null) returnMessage = "Fietssoort moet een omschrijving hebben.";
        else if (objectType.price < 0) returnMessage = "Prijs kan niet onder de nul zijn";
        return returnMessage;
    }

    public string ValidateSale(Sale sale)
    {
        string returnMessage = string.Empty;
        if (sale.days_to_rent < sale.days_to_pay) returnMessage = "Te huren dagen kan niet kleiner zijn dan te betalen dagen.";
        else if (sale.days_to_rent == sale.days_to_pay) returnMessage = "Te huren dagen kan niet gelijk zijn aan te betalen dagen.";
        return returnMessage;
    }

    public string ValidateCustomer(Customer customer)
    {
        string returnMessage = string.Empty;
        if (customer.name == null) returnMessage = "De naam van de klant is leeg.";
        else if (customer.telephone == null) returnMessage = "De klant heeft geen telefoonnummer.";
        else if (!ValidatePhone(customer.telephone)) returnMessage = "Het telefoonnummer van de klant is niet geldig.";
        else if (customer.email == null) returnMessage = "De klant heeft geen email.";
        else if (!ValidateEmail(customer.email)) returnMessage = "De email van de klant is niet geldig.";
        return returnMessage;
    }

    private bool ValidatePhone(string phone)
    {
        const string phoneRegex = @"([+]?\d{1,3}[.\s-]?)?6?(\d{8,10})";
        return Regex.IsMatch(phone, phoneRegex);
    }

    private bool ValidateEmail(string email)
    {
        string trimmedEmail = email.Trim();
        if (trimmedEmail.EndsWith('.')) return false;
        if (!trimmedEmail.Contains('@')) return false; // no @? no mail.
        if (!trimmedEmail.Contains('.')) return false;
        if (trimmedEmail.Split('.').Last().Contains('@')) return false; // ex.ample@mail == false
        
        return true;
    }

    public object GenerateResultObject(int result, string table)
    {
        object returnData;
        bool success;
        string resultMessage = string.Empty;
        
        switch (result) {
            case < 0:
                resultMessage = "Kon gegevens niet bijwerken.";
                success = false;
                break;
            case 0:
                resultMessage = "Er zijn geen gegevens aangepast.";
                success = false;
                break;
            default:
                resultMessage = "Gegevens successvol aangepast.";
                success = true;
                break;
        }

        switch (table) {
            case "Object":
                List<ObjectData> data = ObjectRepo.SelectObjects();
                if (result <= 0) return new { warning = resultMessage, body = new { table, data } };
                return new { success = resultMessage, body = new { table, data } };
            default:
                return success ? new { success = resultMessage } : new { warning = resultMessage };
        }
    }
}