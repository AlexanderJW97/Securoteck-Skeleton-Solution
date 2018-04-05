using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using SecuroteckWebApplication.Models;

namespace SecuroteckWebApplication.Controllers
{
    public class APIAuthorisationHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync (HttpRequestMessage  request, CancellationToken cancellationToken)
       {
            #region Task5
            // TODO:  Find if a header ‘ApiKey’ exists, and if it does, check the database to determine if the given API Key is valid
            //        Then authorise the principle on the current thread using a claim, claimidentity and claimsprinciple
            #endregion
            if (request.Headers.Contains("ApiKey"))
            {
                string apiKeyHeaderValue = request.Headers.GetValues("ApiKey").FirstOrDefault();
                if (apiKeyHeaderValue != null && UserDatabaseAccess.checkUserKey(apiKeyHeaderValue))
                {

                    if (UserDatabaseAccess.checkUserKey(apiKeyHeaderValue))
                    {

                        User user = UserDatabaseAccess.checkUserRtnUsr(apiKeyHeaderValue);
                        ClaimsPrincipal claimPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                        {
                        new Claim(ClaimTypes.Name, user.UserName, ClaimValueTypes.String)
                        }, "ApiKey"));


                        Thread.CurrentPrincipal = claimPrincipal;
                    }
                }
            }


            return base.SendAsync(request, cancellationToken);
        }
    }
}