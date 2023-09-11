using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoApp.BL.BLModels;
using VideoApp.BL.DALModels;

namespace VideoApp.BL.Mapping
{
    public static class UserMapper
    {
        public static IEnumerable<User> MapToDal(IEnumerable<BLUser> blUsers) =>
          blUsers.Select(x => MapToDal(x));

        public static User MapToDal(BLUser blUser)
        {
            return new User
            {
                Id = blUser.Id,
                CreatedAt = blUser.CreatedAt,
                DeletedAt = blUser.DeletedAt,
                Username = blUser.Username,
                FirstName = blUser.FirstName,
                LastName = blUser.LastName,
                Email = blUser.Email,
                Phone = blUser.Phone,
                IsConfirmed = blUser.IsConfirmed,
                SecurityToken = blUser.SecurityToken,
                CountryOfResidenceId = blUser.CountryOfResidenceId    
            };
        }
        public static IEnumerable<BLUser> MapToBl(IEnumerable<User> users) =>
           users.Select(x => MapToBl(x));

        public static BLUser MapToBl(User dalUser)
        {
            return new BLUser
            {
                Id = dalUser.Id,
                CreatedAt = dalUser.CreatedAt,
                DeletedAt = dalUser.DeletedAt,
                Username = dalUser.Username,
                FirstName = dalUser.FirstName,
                LastName = dalUser.LastName,
                Email = dalUser.Email,
                Phone = dalUser.Phone,
                IsConfirmed = dalUser.IsConfirmed,
                SecurityToken = dalUser.SecurityToken,
                CountryOfResidenceId = dalUser.CountryOfResidenceId
            };
        }
    }
}
