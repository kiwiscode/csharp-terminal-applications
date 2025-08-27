public class UserUtils
{
    public static bool CheckIfUserExist(string username, List<User>? userList)
    {
        if (userList is null) return false;
        return userList.Any(u => u.Username?.ToLower() == username.ToLower());
    }
}