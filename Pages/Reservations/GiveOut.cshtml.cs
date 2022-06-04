using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Object_management.Entity;
using Object_management.Models.FormDataFormats;
using Object_management.Repositories;

namespace Object_management.Pages.Reservations;

public class GiveOut : PageModel
{
    public int ReservationNumber { get; set; }
    public Reservation CurrentReservation { get; set; }
    public List<ObjectData> AvailableObjects { get; set; }

    private ReservationRepository ReservationRepo = new ReservationRepository();
    
    public void OnGet(int? resId)
    {
        ReservationNumber = resId ?? -1;
        CurrentReservation = ReservationRepo.SelectReservation(ReservationNumber);
        AvailableObjects = ReservationRepo.SelectAvailableObjects(CurrentReservation.start_date, CurrentReservation.return_date);
    }

    public void OnPost([FromBody] Form formData)
    {
        if (formData.Reservations.Count == 0) return;
        Reservation reservation = formData.Reservations.First();
        ReservationRepo.GiveOutReservation(reservation.reservation_number, reservation.Objects);
    }
}