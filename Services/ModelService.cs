using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
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
            using _4restContext context = new();
            var transaction = context.Transactions
             .FirstOrDefault(t => t.ReservationId == id && t.Type == -1);
            return (DateTime)transaction.DeadlineDate;
        }

        public DateTime GetDeadlineDepositDate(int id)
        {
            using _4restContext context = new();
            var transaction = context.Transactions
             .FirstOrDefault(t => t.ReservationId == id && t.Type == 0);
            return (DateTime)transaction.DeadlineDate;
        }

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
