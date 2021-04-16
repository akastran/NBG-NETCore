using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBank.Core.Constants;
using TinyBank.Core.Model;
using TinyBank.Core.Services;
using TinyBank.Core.Services.Options;

namespace TinyBank.Core.Implementation.Services
{
    public class CardService : ICardService
    {
        private readonly Data.TinyBankDbContext _dbContext;

        public CardService(Data.TinyBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Card> SearchCard(SearchCardOptions options)
        {
            if (options == null) {
                throw new ArgumentNullException(nameof(options));
            }

            // SELECT FROM CARD
            var q = _dbContext.Set<Card>()
                .AsQueryable();

            // SELECT FROM CARD WHERE CardId = options.CardId
            if (options.CardId != null) {
                q = q.Where(c => c.CardId == options.CardId);
            }

            if (options.TrackResults != null &&
              !options.TrackResults.Value) {
                q = q.AsNoTracking();
            }

            if (options.Skip != null) {
                q = q.Skip(options.Skip.Value);
            }

            q = q.Take(options.MaxResults ?? 500);

            return q;
        }

        public ApiResult<Card> GetByCardId(Guid? cardId)
        {
            if (cardId == null) {
                return new ApiResult<Card>() {
                    Code = ApiResultCode.BadRequest,
                    ErrorText = $"Bad request - Empty Card Id"
                };
            }

            var card = SearchCard(
                new SearchCardOptions() {
                    CardId = cardId
                })
                .Include(c => c.Accounts)
                .FirstOrDefault();

            if (card == null) {
                return new ApiResult<Card>() {
                    Code = ApiResultCode.NotFound,
                    ErrorText = $"Card {cardId} was not found"
                };
            }

            return new ApiResult<Card>() {
                Data = card
            };
        }

        public ApiResult<Card> GetByCardNumber(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber)) {
                return new ApiResult<Card>() {
                    Code = ApiResultCode.BadRequest,
                    ErrorText = $"Bad request - Empty Card Number"
                };
            }

            var card = SearchCard(
                new SearchCardOptions() {
                    CardNumber = cardNumber
                })
                .Include(c => c.Accounts)
                .FirstOrDefault();

            if (card == null) {
                return new ApiResult<Card>() {
                    Code = ApiResultCode.NotFound,
                    ErrorText = $"Card {cardNumber} was not found"
                };
            }

            return new ApiResult<Card>() {
                Data = card
            };
        }

        public ApiResult<Card> Checkout(CheckoutOptions options)
        {
            if (options == null) {
                return new ApiResult<Card>() {
                    Code = ApiResultCode.BadRequest,
                    ErrorText = $"Bad request"
                };
            }

            var result = GetByCardNumber(options.CardNumber);

            if (!result.IsSuccessful()) {
                return result;
            }

            var card = result.Data;

            var cardValidationsresult = CardValidations(card, options);
            if (!cardValidationsresult.IsSuccessful()) {
                return cardValidationsresult;
            }

            var accountValidationsresult = AccountValidations(card, options);
            if (!accountValidationsresult.IsSuccessful()) {
                return accountValidationsresult;
            }

            if (card != null) {
                card.Accounts[0].Balance -= options.Amount;

                _dbContext.SaveChanges();
            }
            else {
                return new ApiResult<Card>() {
                    Code = ApiResultCode.NotFound,
                    ErrorText = $"Card not found !"
                };
            }

            return new ApiResult<Card>() {
                Data = card
            };
        }

        public ApiResult<Card> CardValidations(Card card, CheckoutOptions options)
        {
            var result = new ApiResult<Card>();

            if (!card.Active) {
                result.Code = ApiResultCode.Conflict;
                result.ErrorText = $"Card {card.CardNumber} is not active!";
            }
            else if (options.ExpiryMonth != card.Expiration.Month) {
                result.Code = ApiResultCode.Conflict;
                result.ErrorText = $"Expiry month {options.ExpiryMonth} is incorrect!";
            }
            else if (options.ExpiryYear != card.Expiration.Year) {
                result.Code = ApiResultCode.Conflict;
                result.ErrorText = $"Expiry year {options.ExpiryYear} is incorrect!";
            }
            else {
                result.Code = ApiResultCode.Success;
                result.Data = card;
            }

            return result;
        }

        public ApiResult<Card> AccountValidations(Card card, CheckoutOptions options)
        {
            var result = new ApiResult<Card>();

            if (card.Accounts[0].State != AccountState.Active) {
                result.Code = ApiResultCode.Conflict;
                result.ErrorText = $"Related Account is not active!";
                //result.ErrorText = $"Account {card.Accounts[0].AccountId} is not active!";
            }
            else if (options.Amount > card.Accounts[0].Balance) {
                result.Code = ApiResultCode.Conflict;
                result.ErrorText = $"Insufficient funds!";
            }
            else {
                result.Code = ApiResultCode.Success;
                result.Data = card;
            }

            return result;
        }
    }
}
