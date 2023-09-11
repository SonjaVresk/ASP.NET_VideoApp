using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;
using VideoApp.BL.BLModels;
using VideoApp.BL.DALModels;
using VideoApp.BL.Mapping;

namespace VideoApp.BL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly IMapper _mapper;        
        private readonly IConfiguration _configuration;

        public UserRepository(RwaMoviesContext dbContext, IMapper mapper, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _configuration = configuration;
        }

        private static (byte[], string) GenerateSalt()
        {
            // Generate salt
            var salt = RandomNumberGenerator.GetBytes(128 / 8);
            var b64Salt = Convert.ToBase64String(salt);

            return (salt, b64Salt);
        }

        private static string CreateHash(string password, byte[] salt)
        {
            // Create hash from password and salt
            byte[] hash =
                KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8);
            string b64Hash = Convert.ToBase64String(hash);

            return b64Hash;
        }

        private static string GenerateSecurityToken()
        {
            byte[] securityToken = RandomNumberGenerator.GetBytes(256 / 8);
            string b64SecToken = Convert.ToBase64String(securityToken);

            return b64SecToken;
        }

        public IEnumerable<BLUser> GetAll()
        {
            //Dohvaćanje kolekcije iz baze:
            var dbUsers = _dbContext.Users.Include("CountryOfResidence");

            //Pretvaranje user-a iz baze u user-a u BL automapperom
            var blUsers = _mapper.Map<IEnumerable<BLUser>>(dbUsers);
            return blUsers;
        }

        public IEnumerable<BLUser> GetUsers(string filterFirstName, string filterLastName, string filterUserName, string filterCountry)
        {
            //Dohvaćanje kolekcije iz baze:
            IEnumerable<User> dbUsers = _dbContext.Users.Include("CountryOfResidence").AsEnumerable();

            if (filterFirstName != null)
            {
                dbUsers = dbUsers.ToList().
                Where(x => x.FirstName.Contains(filterFirstName)).ToList();
            }

            if (filterLastName != null)
            {
                dbUsers = dbUsers.ToList().
                Where(x => x.LastName.Contains(filterLastName)).ToList();
            }

            if (filterUserName != null)
            {
                dbUsers = dbUsers.ToList().
                Where(x => x.Username.Contains(filterUserName)).ToList();
            }
            
            if (filterCountry != null)
            {
                dbUsers = dbUsers.ToList().
                Where(x => x.CountryOfResidence.Name.Contains(filterCountry)).ToList();
            }

            //Pretvaranje user-a iz baze u user-a u BL automapperom
            var blUsers = _mapper.Map<IEnumerable<BLUser>>(dbUsers);

            //Naziv države
            foreach (var blUser in blUsers)
            {
                blUser.CountryOfResidence = _dbContext.Countries
                    .FirstOrDefault(country => country.Id == blUser.CountryOfResidenceId)?.Name;
            }

            return blUsers;
        }

        public BLUser GetUser(int id)
        {
            var dbUser = _dbContext.Users.Include("CountryOfResidence").FirstOrDefault(x => x.Id == id);
            var blUser = _mapper.Map<BLUser>(dbUser);

            //Naziv države

            blUser.CountryOfResidence = _dbContext.Countries
                .FirstOrDefault(country => country.Id == blUser.CountryOfResidenceId)?.Name;


            return blUser;
        }

        public BLUser CreateUser(string username, string firstName, string lastName, string email, string phone, string password, int country)
        {
            (var salt, var b64Salt) = GenerateSalt();   //Kreira random salt, kodirano b64 enkripcijom
            var b64Hash = CreateHash(password, salt);   //Na temelju salta i passworda kreira se hash
            var b64SecToken = GenerateSecurityToken();  //Generira se token za potvrdu e-mail-a

            var dbUser = new User 
            {
                Username = username,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone,
                PwdHash = b64Hash,
                PwdSalt = b64Salt,
                SecurityToken = b64SecToken,               
                CreatedAt = DateTime.UtcNow,
                CountryOfResidenceId = country
  
        };
            
            _dbContext.Users.Add(dbUser);
            _dbContext.SaveChanges();

            var blUser = UserMapper.MapToBl(dbUser);

            return blUser;
        }

        public void ConfirmEmail(string email, string securityToken) 
        { 
            var dbUser = _dbContext.Users.FirstOrDefault(x => x.Email == email && 
                x.SecurityToken == securityToken &&
                x.DeletedAt == null);

            dbUser.IsConfirmed = true; 

            _dbContext.SaveChanges();
        }

        public BLUser GetConfirmedUser(string username, string password)
        {
            var dbUser = _dbContext.Users.FirstOrDefault(x => x.Username == username &&
                    x.IsConfirmed == true &&
                    x.DeletedAt == null);

            if (dbUser == null)
                return null;

            var salt = Convert.FromBase64String(dbUser.PwdSalt);
            var b64Hash = CreateHash(password, salt);

            if (dbUser.PwdHash != b64Hash)
                return null;

            var blUser = UserMapper.MapToBl(dbUser);

            return blUser;
        }

        public void ChangePassword(string username, string newPassword)
        {
            var userToChangePassword = _dbContext.Users.FirstOrDefault(x =>
                x.Username == username &&
                x.DeletedAt == null);

            (var salt, var b64Salt) = GenerateSalt();

            var b64Hash = CreateHash(newPassword, salt);

            userToChangePassword.PwdHash = b64Hash;
            userToChangePassword.PwdSalt = b64Salt;

            _dbContext.SaveChanges();
        }

        public void SoftDeleteUser(int userId)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.DeletedAt = DateTime.UtcNow;
                _dbContext.SaveChanges();
            }
        }

        public bool CheckUsernameExists(string username)
           => _dbContext.Users.Any(x => x.Username == username && x.DeletedAt == null);

        public bool CheckEmailExists(string email)
            => _dbContext.Users.Any(x => x.Email == email && x.DeletedAt == null);


        //Za API:
        public User Add(UserRegisterRequest request)
        {
            var dbUsers = _dbContext.Users.Include("CountryOfResidence");
            var blUsers = UserMapper.MapToBl(dbUsers);

            // Username: Normalize and check if username exists
            var normalizedUsername = request.Username.ToLower().Trim();
            if (dbUsers.Any(x => x.Username.Equals(normalizedUsername)))
                throw new InvalidOperationException("Username already exists");

            // Password: Salt and hash password
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8); // divide by 8 to convert bits to bytes
            string b64Salt = Convert.ToBase64String(salt);

            byte[] hash =
                KeyDerivation.Pbkdf2(
                    password: request.Password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8);
            string b64Hash = Convert.ToBase64String(hash);

            // SecurityToken: Random security token
            byte[] securityToken = RandomNumberGenerator.GetBytes(256 / 8);
            string b64SecToken = Convert.ToBase64String(securityToken);


            // New user
            var newUser = new User
            {                
                Username = request.Username,
                Email = request.Email,
                FirstName = request.FirstName, 
                LastName = request.LastName,
                Phone = request.Phone,
                CountryOfResidenceId = request.CountryOfResidenceId,
                IsConfirmed = false,
                SecurityToken = b64SecToken,
                PwdSalt = b64Salt,
                PwdHash = b64Hash
                
            };
           
            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();

            return newUser;
        }
        public void ValidateEmailAPI(string email, string token)
        {
            var dbUsers = _dbContext.Users;          

            var target = dbUsers.FirstOrDefault(x =>
                x.Email == email && x.SecurityToken == token);

            if (target == null)
                throw new InvalidOperationException("Authentication failed");

            target.IsConfirmed = true;

            _dbContext.SaveChanges();

        }

        private bool Authenticate(string username, string password)
        {
            var dbUsers = _dbContext.Users;           

            var target = dbUsers.FirstOrDefault(x => x.Username == username);

            if (!target.IsConfirmed)
                return false;

            // Get stored salt and hash
            byte[] salt = Convert.FromBase64String(target.PwdSalt);
            byte[] hash = Convert.FromBase64String(target.PwdHash);

            byte[] calcHash =
                KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8);

            return hash.SequenceEqual(calcHash);
        }

        public Tokens JwtTokens(JwtTokensRequest request)
        {
            var isAuthenticated = Authenticate(request.Username, request.Password);

            if (!isAuthenticated)
                throw new InvalidOperationException("Authentication failed");

            // Get secret key bytes
            var jwtKey = _configuration["JWT:Key"];
            var jwtKeyBytes = Encoding.UTF8.GetBytes(jwtKey);

            // Create a token descriptor (represents a token, kind of a "template" for token)
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new System.Security.Claims.Claim[]
                {
                    new System.Security.Claims.Claim(ClaimTypes.Name, request.Username),
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, request.Username),
                    //new System.Security.Claims.Claim(ClaimTypes.Role, "User")
                }),
                Issuer = _configuration["JWT:Issuer"],
                Audience = _configuration["JWT:Audience"],
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(jwtKeyBytes),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            // Create token using that descriptor, serialize it and return it
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var serializedToken = tokenHandler.WriteToken(token);

            return new Tokens
            {
                Token = serializedToken
            };
        }

        public void ChangePassword(ChangePasswordRequest request)
        {
            var dbUser = _dbContext.Users;

            var isAuthenticated = Authenticate(request.Username, request.OldPassword);

            if (!isAuthenticated)
                throw new InvalidOperationException("Authentication failed");

            // Salt and hash pwd
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8); // divide by 8 to convert bits to bytes
            string b64Salt = Convert.ToBase64String(salt);

            byte[] hash =
                KeyDerivation.Pbkdf2(
                    password: request.NewPassword,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8);
            string b64Hash = Convert.ToBase64String(hash);

            // Update user
            var target = dbUser.Single(x => x.Username == request.Username);
            target.PwdSalt = b64Salt;
            target.PwdHash = b64Hash;
        }
    }
}
