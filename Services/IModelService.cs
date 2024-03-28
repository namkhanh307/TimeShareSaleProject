using TimeShareProject.Models;

namespace TimeShareProject.Services
{
    public interface IModelService
    {
        Task<List<Reservation>> GetFinishedReservation();

        Task<List<List<Reservation>>> GetGroupReservationsAsync();

        Task<List<Reservation>> GetBuyNowReservation();

        DateTime GetDeadlineReserveDate(int id);

        DateTime GetDeadlineDepositDate(int id);

        DateTime GetDeadlineFirstDate(int id);

        DateTime GetDeadlineSecondDate(int id);

        DateTime GetDeadlineThirdDate(int id);

    }
}
