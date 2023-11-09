using Microsoft.EntityFrameworkCore;
using Pink_Panthers_Project.Controllers;
using Pink_Panthers_Project.Data;
using Pink_Panthers_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pink_Panthers_Test
{
    [TestClass]
    public class AccountPaymentTests : Tests
    {
        [TestMethod]
        public async Task CanMakePaymentToAccount()
        {
            UnitTestingData.isUnitTesting = true;

            ProfileController profileController = new ProfileController(_context);
            Account? account = _context.Account.Where(ac => ac.ID == 5).SingleOrDefault(); //ID 5 is test student
            if (account != null)
                UnitTestingData._account = account;

            double currAmount = Math.Round(account!.AmountToBePaid, 2);

            Random rand = new Random();
            double payment = Math.Round(rand.NextDouble(), 2) + rand.Next(50); //Some random number between 0-300, rounded to 2 decimal places
            await profileController.Account(payment);

            double newAmount = account!.AmountToBePaid;
            Assert.AreEqual(currAmount - payment, newAmount == 0 ? newAmount - payment : newAmount); //If account has no balance to pay, then they can't make a payment

            account.AmountToBePaid = currAmount;
            _context.Account.Update(account);
            await _context.SaveChangesAsync();

            profileController.logoutAccount();
        }
    }

}
