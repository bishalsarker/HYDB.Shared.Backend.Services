using HYDB.Services.DTO;

namespace HYDB.Services.Services
{
    public interface IUserAccountInfo
    {
        Response AuthenticateUser(UserAccountPayload userCredentials);
        Response CreateAccount(UserAccountPayload newUserAccountDetails);
    }
}