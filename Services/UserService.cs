using Employee_Api.Identity;
using Employee_Api.ServiceContract;
using Employee_Api.ViewModel;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Employee_Api.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationUserManager _applicationUserManager;
        private readonly ApplicationSignInManager _applicationSignInManager;
        private readonly AppSettings _appSettings;
        public UserService(ApplicationUserManager applicationUserManager,
                      ApplicationSignInManager applicationSignInManager, IOptions<AppSettings> appSettings)
        {
            _applicationSignInManager = applicationSignInManager;
            _applicationUserManager = applicationUserManager;
            _appSettings = appSettings.Value;
        }
        public async Task<ApplicationUser> Authenticate(Loginviewmodel loginviewmodel)
        {
            var result = await _applicationSignInManager.PasswordSignInAsync
         (loginviewmodel.Username, loginviewmodel.Password, false, false);
            if (result.Succeeded)
            {
                var applicationUser = await _applicationUserManager.
                  FindByNameAsync(loginviewmodel.Username);
                applicationUser.PasswordHash = "";

                //JWT Token
                if (await _applicationUserManager.IsInRoleAsync(applicationUser, SD.Role_Admin))
                    applicationUser.Role = SD.Role_Admin;
                //
                if (await _applicationUserManager.IsInRoleAsync(applicationUser, SD.Role_Employee))
                    applicationUser.Role = SD.Role_Employee;
                var tokenhandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, applicationUser.Id),
                    new Claim(ClaimTypes.Email, applicationUser.Email),
                    new Claim(ClaimTypes.Role,applicationUser.Role)
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key)
,
                    SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenhandler.CreateToken(tokenDescriptor);
                applicationUser.Token = tokenhandler.WriteToken(token);
                applicationUser.PasswordHash = "";


                return applicationUser;
            }
            else
            {
                return null;
            }
        }
    }
}
