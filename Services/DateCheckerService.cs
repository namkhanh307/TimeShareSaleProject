using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
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

                foreach (var group in groupReservations) //list group reservation
                {
                    if (group.Count >= 1) //duplicate reservation
                    {
                        var first = group.FirstOrDefault(r => r.Order == 1); //lay dau tien order == 1
                        if (first != null)
                        {
                            if (scopeService.GetDeadlineReserveDate(first.Id) <= DateTime.Today) //lay ngay deadline cua thang 1
                            {

                                var existTransaction = context.Transactions.FirstOrDefault(r => r.ReservationId == first.Id && r.Type == -1);
                                var reserveTransaction = context.Transactions.FirstOrDefault(t => t.ReservationId == first.Id && t.Type == -1 && t.Status == true);//tra tien reserve chua
                                if (reserveTransaction == null && first.Status != 2)//thang dau chua tra
                                {
                                    first.Status = 2;// cancel
                                    context.Update(first);
                                    context.SaveChanges();
                                    NewsController.CreateNewForAll(first.UserId, existTransaction.Id, 8);
                                    foreach (var item in group) //list cua duplicate 10 thang
                                    {
                                        item.Order--;
                                        context.Update(item);
                                        context.SaveChanges();
                                    }
                                }
                                else {

                                    
                                }
                            }

                            if (scopeService.GetDeadlineDepositDate(first.Id) <= DateTime.Today) //lay ngay deadline cua thang 1
                            {
                                var existTransaction = context.Transactions.FirstOrDefault(r => r.ReservationId == first.Id && r.Type == 0);
                                var depositTransaction = context.Transactions.FirstOrDefault(t => t.ReservationId == first.Id && t.Type == 0 && t.Status == true);
                               
                                if (depositTransaction == null && first.Status != 2)//thang dau chua tra
                                {
                                    first.Status = 2;// cancel
                                    context.Update(first);
                                    context.SaveChanges();
                                    NewsController.CreateNewForAll(first.UserId, existTransaction.Id, 9);
                                    foreach (var item in group) //list cua duplicate 10 thang
                                    {
                                        item.Order--;
                                        context.Update(item);
                                        context.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                }
                await context.SaveChangesAsync();
                #endregion
                #region Handle Buy now
                var buyNowReservation = await scopeService.GetBuyNowReservation();
                foreach (var item in buyNowReservation)
                {
                    //scopeService.GetDeadlineDepositDate(item.Id)
                    if ( item.RegisterDate < DateTime.Today)
                    {
                        var exdepositTransaction = context.Transactions.FirstOrDefault(t => t.ReservationId == item.Id && t.Type == 0);
                        var depositTransaction = context.Transactions.FirstOrDefault(t => t.ReservationId == item.Id && t.Type == 0 && t.Status == true);
                       
                        if (depositTransaction == null && item.Status != 2)
                        {
                            item.Status = 2;
                            context.Update(item);
                            context.SaveChanges();
                            NewsController.CreateNewForAll(exdepositTransaction.Reservation.UserId, exdepositTransaction.Id, 9);
                        }
                    }
                }

                #endregion
                #region Update done reservation status
                //update status for reservation if every field of transaction is done
                var finishedReservation = await scopeService.GetFinishedReservation();
                if (finishedReservation != null)
                {
                    foreach (var item in finishedReservation)
                    {
                        bool allTransactionIsPaid = item.Transactions.All(t => t.Status == true && item.Transactions.Count >= 5);
                        if (allTransactionIsPaid && item.Status != 4)
                        {
                            item.Status = 4;
                            context.Update(item);
                            context.SaveChanges();
                            NewsController.CreateFinishNews(item.UserId);
                        }
                    }
                }
                #endregion
            }
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}



