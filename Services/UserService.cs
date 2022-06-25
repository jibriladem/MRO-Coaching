using MROCoatching.DataObjects.Helper;
using MROCoatching.DataObjects.Models.Operational;
using MROCoatching.DataObjects.Models.UserManagment.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace MROCoatching.DataObjects.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password, ApplicationUser user);
        IEnumerable<User> GetAll();
    }

    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private List<User> _users = new List<User>
        {
            new User { Id = 1, FirstName = "Test", LastName = "User", Username = "test", Password = "test" }
        };

        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public User Authenticate(string username, string password, ApplicationUser user)
        {
            var userTokenVar = new User { Id = 2, FirstName = user.UserName, LastName = user.UserName, Username = user.UserName, Password = user.PasswordHash };
            var userToken = userTokenVar;
            //userToken.FirstName = user.UserName;
            //userToken.Password = user.PasswordHash;
            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            userToken.Token = tokenHandler.WriteToken(token);

            // remove password before returning
            userToken.Password = null;

            return userToken;
        }

        public IEnumerable<User> GetAll()
        {
            // return users without passwords
            return _users.Select(x =>
            {
                x.Password = null;
                return x;
            });
        }
    }
}
