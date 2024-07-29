using ASM_Client.Models.VNPAY;

namespace ASM_Client.Services
{
    public interface IVnPaySvc
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);

        VnPaymentResponseModel PaymenteExecute(IQueryCollection collections);
    }
}
