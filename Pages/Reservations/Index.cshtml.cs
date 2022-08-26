using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Object_management.Entity;
using Object_management.Models;
using Object_management.Models.FormDataFormats;
using Object_management.Repositories;

namespace Object_management.Pages.Reservations;

public class Index : PageModel
{
    private readonly ReservationRepository ReservationRepo = new ReservationRepository();
    private readonly FormValidator Validator = new FormValidator();
    public List<Reservation> ReservedReservations = new List<Reservation>();
    public List<Reservation> GivenOutReservations = new List<Reservation>();

    public string Sort = string.Empty;
    public string SortTable = string.Empty;
    public bool SortDesc = false;

    public void OnGet(string? sort, bool? desc, string? table, DateTime? day)
    {
        ReservedReservations = ReservationRepo.SelectReservedReservations();
        GivenOutReservations = ReservationRepo.SelectLoanedOutReservations(day ?? DateTime.Today);

        Sort = sort ?? string.Empty;
        SortTable = table ?? string.Empty;
        SortDesc = desc ?? false;

        if (Sort == string.Empty) return;
        if (SortTable == "reservations") {
            ReservedReservations = Sort switch {
                "reservationNumber" => SortDesc ? ReservedReservations.OrderByDescending(x => x.reservation_number).ToList() : ReservedReservations.OrderBy(x => x.reservation_number).ToList(),
                "customer" => SortDesc ? ReservedReservations.OrderByDescending(x => x.CustomerData.id).ToList() : ReservedReservations.OrderBy(x => x.CustomerData.id).ToList(),
                "paymentMethod" => SortDesc ? ReservedReservations.OrderByDescending(x => x.payment_method).ToList() : ReservedReservations.OrderBy(x => x.payment_method).ToList(),
                "startDate" => SortDesc ? ReservedReservations.OrderByDescending(x => x.start_date).ToList() : ReservedReservations.OrderBy(x => x.start_date).ToList(),
                "endDate" => SortDesc ? ReservedReservations.OrderByDescending(x => x.return_date).ToList() : ReservedReservations.OrderBy(x => x.return_date).ToList(),
                "totalPrice" => SortDesc ? ReservedReservations.OrderByDescending(x => x.Objects.Select(r => r.CalculatePrice(x.start_date, x.return_date)).Sum()).ToList() : ReservedReservations.OrderBy(x => x.Objects.Select(r => r.CalculatePrice(x.start_date, x.return_date)).Sum()).ToList(),
                "paid" => SortDesc ? ReservedReservations.OrderByDescending(x => x.paid).ToList() : ReservedReservations.OrderBy(x => x.paid).ToList(),
                _ => ReservedReservations
            };
        }

        if (table == "givenOut") {
            GivenOutReservations = Sort switch {
                "reservationNumber" => SortDesc ? GivenOutReservations.OrderByDescending(x => x.reservation_number).ToList() : GivenOutReservations.OrderBy(x => x.reservation_number).ToList(),
                "customer" => SortDesc ? GivenOutReservations.OrderByDescending(x => x.CustomerData.id).ToList() : GivenOutReservations.OrderBy(x => x.CustomerData.id).ToList(),
                "paymentMethod" => SortDesc ? GivenOutReservations.OrderByDescending(x => x.payment_method).ToList() : GivenOutReservations.OrderBy(x => x.payment_method).ToList(),
                "startDate" => SortDesc ? GivenOutReservations.OrderByDescending(x => x.start_date).ToList() : GivenOutReservations.OrderBy(x => x.start_date).ToList(),
                "endDate" => SortDesc ? GivenOutReservations.OrderByDescending(x => x.return_date).ToList() : GivenOutReservations.OrderBy(x => x.return_date).ToList(),
                "totalPrice" => SortDesc ? GivenOutReservations.OrderByDescending(x => x.Objects.Select(r => r.CalculatePrice(x.start_date, x.return_date)).Sum()).ToList() : GivenOutReservations.OrderBy(x => x.Objects.Select(r => r.CalculatePrice(x.start_date, x.return_date)).Sum()).ToList(),
                "paid" => SortDesc ? GivenOutReservations.OrderByDescending(x => x.paid).ToList() : GivenOutReservations.OrderBy(x => x.paid).ToList(),
                _ => GivenOutReservations
            };
        }
    }

    public IActionResult OnPost([FromBody] Form formData)
    {
        int result = 0;
        string validationMsg = string.Empty;

        if (formData.Reservations.Count != 0) {
            switch (formData.QueryType) {
                case "edit":
                    if (formData.Objects.Count != 0) {
                        validationMsg = Validator.ValidateObject(formData.Objects.First());
                        if (validationMsg != string.Empty) return new JsonResult(new { warning = validationMsg });
                    }
                    result = ReservationRepo.UpdateReservation(formData.Reservations);
                    return new JsonResult(Validator.GenerateResultObject(result, "Reservation"));
                case "drop":
                    if (formData.Objects.Count != 0) {
                        validationMsg = Validator.ValidateObject(formData.Objects.First());
                        if (validationMsg != string.Empty) return new JsonResult(new { warning = validationMsg });
                    }
                    // different handling depending on whether the reservation has been given out
                    result = formData.Reservations[0].Objects.Count != 0 ? ReservationRepo.DeleteReservedObject(formData.Reservations[0]) : ReservationRepo.DeleteReservation(formData.Reservations[0].reservation_number);
                    return new JsonResult(Validator.GenerateResultObject(result, "Reservation"));
            }

            return new JsonResult(new { warning = "Querytype is empty exception" });
        }

        return new JsonResult(new { warning = "Data is empty exception" });
    }
}