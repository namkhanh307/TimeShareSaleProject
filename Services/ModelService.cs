using Microsoft.EntityFrameworkCore;
using TimeShareProject.Models;

namespace TimeShareProject.Services
{
    public class ModelService : IModelService
    {

        _4restContext _context;

        public async Task<List<Reservation>> GetFinishedReservation()
        {
            _context = new();
            var reservation = await _context.Reservations.Include(r => r.Transactions).ToListAsync();
            return reservation;
        }

        public async Task<List<Reservation>> GetBuyNowReservation()
        {
            _context = new();
            var reservation = await _context.Reservations.Include(r => r.Transactions).Where(r => r.Type == 2).ToListAsync();
            return reservation;
        }

        public async Task<List<List<Reservation>>> GetGroupReservationsAsync()
        {
            _context = new();
            var distinctReservations = await _context.Reservations
                .Include(r => r.Property)
                .Include(r => r.Block)
                .Where(r => r.Type == 1)
                .ToListAsync();

            var groupedReservations = distinctReservations
                .GroupBy(r => new { r.PropertyId, r.BlockId })
                .Select(group => group.ToList())
                .ToList();

            return groupedReservations;
        }
        public DateTime GetDeadlineReserveDate(int id)
        {
            using (_4restContext context = new _4restContext())
            {
                var transaction = context.Transactions
                    .FirstOrDefault(t => t.ReservationId == id && t.Type == -1);

                if (transaction != null && transaction.DeadlineDate != null)
                {
                    return transaction.DeadlineDate.Value;
                }
                else
                {
                    // Handle the case when transaction or DeadlineDate is null (optional)
                    throw new InvalidOperationException("Transaction or DeadlineDate is null.");
                }
            }
        }

        public DateTime GetDeadlineDepositDate(int id)
        {
            using (_4restContext context = new _4restContext())
            {
                var transaction = context.Transactions
                    .FirstOrDefault(t => t.ReservationId == id && t.Type == 0);

                if (transaction != null && transaction.DeadlineDate != null)
                {
                    return transaction.DeadlineDate.Value;
                }
                else
                {
                    // Handle the case when transaction or DeadlineDate is null (optional)
                    throw new InvalidOperationException("Transaction or DeadlineDate is null.");
                }
            }
        }

        //public DateTime GetDeadlineDepositDate(int id)
        //{
        //    using _4restContext context = new();
        //    // var property = context.Properties.Include(p => p.Reservations).Where(p => p.Id == )
        //    var resrvation = context.Reservations.FirstOrDefault(r => r.Id == id);
        //    var property = context.Properties.FirstOrDefault(p => p.Id == resrvation.PropertyId);
        //    return (DateTime)property.SaleDate.AddDays(resrvation.Order -1);
        //}

        public DateTime GetDeadlineFirstDate(int id)
        {
            using _4restContext context = new();
            var transaction = context.Transactions
             .FirstOrDefault(t => t.ReservationId == id && t.Type == 1);
            return (DateTime)transaction.DeadlineDate;
        }

        public DateTime GetDeadlineSecondDate(int id)
        {
            using _4restContext context = new();
            var transaction = context.Transactions
             .FirstOrDefault(t => t.ReservationId == id && t.Type == 2);
            return (DateTime)transaction.DeadlineDate;
        }

        public DateTime GetDeadlineThirdDate(int id)
        {
            using _4restContext context = new();
            var transaction = context.Transactions
             .FirstOrDefault(t => t.ReservationId == id && t.Type == 3);
            return (DateTime)transaction.DeadlineDate;
        }
    }
}
