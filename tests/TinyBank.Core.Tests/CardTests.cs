using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TinyBank.Core.Constants;
using TinyBank.Core.Implementation.Data;
using TinyBank.Core.Model;
using TinyBank.Core.Services;
using TinyBank.Core.Services.Options;
using Xunit;

namespace TinyBank.Core.Tests
{
    public class CardTests : IClassFixture<TinyBankFixture>
    {
        private readonly TinyBankDbContext _dbContext;
        private readonly ICardService _cards;

        public CardTests(TinyBankFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _cards = fixture.GetService<ICardService>();
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

        [Fact]
        public void Card_Chekout_Success()
        {
            var customer = new Customer() {
                Firstname = "Exam2",
                Lastname = "Test2",
                VatNumber = "134679863",
                Email = "myemail2@email2.gr",
                IsActive = true
            };

            var account = new Account() {
                Balance = 500M,
                CurrencyCode = "EUR",
                State = Constants.AccountState.Active,
                AccountId = "GR0000000000000004041834002"
            };

            customer.Accounts.Add(account);

            var card = new Card() {
                Active = true,
                CardNumber = "4223454455556666",
                CardType = Constants.CardType.Debit
            };

            account.Cards.Add(card);

            _dbContext.Add(customer);
            _dbContext.SaveChanges();

            var customerFromDb = _dbContext.Set<Customer>()
                .Where(c => c.VatNumber == customer.VatNumber)
                .Include(c => c.Accounts)
                .ThenInclude(a => a.Cards)
                .SingleOrDefault();

            var customerCard = customerFromDb.Accounts
                .SelectMany(a => a.Cards)
                .Where(c => c.CardNumber == card.CardNumber)
                .SingleOrDefault();

            Assert.NotNull(customerCard);
            Assert.Equal(Constants.CardType.Debit, customerCard.CardType);
            Assert.True(customerCard.Active);

            var checkoutOptions = new CheckoutOptions() {
                CardNumber = card.CardNumber,
                ExpiryMonth = DateTimeOffset.Now.Month,
                ExpiryYear = DateTimeOffset.Now.Year + 6,
                Amount = 3M
            };

            var checkoutResult = _cards.Checkout(checkoutOptions);

            Assert.NotNull(checkoutResult);
            Assert.Equal(ApiResultCode.Success, checkoutResult.Code);
            Assert.NotNull(checkoutResult.Data);
        }

        [Fact]
        public void Card_Chekout_Fail_InsufficientFunds()
        {
            var customer = new Customer() {
                Firstname = "Exam6",
                Lastname = "Test6",
                VatNumber = "169899863",
                Email = "myemail6@email6.gr",
                IsActive = true
            };

            var account = new Account() {
                Balance = 500M,
                CurrencyCode = "EUR",
                State = Constants.AccountState.Active,
                AccountId = "GR0000000000000002041424342"
            };

            customer.Accounts.Add(account);

            var card = new Card() {
                Active = true,
                CardNumber = "4163454455577886",
                CardType = Constants.CardType.Debit
            };

            account.Cards.Add(card);

            _dbContext.Add(customer);
            _dbContext.SaveChanges();

            var customerFromDb = _dbContext.Set<Customer>()
                .Where(c => c.VatNumber == customer.VatNumber)
                .Include(c => c.Accounts)
                .ThenInclude(a => a.Cards)
                .SingleOrDefault();

            var customerCard = customerFromDb.Accounts
                .SelectMany(a => a.Cards)
                .Where(c => c.CardNumber == card.CardNumber)
                .SingleOrDefault();

            Assert.NotNull(customerCard);
            Assert.Equal(Constants.CardType.Debit, customerCard.CardType);
            Assert.True(customerCard.Active);

            var checkoutOptions = new CheckoutOptions() {
                CardNumber = customerCard.CardNumber,
                ExpiryMonth = DateTimeOffset.Now.Month,
                ExpiryYear = DateTimeOffset.Now.Year + 5,
                Amount = 503M
            };

            var checkoutResult = _cards.Checkout(checkoutOptions);

            Assert.NotNull(checkoutResult);
            Assert.Equal(ApiResultCode.Conflict, checkoutResult.Code);
            Assert.Null(checkoutResult.Data);
            Assert.Contains("Insufficient funds", checkoutResult.ErrorText);
        }

        [Fact]
        public void Card_Chekout_Fail_WrongYear()
        {
            var customer = new Customer() {
                Firstname = "Exam7",
                Lastname = "Test7",
                VatNumber = "179899863",
                Email = "myemail6@email6.gr",
                IsActive = true
            };

            var account = new Account() {
                Balance = 500M,
                CurrencyCode = "EUR",
                State = Constants.AccountState.Active,
                AccountId = "GR0000000000000003041424342"
            };

            customer.Accounts.Add(account);

            var card = new Card() {
                Active = true,
                CardNumber = "4173454455577886",
                CardType = Constants.CardType.Debit
            };

            account.Cards.Add(card);

            _dbContext.Add(customer);
            _dbContext.SaveChanges();

            var customerFromDb = _dbContext.Set<Customer>()
                .Where(c => c.VatNumber == customer.VatNumber)
                .Include(c => c.Accounts)
                .ThenInclude(a => a.Cards)
                .SingleOrDefault();

            var customerCard = customerFromDb.Accounts
                .SelectMany(a => a.Cards)
                .Where(c => c.CardNumber == card.CardNumber)
                .SingleOrDefault();

            Assert.NotNull(customerCard);
            Assert.Equal(Constants.CardType.Debit, customerCard.CardType);
            Assert.True(customerCard.Active);

            var checkoutOptions = new CheckoutOptions() {
                CardNumber = customerCard.CardNumber,
                ExpiryMonth = DateTimeOffset.Now.Month,
                ExpiryYear = DateTimeOffset.Now.Year + 5,
                Amount = 503M
            };

            var checkoutResult = _cards.Checkout(checkoutOptions);

            Assert.NotNull(checkoutResult);
            Assert.Equal(ApiResultCode.Conflict, checkoutResult.Code);
            Assert.Null(checkoutResult.Data);
            Assert.Contains("Expiry year", checkoutResult.ErrorText);
        }
    }
}
