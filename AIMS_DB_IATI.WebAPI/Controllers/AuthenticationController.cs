using AIMS_BD_IATI.WebAPI.Models.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AIMS_BD_IATI.WebAPI.Controllers
{
    public class AuthenticationController : ApiController
    {
        public IMembershipService MembershipService
        {
            get;
            private set;
        }

        [Route("authenticate")]
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult Authenticate(AuthenticateViewModel viewModel)
        {
            //Check Security Service
            MembershipService = new AccountMembershipService();
            if (MembershipService.ValidateUser(viewModel.Username, viewModel.Password))
            {
                return Ok(new { success = true });
            }

            return Ok(new { success = false, message = "User Id or password is incorrect" });
        }
    }
}
