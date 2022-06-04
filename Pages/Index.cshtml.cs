using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Object_management.Entity;
using Object_management.Models;
using Object_management.Repositories;

namespace Object_management.Pages;

public class Index : PageModel
{
    public int CurrentTime { get; private set; }
    public List<Reservation> ObjectsToReturn { get; private set; } = new List<Reservation>();
    public List<Reservation> ObjectsToGiveOut { get; private set; } = new List<Reservation>();
    public List<ObjectAvailability> AvailableObjects { get; private set; } = new List<ObjectAvailability>();
    
    public DateTime? SelectedDate { get; set; }

    private ReservationRepository ReservationRepo = new ReservationRepository();
    public void OnGet(DateTime? selectedDate)
    {
        CurrentTime = DateTime.Now.Hour;
        if (selectedDate != null) SelectedDate = selectedDate;
        ObjectsToReturn = ReservationRepo.SelectObjectsDue();
        ObjectsToGiveOut = ReservationRepo.SelectObjectsToLendOut();
        AvailableObjects = ReservationRepo.SelectObjectAvailabilities(SelectedDate);
    }
}