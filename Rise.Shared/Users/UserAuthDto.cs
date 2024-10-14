using System;
namespace Rise.Shared.Users;
public static class UserAuthDto
{
    public class Index
    {
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required bool Blocked { get; set; }
    }
}