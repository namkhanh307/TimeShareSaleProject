using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System.Collections.Generic;
using TimeShareProject.Controllers;
using TimeShareProject.Models;
using TimeShareProject.Services;

public class DateCheckerService : BackgroundService
{
    //private readonly IServiceProvider _serviceProvider;
    //private readonly ILogger<DateCheckerService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public DateCheckerService(IServiceScopeFactory scopeFactory) //IServiceProvider serviceProvider, 
    {
        //_serviceProvider = serviceProvider;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine("Check");
            await Console.Out.WriteLineAsync(DateTime.Today.ToString());
            using (var scope = _scopeFactory.CreateScope())
            {
                var scopedServiceProvider = scope.ServiceProvider;
                var context = scopedServiceProvider.GetRequiredService<_4restContext>();
                var scopeService = scopedServiceProvider.GetRequiredService<IModelService>();

                #region Handle Reserve
                var groupReservations = await scopeService.GetGroupReservationsAsync(); // list list group reservation

                foreach (var group in groupReservations)
                {
                    if (group.Count >= 1)
                    {
                        var sortedReservations = group.Where(r => r.Order > 0).OrderBy(r => r.Order).ToList();

                        for (int i = 0; i < sortedReservations.Count; i++)
                        {
                            var reservation = sortedReservations[i];

                            if (reservation != null)
                            {
                                // Check reservation fee payment deadline
                                if (scopeService.GetDeadlineReserveDate(reservation.Id) <= DateTime.Today)
                                {
                                    // Check if the user paid the reservation fee
                                    var reserveTransaction = context.Transactions.FirstOrDefault(t => t.ReservationId == reservation.Id && t.Type == -1 && t.Status == true);
                                    if (reserveTransaction == null)
                                    {


                                        reservation.Status = 2; // cancel
                                        reservation.Order = 0;
                                        context.Update(reservation);

                                        context.SaveChanges();

                                    }
                                }


                                if (scopeService.GetDeadlineDepositDate(reservation.Id) == DateTime.Today)
                                {
                                    // Check if the user paid the deposit fee
                                    var depositTransaction = context.Transactions.FirstOrDefault(t => t.ReservationId == reservation.Id && t.Type == 0 && t.Status == true);
                                    if (depositTransaction != null)
                                    {

                                        for (int j = i + 1; j < sortedReservations.Count; j++)
                                        {
                                            sortedReservations[j].Order = 0; // Cancelled
                                            sortedReservations[j].Status = 2;// Cancelled
                                            context.Update(sortedReservations[j]);
                                            context.SaveChanges();
                                        }


                                        break;
                                    }
                                    else
                                    {
                                        reservation.Status = 2;
                                        reservation.Order = 0;
                                        context.Update(reservation);
                                        context.SaveChanges();
                                    }
                                }
                            }
                            if (sortedReservations[i].Transactions.Any(r => r.Type == 0 && r.Status == true))
                            {
                                for (int j = i + 1; i < sortedReservations.Count; j++)
                                {
                                    sortedReservations[j].Status = 2;
                                    context.Update(sortedReservations[i]);
                                    context.SaveChanges();

                                }
                            }

                        }

                    }
                }

                #endregion

                #region Handle Buy now

                var buyNowReservation = await scopeService.GetBuyNowReservation();
                if (buyNowReservation != null)
                {
                    foreach (var item in buyNowReservation)
                    {
                        // Check if the entity is already tracked
                        var existingItem = context.Set<Reservation>().Local.FirstOrDefault(e => e.Id == item.Id);
                        if (existingItem != null)
                        {
                            // If the entity is tracked, update its properties
                            existingItem.Status = 2;
                            existingItem.Order = 0;
                            context.Entry(existingItem).State = EntityState.Modified;
                        }
                        else
                        {
                            // If the entity is not tracked, attach it
                            context.Attach(item);
                            context.Entry(item).State = EntityState.Modified;
                        }

                        // Check deadline deposit date
                        if (scopeService.GetDeadlineDepositDate(item.Id) == DateTime.Today)
                        {
                            var exdepositTransaction = context.Transactions.FirstOrDefault(t => t.ReservationId == item.Id && t.Type == 0);
                            var depositTransaction = context.Transactions.FirstOrDefault(t => t.ReservationId == item.Id && t.Type == 0 && t.Status == true);

                            if (depositTransaction == null && item.Status != 2)
                            {
                                // Update status to 2 (cancelled)
                                if (existingItem != null)
                                {
                                    existingItem.Status = 2;
                                    existingItem.Order = 0;
                                    context.Entry(existingItem).State = EntityState.Modified;
                                }
                                else
                                {
                                    context.Attach(item);
                                    item.Status = 2;
                                    item.Order = 0;
                                    context.Entry(item).State = EntityState.Modified;
                                }

                                // Save changes to the database
                                context.SaveChanges();
                                NewsController.CreateNewForAll(exdepositTransaction.Reservation.UserId, exdepositTransaction.Id, 9, DateTime.Today);
                            }
                        }
                    }

                }

                #endregion

          
            }
            //await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}



