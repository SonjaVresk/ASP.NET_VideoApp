using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoApp.BL.BLModels;
using VideoApp.BL.DALModels;

namespace VideoApp.BL.Repositories
{
    public interface IUserRepository
    {
        //Za MVC
        IEnumerable<BLUser> GetAll();
        BLUser GetUser(int id);
        IEnumerable<BLUser> GetUsers(string filterFirstName, string filterLastName, string filterUserName, string filterCountry);
        BLUser CreateUser(string username, string firstName, string lastName, string email, string phone, string password, int country);
        void ConfirmEmail(string email, string securityToken);
        BLUser GetConfirmedUser(string username, string password);
        void ChangePassword(string username, string newPassword);
        void SoftDeleteUser(int userId);
        bool CheckUsernameExists(string username);
        bool CheckEmailExists(string email);


        //Za API
        User Add(UserRegisterRequest request);
        void ValidateEmailAPI(string email, string token);
        Tokens JwtTokens(JwtTokensRequest request);
        void ChangePassword(ChangePasswordRequest request);
    }
}
