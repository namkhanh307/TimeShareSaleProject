using TimeShareProject.Models;

namespace TimeShareProject.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, Reservation model);
        //VnPaymentRequestModel PaymentExecute(IQueryCollection collections);
        Transaction PaymentExecute(IQueryCollection collections);


    }
}
