using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security;
using OAuthTest.Models;
using System.Linq;

namespace OAuthTest.Controllers {

    [RoutePrefix("rest/account")]
    public class AccountController : ApiController {

        IAuthenticationManager AuthManager => Request.GetOwinContext().Authentication;

        [HttpGet, Route("external-login")]
        public IHttpActionResult GetExternalLogins() {
            var externalLogins = AuthManager.GetExternalAuthenticationTypes();
            return Ok(externalLogins);
        }

        [HttpPost, Route("external-login")]
        public IHttpActionResult ExternalLogin([FromBody]ExternalLoginModel model) {
            var properties = new AuthenticationProperties();
            var redirectUri = RequestContext.Url.Route("external-login-callback", null);
            properties.RedirectUri = redirectUri;
            AuthManager.Challenge(properties, model.Provider);
            return StatusCode(HttpStatusCode.Unauthorized);
        }

        [HttpGet, Route("external-login-callback", Name = "external-login-callback")]
        public IHttpActionResult ExternalLoginCallback() {
            var error = Request.GetQueryNameValuePairs()
                               .FirstOrDefault(pair => pair.Key == "error");
            var info = AuthManager.GetExternalLoginInfo();
            if (info == null) {
                return BadRequest("External Login Error!");
            }
            var name = info.ExternalIdentity.Name;
            var isAuth = info.ExternalIdentity.IsAuthenticated;
            var msg = $"User: {name} , is authenticated: {isAuth}";
            return Ok(msg);
        }

    }

}
