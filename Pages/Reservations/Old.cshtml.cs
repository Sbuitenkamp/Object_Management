using Microsoft.AspNetCore.Mvc.RazorPages;
using Object_management.Entity;
using Object_management.Repositories;

namespace Object_management.Pages.Reservations;

public class Old : PageModel
{
    private readonly ReservationRepository ReservationRepo = new ReservationRepository();
    public List<Reservation> ReservationHistory = new List<Reservation>();
    
    public string Sort = string.Empty;
    public bool SortDesc = false;

    public void OnGet(string? sort, bool? desc, string? table, DateTime? day)
    {
        ReservationHistory = ReservationRepo.SelectOldReservations(day ?? DateTime.Today);

        Sort = sort ?? string.Empty;
        SortDesc = desc ?? false;

        if (Sort == string.Empty) return;
        ReservationHistory = Sort switch {
            "reservationNumber" => SortDesc ? ReservationHistory.OrderByDescending(x => x.reservation_number).ToList() : ReservationHistory.OrderBy(x => x.reservation_number).ToList(),
            "customer" => SortDesc ? ReservationHistory.OrderByDescending(x => x.CustomerData.id).ToList() : ReservationHistory.OrderBy(x => x.CustomerData.id).ToList(),
            "paymentMethod" => SortDesc ? ReservationHistory.OrderByDescending(x => x.payment_method).ToList() : ReservationHistory.OrderBy(x => x.payment_method).ToList(),
            "startDate" => SortDesc ? ReservationHistory.OrderByDescending(x => x.start_date).ToList() : ReservationHistory.OrderBy(x => x.start_date).ToList(),
            "endDate" => SortDesc ? ReservationHistory.OrderByDescending(x => x.return_date).ToList() : ReservationHistory.OrderBy(x => x.return_date).ToList(),
            "totalPrice" => SortDesc ? ReservationHistory.OrderByDescending(x => x.Objects.Select(r => r.CalculatePrice(x.start_date, x.return_date)).Sum()).ToList() : ReservationHistory.OrderBy(x => x.Objects.Select(r => r.CalculatePrice(x.start_date, x.return_date)).Sum()).ToList(),
            "paid" => SortDesc ? ReservationHistory.OrderByDescending(x => x.paid).ToList() : ReservationHistory.OrderBy(x => x.paid).ToList(),
            _ => ReservationHistory
        };
    }
}