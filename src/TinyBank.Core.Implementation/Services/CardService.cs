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

        //private bool CardExists(Guid cardId)
        //{
        //    if (cardId == null) {
        //        throw new ArgumentNullException(cardId.ToString());
        //    }

        //    return SearchCard(
        //        new SearchCardOptions() {
        //            CardId = cardId
        //        }).Any();
        //}

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

        public ApiResult<Card> GetCardById(Guid? cardId)
        {
            if (cardId == null) {
                return new TinyBank.Core.ApiResult<Card>() {
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
                    Code = Constants.ApiResultCode.NotFound,
                    ErrorText = $"Card {cardId} was not found"
                };
            }

            return new ApiResult<Card>() {
                Data = card
            };
        }

        public ApiResult<Card> GetCardByNumber(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber)) {
                return new TinyBank.Core.ApiResult<Card>() {
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
                    Code = Constants.ApiResultCode.NotFound,
                    ErrorText = $"Card {cardNumber} was not found"
                };
            }

            return new ApiResult<Card>() {
                Data = card
            };
        }

        //public ApiResult<Card> RegisterCard(RegisterCardOptions options)
        //{
        //    if (options == null) {
        //        return new ApiResult<Card>() {
        //            Code = ApiResultCode.BadRequest,
        //            ErrorText = $"Null {nameof(options)}"
        //        };
        //    }

        //    if (!IsValidCardType(options.CardType)) {
        //        return new ApiResult<Card>() {
        //            Code = ApiResultCode.BadRequest,
        //            ErrorText = $"Invalid Card type {options.CardType}"
        //        };
        //    }

        //    var customerExists = Exists(options.VatNumber);

        //    if (customerExists) {
        //        return new ApiResult<Customer>() {
        //            Code = ApiResultCode.Conflict,
        //            ErrorText = $"Customer with Vat number {options.VatNumber} already exists"
        //        };
        //    }

        //    var customer = _mapper.Map<Customer>(options);
        //    customer.IsActive = true;

        //    _dbContext.Add(customer);

        //    try {
        //        _dbContext.SaveChanges();
        //    }
        //    catch (Exception ex) {
        //        // log
        //        Console.WriteLine(ex);

        //        return new ApiResult<Customer>() {
        //            Code = ApiResultCode.InternalServerError,
        //            ErrorText = $"Customer could not be saved"
        //        };
        //    }

        //    return new ApiResult<Customer>() {
        //        Data = customer
        //    };
        //}
    }
}
