using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserProfile.Data;
using UserProfile.Dto.Request;
using UserProfile.Entities;


namespace UserProfile.Services
{
    public class AuthService(AppDbContext context) : IAuthService
    {
        public async Task<User> RegisterAsync(RegisterRequestDto request)
        {
            // Check if the user already exists
            var existingUser = await context.Users.AnyAsync(user => user.email == request.email);
            if (existingUser)
            {
                throw new Exception("User already exists");
            }

            // Check if the user password and confirm password match
            if (request.password != request.confirmPassword)
            {
                throw new Exception("Password are not macht");
            }

            // if not exits create new User
            var user = new User();

            // hash password
            var hashPassword = new PasswordHasher<User>().HashPassword(user, request.password);

            // set the user properties
            user.firstname = request.firstname;
            user.lastname = request.lastname;
            user.email = request.email;
            user.password = hashPassword;


            // set use to db
            await context.Users.AddAsync(user);
            // save db changes
            await context.SaveChangesAsync();

            // return user
            return user;
        }


        public async Task<User> LoginAsync(LoginRequestDto request)
        {
            // Check if the user exists
            var user = await context.Users.FirstOrDefaultAsync(u => u.email == request.email);
            if (user is null)
            {
                throw new Exception("User does not exist");
            }

            // verify password
            var passwordVerificationResult = new PasswordHasher<User>().VerifyHashedPassword(user, user.password, request.password);
            if(passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                throw new Exception("Invalid password");
            }

            // return user
            return user;
        }
    }
}
