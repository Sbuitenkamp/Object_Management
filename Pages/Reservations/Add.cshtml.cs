using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Object_management.Entity;
using Object_management.Models.FormDataFormats;
using Object_management.Repositories;

namespace Object_management.Pages.Reservations;
[BindProperties]
public class Add : PageModel
{
    public int CustomerId { get; set; }
    public string PaymentMethod { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime ReturnDate { get; set; }
    public string Residence { get; set; }
    public string Comment { get; set; }
    public Reservation NewReservation { get; set; }
    public List<ObjectAmount> ObjectTypeAmounts { get; set; }
    public List<Customer> Customers { get; set; }
    public List<ObjectType> ObjectTypes { get; set; }
    
    public int NewCustomerId { get; set; }

    private ReservationRepository ReservationRepo = new ReservationRepository();
    private CustomerRepository CustomerRepo = new CustomerRepository();
    private ObjectRepository ObjectRepo = new ObjectRepository();

    public void OnGet(int? newId, int? newCustomerId)
    {
        Customers = CustomerRepo.SelectAllCustomers();
        ObjectTypes = ObjectRepo.SelectObjectTypes();

        if (newId != null) NewReservation = ReservationRepo.SelectReservation((int) newId);
        if (newCustomerId != null) NewCustomerId = (int) newCustomerId;
    }

    public IActionResult OnPost()
    {
        Reservation reservation = new Reservation {
            CustomerData = new Customer { id = CustomerId },
            ObjectAmounts = ObjectTypeAmounts.Where(x => x.amount != 0).ToList(),
            payment_method = PaymentMethod,
            start_date = StartDate,
            return_date = ReturnDate,
            residence = Residence,
            comment = Comment
        };

        // get the objects that are available at the date
        // todo: add this to give out
        // List<ObjectData> res = ReservationRepo.SelectAvailableObjects(StartDate, ReturnDate, ObjectTypeAmounts);
        // reservation.Objects.AddRange(res);

        int newId = ReservationRepo.CreateReservation(reservation);

        return RedirectToPage(new { newId });
    }
}