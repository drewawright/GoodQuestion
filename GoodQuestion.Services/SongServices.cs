using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;

namespace GoodQuestion.Services
{
    public class SongServices
    {
        private SpotifyWebAPI _api = new SpotifyWebAPI
        {
            AccessToken = "BQAu0mW3ne5pg89O1GWpqLCKOAS6p3mRVlzOSiHcwwYyPZhJugeJH7Oym05t2mlm8PWTpxAX9wQgWMv_O3MVqK1FPrN8amq_NDzewVBljSXZVEpNChYAtibNMyHP6TKN78pi91MaZ7K2wRh1ksEW0ZUkaPJ1jnMZWN6expIA5GwY6f5lYcK-RQV2W9RT1xYzoHO4qKVfh0dZdDZLDoa9lC_dnX9tD_A3UcxvJteHZDev1kFihT_q5_YEEVQ3r_ekpEPuicvoZGjJJNoX0AAH8w-vPehQnIB9",
            TokenType = "Bearer"
        };
        private string _accountId = "38vdur0tacvhr9wud418mvzqh";

    }
}
