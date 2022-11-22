using Discount.Grpc.Protos;
using Grpc.Core;

namespace Basket.API.GrpcServices
{
    public class DiscountGrpcService
    {
        private readonly DiscountProtoService.DiscountProtoServiceClient _discountProtoService;

        public DiscountGrpcService(DiscountProtoService.DiscountProtoServiceClient discountProtoService)
        {
            _discountProtoService = discountProtoService ?? throw new ArgumentNullException(nameof(discountProtoService));
        }

        public async Task<CouponModel> GetCouponModel(string productName, CancellationToken cancellationToken)
        {
            GetDiscountRequest discountRequest = new() { ProductName = productName };
            return await _discountProtoService
                .GetDiscountAsync(discountRequest, cancellationToken: cancellationToken);
        }
    }
}
