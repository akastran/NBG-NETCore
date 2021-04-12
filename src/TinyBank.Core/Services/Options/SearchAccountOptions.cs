using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBank.Core.Services.Options
{
    public class SearchAccountOptions
    {
        public string AccountId { get; set; }
        public int? MaxResults { get; set; }
        public bool? TrackResults { get; set; }
        public int? Skip { get; set; }
    }
}
