using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiProduct.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiProduct.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost]
        public object Post(
            [FromBody] AccessCredentials credenciais,
            [FromServices] AccessManager accessManager)
        {
            if (accessManager.ValidateCredentials(credenciais))
            {
                return accessManager.GenerateToken(credenciais);
            }
            else
            {
                return new
                {
                    Authenticated = false,
                    Message = "Failed to authenticate"

                };
            }
        }
    }
}
