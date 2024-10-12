using Application.Contracts;
using Application.DTOs;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repo
{
    public class UserRepo : IUser
    {
        private readonly AppDbContext appDbContext;

        public UserRepo(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<LoginRespons> LoginUserAsync(LoginDTO loginDTO)
        {
            var getUser = await FindUserByEmail(loginDTO.Email!);
            if (getUser == null) return new LoginRespons(false, "User not found ");

            var checkPassword = BCrypt.Net.BCrypt.Verify(loginDTO.Password, getUser.Password);
            if (checkPassword)
                return new LoginRespons(true, "Login successfull", GenerateJWTToken(getUser));
            else return new LoginRespons(false, "Invalid credent");
           
        }
        private async Task<ApplicationUser> FindUserByEmail(string email)=>
            await appDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);



        public async Task<RegistrationRespons> RegisterUserAsync(RegisterUserDTO registerUserDTO)
        {
            var getUser = await FindUserByEmail(registerUserDTO.Email!);
            if(getUser != null) return new RegistrationRespons(false, "User already exist");

            appDbContext.Users.Add(new ApplicationUser()
            {
                Name = registerUserDTO.Name,
                Email = registerUserDTO.Email,
                Password = registerUserDTO.Password,
            });

            await appDbContext.SaveChangesAsync();
            return new RegistrationRespons(true, "Registration complited");
        }
    }
}
