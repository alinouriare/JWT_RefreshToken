using ApiProduct.Mdoels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace ApiProduct.Security
{
    public class AccessManager
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private SigningConfigurations _signingConfigurations;
        private TokenConfigurations _tokenConfigurations;
        private IDistributedCache _cache;

        public AccessManager(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            SigningConfigurations signingConfigurations,
            TokenConfigurations tokenConfigurations,
            IDistributedCache cache)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _signingConfigurations = signingConfigurations;
            _tokenConfigurations = tokenConfigurations;
            _cache = cache;
        }


        public bool ValidateCredentials(AccessCredentials credencial)
        {

            bool credentialValidas = false;
            if (credencial !=null && !string.IsNullOrWhiteSpace(credencial.UserID))
            {
                if (credencial.GrantType== "password")
                {
                    var userIdentity = _userManager.FindByNameAsync(credencial.UserID).Result;
                    if (userIdentity !=null)
                    {
                        var resultadoLogin = _signInManager.CheckPasswordSignInAsync(userIdentity, credencial.Password, false).Result;
                        if (resultadoLogin.Succeeded)
                        {
                            credentialValidas = _userManager.IsInRoleAsync(userIdentity, Roles.ROLE_API_PRODUCT).Result;
                        }
                    }
                }
                else if (credencial.GrantType == "refresh_token")
                {
                    if (!String.IsNullOrWhiteSpace(credencial.RefreshToken))
                    {
                        RefreshTokenData refreshTokenBase = null;


                        string strTokenStored = _cache.GetString(credencial.RefreshToken);
                        if (!String.IsNullOrWhiteSpace(strTokenStored))
                        {
                            refreshTokenBase = JsonConvert
                                .DeserializeObject<RefreshTokenData>(strTokenStored);
                        }

                        credentialValidas = (refreshTokenBase != null &&
                               credencial.UserID == refreshTokenBase.UserID &&
                               credencial.RefreshToken == refreshTokenBase.RefreshToken);

                        if (credentialValidas)
                            _cache.Remove(credencial.RefreshToken);
                    }
                }
            }
       

            return credentialValidas;
        }


        public Token GenerateToken(AccessCredentials credencial)
        {
            ClaimsIdentity identity = new ClaimsIdentity(
             new GenericIdentity(credencial.UserID, "Login"),
             new[] {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                        new Claim(JwtRegisteredClaimNames.UniqueName, credencial.UserID)
             }
         );


            DateTime dateTime = DateTime.Now;
            DateTime dateTimeExpire = dateTime + TimeSpan.FromSeconds(_tokenConfigurations.Seconds);
            var handler = new JwtSecurityTokenHandler();

            var securityToken = handler.CreateToken(new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
            {
                Issuer = _tokenConfigurations.Issuer,
                Audience = _tokenConfigurations.Audience,
                SigningCredentials = _signingConfigurations.SigningCredentials,
                Subject=identity,
                NotBefore=dateTime,
                Expires=dateTimeExpire

            }) ;

            var token = handler.WriteToken(securityToken);

            var result = new Token() {

                Authenticated = true,
                Created = dateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                Expiration = dateTimeExpire.ToString("yyyy-MM-dd HH:mm:ss"),
                AccessToken = token,
                RefreshToken = Guid.NewGuid().ToString().Replace("-", String.Empty),
                Message = "OK"

            };

            var refreshTokenData = new RefreshTokenData();

            refreshTokenData.RefreshToken = result.RefreshToken;
            refreshTokenData.UserID = credencial.UserID;


            TimeSpan finalExpiration =
             TimeSpan.FromSeconds(_tokenConfigurations.FinalExpiration);


            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();

            options.SetAbsoluteExpiration(finalExpiration);
            _cache.SetString(result.RefreshToken, JsonConvert.SerializeObject(refreshTokenData), options);
            return result;
        }
    }
}
