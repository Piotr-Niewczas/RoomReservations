using System.Text;
using Microsoft.AspNetCore.Identity;
using RoomReservations.Models;

namespace RoomReservations.Data;

public class UserService(UserManager<ApplicationUser> userManager)
{
    public async Task<ApplicationUser> AddUserAsync(string email, string firstName, string lastName)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName
        };

        var result = await userManager.CreateAsync(user, GeneratePassword());
        if (result.Succeeded)
            await userManager.AddToRoleAsync(user, RoleNames.Client);
        else
            throw new Exception($"Failed to create user {email}. Error: {result.Errors}");

        return user;
    }

    public async Task<ApplicationUser?> FindUserByEmailAsync(string email)
    {
        return await userManager.FindByEmailAsync(email);
    }

    private static string GeneratePassword(int baseLength = 16)
    {
        var random = new Random();
        var password = new StringBuilder();
        for (var i = 0; i < baseLength; i++)
        {
            var randomChar = (char)random.Next(97, 122); // a-z
            password.Append(randomChar);
        }

        var howManyUpperCase = random.Next(1, 4);
        for (var i = 0; i < howManyUpperCase; i++)
        {
            var randomChar = (char)random.Next(65, 90); // A-Z
            var randomIndex = random.Next(0, password.Length);
            password.Insert(randomIndex, randomChar);
        }

        var howManyNumbers = random.Next(1, 4);
        for (var i = 0; i < howManyNumbers; i++)
        {
            var randomChar = (char)random.Next(48, 57); // 0-9
            var randomIndex = random.Next(0, password.Length);
            password.Insert(randomIndex, randomChar);
        }

        var howManySpecialChars = random.Next(1, 4);
        for (var i = 0; i < howManySpecialChars; i++)
        {
            var randomChar = (char)random.Next(33, 47); // !-/
            var randomIndex = random.Next(0, password.Length);
            password.Insert(randomIndex, randomChar);
        }

        return password.ToString();
    }
}