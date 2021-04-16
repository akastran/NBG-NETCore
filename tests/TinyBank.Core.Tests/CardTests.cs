using Microsoft.EntityFrameworkCore;

using System.Linq;

using TinyBank.Core.Implementation.Data;
using TinyBank.Core.Model;

using Xunit;

namespace TinyBank.Core.Tests
{
    public class CardTests : IClassFixture<TinyBankFixture>
    {
        private readonly TinyBankDbContext _dbContext;

        public CardTests(TinyBankFixture fixture)
        {
            _dbContext = fixture.DbContext;
        }

        [Fact]
        public void Card_Register_Success()
        {
            var customer = new Customer() {
                Firstname = "Exam",
                Lastname = "Test",
                VatNumber = "134679852",
                Email = "myemail@email.gr",
                IsActive = true
            };

            var account = new Account() {
                Balance = 500M,
                CurrencyCode = "EUR",
                State = Constants.AccountState.Active,
                AccountId = "GR0000000000000004042834002"
            };

            customer.Accounts.Add(account);

            var card = new Card() {
                Active = true,
                CardNumber = "4223444455556666",
                CardType = Constants.CardType.Debit
            };

            account.Cards.Add(card);

            _dbContext.Add(customer);
            _dbContext.SaveChanges();

            var customerFromDb = _dbContext.Set<Customer>()
                .Where(c => c.VatNumber == "134679852")
                .Include(c => c.Accounts)
                .ThenInclude(a => a.Cards)
                .SingleOrDefault();

            var customerCard = customerFromDb.Accounts
                .SelectMany(a => a.Cards)
                .Where(c => c.CardNumber == "4223444455556666")
                .SingleOrDefault();

            Assert.NotNull(customerCard);
            Assert.Equal(Constants.CardType.Debit, customerCard.CardType);
            Assert.True(customerCard.Active);
        }
    }
}
