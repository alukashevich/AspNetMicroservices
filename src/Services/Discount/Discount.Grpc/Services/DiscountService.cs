using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories.Interfaces;
using Grpc.Core;

namespace Discount.Grpc.Services
{
    public class DiscountService: DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IDiscountRepository repository;
        private readonly ILogger<DiscountService> logger;
        private readonly IMapper mapper;

        public DiscountService(IDiscountRepository repository, ILogger<DiscountService> logger, IMapper mapper)
        {
            this.repository = repository;
            this.logger = logger;
            this.mapper = mapper;
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            Coupon coupon = await repository.GetDiscount(request.ProductName);
            if(coupon == null)
            {
                logger.LogError($"Discount for {request.ProductName} not found");
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount for {request.ProductName} not found"));
            }

            logger.LogInformation($"Discount is retrieved for {coupon}");

            CouponModel couponModel = mapper.Map<CouponModel>(coupon);

            return couponModel;
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            Coupon coupon = mapper.Map<Coupon>(request.Coupon);

            await repository.CreateDiscount(coupon);

            string message = $"Discount was created as {coupon}";

            logger.LogInformation(message);

            return mapper.Map<CouponModel>(coupon);
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            Coupon coupon = mapper.Map<Coupon>(request.Coupon);

            bool result = await repository.UpdateDiscount(coupon);

            if(!result)
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount for {coupon} was not updated"));

            string message = $"Discount was updated as {coupon}";

            logger.LogInformation(message);

            return mapper.Map<CouponModel>(coupon);
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            bool deleted = await repository.DeleteDiscount(request.ProductName);

            return new DeleteDiscountResponse { Success = deleted };
        }
    }
}
