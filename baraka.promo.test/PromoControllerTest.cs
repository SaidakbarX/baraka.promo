using baraka.promo.Core;
using baraka.promo.Data;
using baraka.promo.Models;
using baraka.promo.Models.OrderApiModel;
using baraka.promo.Pages.TgPushSender;
using baraka.promo.Utils;
using Humanizer;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Transactions;

namespace baraka.promo.test
{
    public class PromoControllerTest : Container
    {
        [Fact]
        public async Task GetPromo_ReturnsOkResult()
        {
            _ = GetNewContextAndInit();

            var orderModel = new OrderModel
            {
                ClientPhone = "998917701703",
                Products = new List<OrderProductModel>
                {
                    new OrderProductModel { ProductId = Guid.Parse("00000000-0000-0000-0000-000000000004") }
                },
                OrderAmount = 1500,
                RegionId = 0,
                RestaurantId = Guid.Parse("7ADD6FEA-2DF7-4F05-8ED8-7A71BB40AFCF")
            };

            var expectedResult = new ApiBaseResultModel<OrderApiResultModel>
            {
                Data =  new OrderApiResultModel()
                {
                    Promo = new List<PromoApiResultModel>
                    {
                        new PromoApiResultModel
                        {
                            Id = 10001,
                            Name = "Test1",
                            MaxCount = 1,
                            OrderDiscount = null,
                            MinOrderAmount = 1000,
                            MaxOrderAmount = 2000,
                            PromoType = Data.PromoType.All,
                            PromoView = Data.PromoView.FreeDelivery,
                            FreeProduct = null,
                            ProductDiscount = null
                        },
                        new PromoApiResultModel
                        {
                            Id = 10002,
                            Name = "Test2",
                            MaxCount = 2,
                            OrderDiscount = 20,
                            MinOrderAmount = null,
                            MaxOrderAmount = 2000,
                            PromoType = Data.PromoType.All,
                            PromoView = Data.PromoView.OrderDiscount,
                            FreeProduct = null,
                            ProductDiscount = null
                        },
                        new PromoApiResultModel
                        {
                            Id = 10007,
                            Name = "Test7",
                            MaxCount = 1,
                            OrderDiscount = null,
                            MinOrderAmount = null,
                            MaxOrderAmount = 3000,
                            PromoType = Data.PromoType.Personal,
                            PromoView = Data.PromoView.FreeProduct,
                            FreeProduct = new List<FreeProductModel>()
                            {
                                new FreeProductModel
                                {
                                    //PromoId = 10007,
                                    Count = 1,
                                    //Discount = null,
                                    ProductId = "00000000-0000-0000-0000-000000000003"
                                } 
                            },
                            ProductDiscount = null
                        },
                        new PromoApiResultModel
                        {
                            Id = 10008,
                            Name = "Test8",
                            MaxCount = null,
                            OrderDiscount = null,
                            MinOrderAmount = null,
                            MaxOrderAmount = null,
                            PromoType = Data.PromoType.Personal,
                            PromoView = Data.PromoView.ProductDiscount,
                            FreeProduct = null,
                            ProductDiscount = new  List<DiscountProductModel>()
                            {
                                new DiscountProductModel
                                {
                                    //PromoId = 10008,
                                    Count = 2,
                                    Discount = 40,
                                    ProductId = "00000000-0000-0000-0000-000000000004"
                                }
                            }
                        }
                    }
                    
                },
                Success = true,
                Error = null
            };

            //mediatorMock.Setup(x => x.Send(It.IsAny<GetPromo.Command>(), default))
            //            .ReturnsAsync(expectedResult);

            var command = new GetPromo.Command(orderModel);
            var result = await _mediator.Send(command);

            Assert.NotNull(result);

            Assert.Multiple(() => 
            {
                Assert.Equal(expectedResult.Data.Promo.Count, result.Data?.Promo?.Count);

                for (int i = 0; i < expectedResult.Data.Promo.Count; i++)
                {
                    Assert.Equal(expectedResult.Data.Promo[i].Name, result.Data?.Promo?[i].Name);
                    Assert.Equal(expectedResult.Data.Promo[i].Id, result.Data?.Promo?[i].Id);
                    Assert.Equal(expectedResult.Data.Promo[i].PromoType, result.Data?.Promo?[i].PromoType);
                    Assert.Equal(expectedResult.Data.Promo[i].PromoView, result.Data?.Promo?[i].PromoView);
                }
            });

        }

        [Fact]
        public async Task GetPromoByName_ReturnsOkResult()
        {
            _ = GetNewContextAndInit();

            var model = new OrderPromoModel
            {
                PromoName = "Test5",
                ClientPhone = "998917701703",
                RegionId = 0,
                RestaurantId = Guid.Parse("7ADD6FEA-2DF7-4F05-8ED8-7A71BB40AFCF"),
                OrderAmount = 5000
            };

            var expectedResult = new ApiBaseResultModel<PromoApiResultModel>
            {
                Data = new PromoApiResultModel()
                {
                    Id = 10005,
                    Name = "Test5",
                    MaxCount = 1,
                    OrderDiscount = null,
                    MinOrderAmount = 4000,
                    MaxOrderAmount = 6000,
                    PromoType = PromoType.Personal,
                    PromoView = PromoView.FreeDelivery,
                    FreeProduct = null,
                    ProductDiscount = null
                },
                Success = true,
                Error = null
            };

            var command = new GetPromoByName.Command(model);
            var result = await _mediator.Send(command);


            // Assert
            Assert.NotNull(result);


            Assert.Equal(expectedResult.Data.Id, result.Data?.Id);
            Assert.Equal(expectedResult.Data.Name, result.Data?.Name);
            // Add more assertions for other properties
        }

        [Fact]
        public async Task AppliedPromo_ReturnsOkResult()
        {
            _ = GetNewContextAndInit();

            var model = new AppliedPromoModel
            {
                PromoId = 10010,
                ClientPhone = "998994348242",
                OrderId = Guid.NewGuid().ToString(),
                RegionId = 0,
                RestaurantId = Guid.Parse("7ADD6FEA-2DF7-4F05-8ED8-7A71BB40AFCF"),
                OrderAmount = 3000
            };

            var expectedResult = true;

            var command = new UserAppliedPromo.Command(model);
            var result = await _mediator.Send(command);

            Assert.NotNull(result);
            Assert.Null(result.Error);

            Assert.Equal(expectedResult, result.Success);
        }

        [Fact]
        public async Task AppliedPromo_ReturnsCantUseResult()
        {
            _ = GetNewContextAndInit();

            var model = new AppliedPromoModel
            {
                PromoId = 10007,
                ClientPhone = "998994445566",
                OrderId = Guid.NewGuid().ToString(),
                RegionId = 0,
                RestaurantId = Guid.Parse("7ADD6FEA-2DF7-4F05-8ED8-7A71BB40AFCF"),
                OrderAmount = 2500
            };

            var expectedResult = new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_PROMO_REQUIREMENTS));

            var command = new UserAppliedPromo.Command(model);
            var result = await _mediator.Send(command);

            Assert.NotNull(result);

            Assert.Equal(expectedResult.Error.Code, result.Error.Code);
        }
        
        [Fact]
        public async Task AppliedPromo_ReturnsUnavailableResult()
        {
            _ = GetNewContextAndInit();

            var model = new AppliedPromoModel
            {
                PromoId = 10009,
                ClientPhone = "998994348242",
                OrderId = Guid.NewGuid().ToString(),
                RegionId = 0,
                RestaurantId = Guid.Parse("7ADD6FEA-2DF7-4F05-8ED8-7A71BB40AFCF"),
                OrderAmount = 1000
            };

            var expectedResult = new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_PROMO_NOT_FOUND));

            var command = new UserAppliedPromo.Command(model);
            var result = await _mediator.Send(command);

            Assert.NotNull(result);

            Assert.Equal(expectedResult.Error.Code, result.Error.Code);
        }
        
        [Fact]
        public async Task AppliedPromo_ReturnsNullResult()
        {
            _ = GetNewContextAndInit();

            var expectedResult = new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_PROMO_NOT_FOUND));

            var command = new UserAppliedPromo.Command(null);
            var result = await _mediator.Send(command);

            Assert.NotNull(result);

            Assert.Equal(expectedResult.Error.Code, result.Error.Code);
        }


        [Fact]
        public async Task GetPromoByNameSegment_ReturnsErrorResult()
        {
            _ = GetNewContextAndInit();

            var model = new OrderPromoModel
            {
                PromoName = "TestSegment",
                ClientPhone = "998917701703",
                RegionId = 0,
                RestaurantId = Guid.Parse("7ADD6FEA-2DF7-4F05-8ED8-7A71BB40AFCF"),
                OrderAmount = 5000
            };

            var expectedResult = new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_PROMO_REQUIREMENTS));

            var command = new GetPromoByName.Command(model);
            var result = await _mediator.Send(command);


            // Assert
            Assert.NotNull(result);

            Assert.Equal(expectedResult.Error.Code, result.Error.Code);
            //Assert.Contains("Сегмент этой промо-акции несовместим!", expectedResult.Error.Description);
            // Add more assertions for other properties
        }

        [Fact]
        public void TestParallelForEach()
        {
            int count = 0;
            var messagesdb = new List<int> { 1 };
            var messages = messagesdb.Take(10000).ToList();
            Parallel.ForEach(messages, new ParallelOptions
            {
                MaxDegreeOfParallelism = 20
            }, delegate (int msg, ParallelLoopState state)
            {
                Task.Delay(1000).Wait();
                count++;
            });
            Assert.Equal(count, messages.Count);
        }
    }
}