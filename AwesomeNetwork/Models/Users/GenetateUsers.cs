using System;
using System.Collections.Generic;

namespace AwesomeNetwork.Models.Users
{
    public class GenetateUsers
    {
        private string[] firstNames;
        private string[] lastNames;

        public GenetateUsers()
        {

            firstNames = new string[] { "Rufus", "Bear", "Dakota", "Fido",
                          "Vanya", "Samuel", "Koani", "Volodya",
                          "Prince", "Yiska" };
            lastNames = new string[] { "Maggie", "Penny", "Saya", "Princess",
                            "Abby", "Laila", "Sadie", "Olivia",
                            "Starlight", "Talla" };
        }

        public List<User> Populate(int num)
        {
            Random rnd = new Random();
            List<User> users = new List<User>();

            for (int i = 0; i < num; i++)
            {
                int fIndex = rnd.Next(firstNames.Length);
                int lIndex = rnd.Next(lastNames.Length);
                string email = $"test{i}@test.com";

                var newUser = new User { FirstName = firstNames[fIndex], LastName = lastNames[lIndex], UserName = email, Email = email, BirthDate = new DateTime(2008, 3, 1) };
                users.Add(newUser);
            }

            return users;
        }
    }
}