using AutoMapper;
using HYDB.Services.DTO;
using HYDB.Services.Models;
using HYDB.Services.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace HYDB.Services.Services
{
    public class UserAccountInfo : IUserAccountInfo
    {
        private readonly UserAccounts _userAccountRepository;
        private readonly IMapper _mapper;

        public UserAccountInfo(IMapper mapper, IConfiguration config)
        {
            _userAccountRepository = new UserAccounts(config);
            _mapper = mapper;
        }

        public Response AuthenticateUser(UserAccountPayload userCredentials)
        {
            var matchedUser = _userAccountRepository.GetByUsername(userCredentials.UserName).FirstOrDefault();

            if (matchedUser != null)
            {
                if (!BC.Verify(userCredentials.Password, matchedUser.Password))
                {
                    return new Response() { IsSuccess = false, Message = "Incorrect password" };
                }
                else
                {
                    return new Response() { IsSuccess = true };
                }
            }
            else
            {
                return new Response() { IsSuccess = false, Message = "User doesn't exist" };
            }
        }

        public Response CreateAccount(UserAccountPayload newUserAccountDetails)
        {
            if (CheckIfUserExists(newUserAccountDetails.UserName))
            {
                return new Response() { IsSuccess = false, Message = "Username already exists" };
            }
            else
            {
                var newAccountModel = _mapper.Map<UserAccount>(newUserAccountDetails);

                var hashedPassword = BC.HashPassword(newAccountModel.Password);
                newAccountModel.Password = hashedPassword;

                newAccountModel.Id = Guid.NewGuid().ToString("N").ToUpper();
                newAccountModel.IsEmailVerified = false;
                newAccountModel.IsActive = true;
                newAccountModel.CreationDate = DateTime.UtcNow;
                _userAccountRepository.AddUser(newAccountModel);
                return new Response() { IsSuccess = true, Message = "User created successfully" };
            }
        }

        private bool CheckIfUserExists(string userName)
        {
            var availableUsers = _userAccountRepository.GetByUsername(userName);
            return (availableUsers.Count() > 0) ? true : false;
        }
    }
}
