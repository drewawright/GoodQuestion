using GoodQuestion.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GoodQuestion.WebAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/Song")]
    public class SongController : ApiController
    {

        private SongServices CreateSongServices()
        {
            var songService = new SongServices();
            return songService;
        } 
    }
}
