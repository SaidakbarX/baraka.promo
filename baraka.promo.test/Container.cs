using baraka.promo.Core;
using baraka.promo.Data;
using baraka.promo.Delivery;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace baraka.promo.test
{
    public class Container
    {

        public static Guid DEFAULT_GUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
        public static Guid DEFAULT_GUID2 = Guid.Parse("00000000-0000-0000-0000-000000000002");
        public static Guid DEFAULT_GUID3 = Guid.Parse("00000000-0000-0000-0000-000000000003");
        public static Guid DEFAULT_GUID4 = Guid.Parse("00000000-0000-0000-0000-000000000004");


        protected readonly IMediator _mediator;
        protected readonly ServiceCollection services;

        public Container()
        {
            services = new ServiceCollection();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("Test");
            });
            
            services.AddDbContext<DeliveryDbContext>(options =>
            {
                options.UseSqlServer("Server=172.16.150.122;Database=delivery_test2;User Id=sa;Password=DBP@ssw0rd; MultipleActiveResultSets=true;");
            });

            services.AddTransient<CheckSegment>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

            services.AddOptions();
            services.AddLogging();
            _mediator = services.BuildServiceProvider()
                .GetRequiredService<IMediator>();
        }

        protected ApplicationDbContext GetNewContext()
        {
            return services.BuildServiceProvider().GetService<ApplicationDbContext>();
        }
        protected ApplicationDbContext GetNewContextAndInit()
        {
            var context = GetNewContext();

            if (!context.Promos.Any())
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        //Type.All promo
                        context.Promos.Add(new Promo()
                        {
                            Id = 10001,
                            Name = "Test1",
                            MaxCount = 1, 
                            StartTime = DateTime.Now,
                            EndTime = null,
                            MinOrderAmount = 1000,
                            MaxOrderAmount = 2000,                    
                            OrderDiscount = null,
                            Type = PromoType.All,
                            View = PromoView.FreeDelivery,
                            IsDeleted = false,
                            IsActive = true
                        });
                        context.SaveChanges();

                        context.Promos.Add(new Promo()
                        {
                            Id = 10002,
                            Name = "Test2",
                            MaxCount = 2, 
                            StartTime = DateTime.Now,
                            EndTime = null,
                            MinOrderAmount = null,
                            MaxOrderAmount = 2000,                    
                            OrderDiscount = 20,
                            Type = PromoType.All,
                            View = PromoView.OrderDiscount,
                            IsDeleted = false,
                            IsActive = true
                        });
                        context.SaveChanges();
                        
                        context.Promos.Add(new Promo()
                        {
                            Id = 10003,
                            Name = "Test3",
                            MaxCount = 1, 
                            StartTime = DateTime.Now,
                            EndTime = null,
                            MinOrderAmount = 3000,
                            MaxOrderAmount = null,                    
                            OrderDiscount = null,
                            Type = PromoType.All,
                            View = PromoView.FreeProduct,
                            IsDeleted = false,
                            IsActive = true
                        });
                        context.SaveChanges();

                        context.PromoProducts.Add(new PromoProduct()
                        {
                            PromoId = 10003,
                            ProductId = DEFAULT_GUID.ToString(),
                            Count = 1,
                            Discount = null
                        });
                        context.SaveChanges();

                        context.Promos.Add(new Promo()
                        {
                            Id = 10004,
                            Name = "Test4",
                            MaxCount = 1, 
                            StartTime = DateTime.Now,
                            EndTime = null,
                            MinOrderAmount = 4000,
                            MaxOrderAmount = 5000,                    
                            OrderDiscount = null,
                            Type = PromoType.All,
                            View = PromoView.ProductDiscount,
                            IsDeleted = false,
                            IsActive = true
                        });
                        context.SaveChanges();

                        context.PromoProducts.Add(new PromoProduct()
                        {
                            PromoId = 10003,
                            ProductId = DEFAULT_GUID2.ToString(),
                            Count = 2,
                            Discount = 40
                        });
                        context.SaveChanges();

                        ////Type.Personal Promo
                        context.Promos.Add(new Promo()
                        {
                            Id = 10005,
                            Name = "Test5",
                            MaxCount = 1, 
                            StartTime = DateTime.Now,
                            EndTime = null,
                            MinOrderAmount = 4000,
                            MaxOrderAmount = 6000,                    
                            OrderDiscount = null,
                            Type = PromoType.Personal,
                            View = PromoView.FreeDelivery,
                            IsDeleted = false,
                            IsActive = true
                        });
                        context.SaveChanges();

                        context.PromoClients.Add(new PromoClient()
                        {
                            Id = DEFAULT_GUID,
                            OrderId = null,
                            Phone = "998917701703",
                            PromoId = 10005,
                            TimeOfUse = null
                        });
                        context.SaveChanges();

                        context.Promos.Add(new Promo()
                        {
                            Id = 10006,
                            Name = "Test6",
                            MaxCount = 2, 
                            StartTime = DateTime.Now,
                            EndTime = null,
                            MinOrderAmount = 4000,
                            MaxOrderAmount = 6000,                    
                            OrderDiscount = 20,
                            Type = PromoType.Personal,
                            View = PromoView.OrderDiscount,
                            IsDeleted = false,
                            IsActive = true
                        });
                        context.SaveChanges();

                        context.PromoClients.Add(new PromoClient()
                        {
                            Id = DEFAULT_GUID2,
                            OrderId = null,
                            Phone = "998917701703",
                            PromoId = 10006,
                            TimeOfUse = null
                        });
                        context.SaveChanges();

                        context.Promos.Add(new Promo()
                        {
                            Id = 10007,
                            Name = "Test7",
                            MaxCount = 1, 
                            StartTime = DateTime.Now,
                            EndTime = null,
                            MinOrderAmount = null,
                            MaxOrderAmount = 3000,                    
                            OrderDiscount = null,
                            Type = PromoType.Personal,
                            View = PromoView.FreeProduct,
                            IsDeleted = false,
                            IsActive = true
                        });
                        context.SaveChanges();

                        context.PromoProducts.Add(new PromoProduct()
                        {
                            PromoId = 10007,
                            ProductId = DEFAULT_GUID3.ToString(),
                            Count = 1,
                            Discount = null
                        });
                        context.SaveChanges();

                        context.PromoClients.Add(new PromoClient()
                        {
                            Id = DEFAULT_GUID3,
                            OrderId = null,
                            Phone = "998917701703",
                            PromoId = 10007,
                            TimeOfUse = null
                        });
                        context.SaveChanges();

                        context.Promos.Add(new Promo()
                        {
                            Id = 10008,
                            Name = "Test8",
                            MaxCount = 1, 
                            StartTime = DateTime.Now,
                            EndTime = null,
                            MinOrderAmount = null,
                            MaxOrderAmount = 5000,                    
                            OrderDiscount = null,
                            Type = PromoType.Personal,
                            View = PromoView.ProductDiscount,
                            IsDeleted = false,
                            IsActive = true
                        });
                        context.SaveChanges();

                        context.PromoProducts.Add(new PromoProduct()
                        {
                            PromoId = 10008,
                            ProductId = DEFAULT_GUID4.ToString(),
                            Count = 2,
                            Discount = 40
                        });
                        context.SaveChanges();

                        context.PromoClients.Add(new PromoClient()
                        {
                            Id = DEFAULT_GUID4,
                            OrderId = null,
                            Phone = "998917701703",
                            PromoId = 10008,
                            TimeOfUse = null
                        });
                        context.SaveChanges();

                        // Use Promo
                        context.Promos.Add(new Promo()
                        {
                            Id = 10010,
                            Name = "Test10",
                            MaxCount = 1,
                            StartTime = DateTime.Now,
                            EndTime = null,
                            MinOrderAmount = 2500,
                            MaxOrderAmount = 3000,
                            OrderDiscount = null,
                            Type = PromoType.Personal,
                            View = PromoView.FreeProduct,
                            IsDeleted = false,
                            IsActive = true
                        });
                        context.SaveChanges();

                        context.PromoProducts.Add(new PromoProduct()
                        {
                            PromoId = 10010,
                            ProductId = DEFAULT_GUID3.ToString(),
                            Count = 1,
                            Discount = null
                        });
                        context.SaveChanges();

                        context.PromoClients.Add(new PromoClient()
                        {
                            Id = Guid.NewGuid(),
                            OrderId = null,
                            Phone = "998994348242",
                            PromoId = 10010,
                            TimeOfUse = null
                        });
                        context.SaveChanges();


                        // Unavailable promo

                        context.Promos.Add(new Promo()
                        {
                            Id = 10009,
                            Name = "Test9",
                            MaxCount = 1,
                            StartTime = DateTime.Now,
                            EndTime = null,
                            MinOrderAmount = 1000,
                            MaxOrderAmount = 2000,
                            OrderDiscount = null,
                            Type = PromoType.All,
                            View = PromoView.FreeDelivery,
                            IsDeleted = true,
                            IsActive = true
                        });
                        context.SaveChanges();

                        context.Promos.Add(new Promo()
                        {
                            Id = 10020,
                            Name = "TestSegment",
                            MaxCount = 1,
                            StartTime = DateTime.Now,
                            EndTime = null,
                            MinOrderAmount = 1000,
                            MaxOrderAmount = null,
                            OrderDiscount = null,
                            Type = PromoType.Segment,
                            View = PromoView.FreeDelivery,
                            SegmentId = 1,
                            IsDeleted = false,
                            IsActive = true
                        });
                        context.SaveChanges();

                        context.Segments.Add(new Segment()
                        {
                            Id = 1,
                            Name = "Free Delivery",
                            AmountMin = 1000,
                            AmountMax = null,
                            DateFrom = DateTime.Now.AddDays(-15),
                            DateTo = null,
                            OrderTimeFrom = null,
                            OrderTimeTo = null,
                            OrderTypeIds = null,
                            QuantityMax = null,
                            QuantityMin = null,
                            TotalAmountMax = null,
                            TotalAmountMin = null,
                            CategoryIds = JsonConvert.SerializeObject(new List<string>() { "973da293-c43c-440c-a34c-7f208c1c8ac8" }),
                            ProductIds = JsonConvert.SerializeObject(new List<string>() { "091317ED-BFEC-48A6-9BE3-5F36950C6F75" }),
                        });
                        context.SaveChanges();

                        scope.Complete();
                    }
                    catch (Exception e)
                    {
                        scope.Dispose();
                    }
                }
            }
            return context;
        }
    }
}
