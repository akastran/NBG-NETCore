using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBank.Core.Model;

namespace TinyBank.Core.Services
{
    public interface ICardService
    {
        public ApiResult<Card> GetCardById(Guid? cardId);
        public ApiResult<Card> GetCardByNumber(string cardNumber);
        //public ApiResult<Card> RegisterCard(Options.RegisterCardOptions options);
    }
}
