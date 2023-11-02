using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pink_Panthers_Project.Data;
using Pink_Panthers_Project.Util;
using Pink_Panthers_Project.Models;
using Stripe;
using Stripe.Checkout;
using Account = Pink_Panthers_Project.Models.Account;

namespace Pink_Panthers_Project.Controllers
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddNewtonsoftJson();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure Stripe API key
            StripeConfiguration.ApiKey = "sk_test_51O0TzbD0tDyaQxJ7NKO7QDy9x2jMErmvqneATyxYot6LW3DyQD0z6oG3iv8T51FFOM8uNEdKh3fZPGSovF4ahM0l00M9nAjGN2";

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }

    [Route("create-checkout-session")]
    [ApiController]
    public class ServerController : Controller
    {
        private readonly Pink_Panthers_ProjectContext _context;

        public ServerController(Pink_Panthers_ProjectContext context)
        {
            _context = context;
        }

        private Account GetAccount()
        {
            return HttpContext.Session.GetSessionValue<Account>("LoggedInAccount")!;
        }

        [HttpPost("create-checkout-session")]
        public ActionResult CreateCheckoutSession([FromForm] decimal hiddenAmountToPay)
        {
            var account = GetAccount();
            var unitAmountCents = (long)(hiddenAmountToPay * 100);

            var successUrl = $"{Request.Scheme}://{Request.Host}/Profile/Receipt?amountToPay={hiddenAmountToPay}&paymentStatus=succeeded";
            var cancelUrl = $"{Request.Scheme}://{Request.Host}/Profile/Account?amountToPay={hiddenAmountToPay}&paymentStatus=canceled";

            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = unitAmountCents,
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Tuition",
                            },
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
            };

            var service = new SessionService();
            Session session = service.Create(options);

            HttpContext.Session.SetSessionValue<decimal>("amountToPay", hiddenAmountToPay);
            return Redirect(session.Url);
        }


    }

}
