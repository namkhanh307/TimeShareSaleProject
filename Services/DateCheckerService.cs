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

                //foreach (var group in groupReservations) //list group reservation
                //{
                //    if (group.Count >= 1) //duplicate reservation
                //    {
                //        var first = group.FirstOrDefault(r => r.Order == 1); //lay dau tien order == 1
                //        if (first != null)
                //        {
                //            if (scopeService.GetDeadlineReserveDate(first.Id) == DateTime.Today) //lay ngay deadline cua thang 1
                //            {
                //                var reserveTransaction = context.Transactions.FirstOrDefault(t => t.ReservationId == first.Id && t.Type == -1 && t.Status == true);//tra tien reserve chua
                //                var DepositTransaction = context.Transactions.FirstOrDefault(t => t.ReservationId == first.Id && t.Type == 0 && t.Status == true);//tra tien reserve chua
                //                if (reserveTransaction == null && first.Status != 2)//thang dau chua tra
                //                {
                //                    first.Status = 2;// cancel
                //                    context.Update(first);
                //                    context.SaveChanges();
                //                    NewsController.CreateNewForAll(first.UserId, reserveTransaction.Id, 8);
                //                    foreach (var item in group) //list cua duplicate 10 thang
                //                    {
                //                        item.Order--;
                //                        context.Update(item);
                //                        context.SaveChanges();
                //                    }
                //                }if(DepositTransaction == null)
                //                {


                //                }
                //            }

                //            if (scopeService.GetDeadlineDepositDate(first.Id) == DateTime.Today) //lay ngay deadline cua thang 1
                //            {
                //                var existTransaction = context.Transactions.FirstOrDefault(r => r.ReservationId == first.Id && r.Type == 0);
                //                var depositTransaction = context.Transactions.FirstOrDefault(t => t.ReservationId == first.Id && t.Type == 0 && t.Status == true);

                //                if (depositTransaction == null && first.Status != 2)//thang dau chua tra
                //                {
                //                    first.Status = 2;// cancel
                //                    context.Update(first);
                //                    context.SaveChanges();
                //                    NewsController.CreateNewForAll(first.UserId, existTransaction.Id, 9);
                //                    //foreach (var item in group) //list cua duplicate 10 thang
                //                    //{
                //                    //    item.Order--;
                //                    //    context.Update(item);
                //                    //    context.SaveChanges();
                //                    //}
                //                }
                //                else
                //                {
                //                    foreach (var item in group)
                //                    {
                //                        if (item.Order > 1)
                //                        {
                //                            item.Status = 2;
                //                            context.Update(item);
                //                            context.SaveChanges();
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}

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
                                    context.Entry(existingItem).State = EntityState.Modified;
                                }
                                else
                                {
                                    context.Attach(item);
                                    item.Status = 2;
                                    context.Entry(item).State = EntityState.Modified;
                                }

                                // Save changes to the database
                                context.SaveChanges();
                                NewsController.CreateNewForAll(exdepositTransaction.Reservation.UserId, exdepositTransaction.Id, 9,DateTime.Today);
                            }
                        }
                    }

                }

                //var buyNowReservation = await scopeService.GetBuyNowReservation();
                //if (buyNowReservation != null)
                //{
                //    foreach (var item in buyNowReservation)
                //    {
                //        //scopeService.GetDeadlineDepositDate(item.Id)
                //        if (scopeService.GetDeadlineDepositDate(item.Id) == DateTime.Today)
                //        {
                //            var exdepositTransaction = context.Transactions.FirstOrDefault(t => t.ReservationId == item.Id && t.Type == 0);
                //            var depositTransaction = context.Transactions.FirstOrDefault(t => t.ReservationId == item.Id && t.Type == 0 && t.Status == true);

                //            if (depositTransaction == null && item.Status != 2)
                //            {

                //                item.Status = 2;

                //                context.SaveChanges();
                //                NewsController.CreateNewForAll(exdepositTransaction.Reservation.UserId, exdepositTransaction.Id, 9);
                //            }
                //        }
                //    }
                //}
                #endregion

                #region Update done reservation status
                var finishedReservation = await scopeService.GetFinishedReservation();
                if (finishedReservation != null)
                {
                    foreach (var item in finishedReservation)
                    {
                        // Check if the entity is already tracked
                        var finishReservation = new Reservation();
                

                        // Check if all transactions are paid and the status is not already set to 4
                        bool allTransactionIsPaid = item.Transactions.All(t => t.Status == true && item.Transactions.Count >= 4);
                        if (allTransactionIsPaid && item.Status != 4)
                        {
                            var existingItem = context.Set<Reservation>().Local.FirstOrDefault(e => e.Id == item.Id);
                            if (existingItem != null)
                            {
                                // If the entity is tracked, update its properties
                                existingItem.Status = 4;
                                context.Entry(existingItem).State = EntityState.Modified;
                            }
                            else
                            {
                                // If the entity is not tracked, attach it
                                context.Attach(item);
                                item.Status = 4;
                                context.Entry(item).State = EntityState.Modified;
                            }
                            context.SaveChanges();
                            NewsController.CreateFinishNews(item.UserId, Common.GetDepositIDByResevationID(item.Id));
                        }
                      
                    }
                }

                ////update status for reservation if every field of transaction is done
                //var finishedReservation = await scopeService.GetFinishedReservation();
                //if (finishedReservation != null)
                //{
                //    foreach (var item in finishedReservation)
                //    {
                //        bool allTransactionIsPaid = item.Transactions.All(t => t.Status == true && item.Transactions.Count >= 5);
                //        if (allTransactionIsPaid && item.Status != 4)
                //        {

                //            item.Status = 4;
                //            context.Update(item);
                //            context.SaveChanges();
                //            NewsController.CreateFinishNews(item.UserId);
                //        }
                //    }
                //}
                #endregion
            }
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

        }
    }
}



