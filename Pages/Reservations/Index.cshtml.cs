using Microsoft.AspNetCore.Mvc.RazorPages;
using Object_management.Entity;
using Object_management.Models;

namespace Object_management.Pages.Reservations;

public class Index : PageModel
{
    public List<Reservation> Reservations;

    private DbHandler DatabaseHandler = new DbHandler();
    
    public void OnGet()
    {
        Reservations = DatabaseHandler.SelectReservations();
        Reservations.ForEach(reservation =>
        {
            reservation.Objects.Add(new ObjectData
            {
                object_number = reservation.object_number,
                object_type_id = reservation.object_type_id,
            });
        });
    }
}