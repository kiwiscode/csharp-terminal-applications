using static Globals;

public class UserController
{
    public static int RegisterUser(string username, string password)
    {
        var loadedUsers = UserDataUtils.LoadUsers();
        bool isExist = UserUtils.CheckIfUserExist(username, loadedUsers);

        if (isExist)
        {
            return HttpStatus.CONFLICT;
        }

        var newUser = new User
        {
            Username = username,
            Password = password
        };


        // Add to the list
        loadedUsers.Add(newUser);

        // Add to the json file
        UserDataUtils.SaveUsers(loadedUsers);



        return HttpStatus.CREATED;
    }

    public static int LoginUser(string username, string password)
    {
        var loadedUsers = UserDataUtils.LoadUsers();

        User? user = loadedUsers?.FirstOrDefault(u => u.Username?.ToLower() == username.ToLower());

        if (user == null)
        {
            return HttpStatus.NOT_FOUND;
        }


        bool passwordValid = PasswordUtils.VerifyPassword(password, user.Password);

        if (!passwordValid)
        {
            return HttpStatus.UNAUTHORIZED;
        }



        return HttpStatus.OK;

    }
}