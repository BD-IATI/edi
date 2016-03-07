using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AIMS_BD_IATI.WebAPIAPI.Controllers
{
    [Authorize]
    public class SecureDataController : ApiController
    {
        public IHttpActionResult Get()
        {
            return Ok(new { secureData = "You have to be authenticated to access this!" });
        }
    }
}
