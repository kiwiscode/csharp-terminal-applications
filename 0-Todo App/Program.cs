using static Globals;

static class Program
{
    static void Main()
    {



        if (!File.Exists("users.json"))
        // Mock users
        {
            UserList.AddRange(new List<User>
        {
            new User { Username = "John", Password = PasswordUtils.HashPassword("1234") },
            new User { Username = "Jane", Password = PasswordUtils.HashPassword("5678") },
            new User { Username = "Mike", Password = PasswordUtils.HashPassword("abcde") }
        });
            // Save users
            UserDataUtils.SaveUsers(UserList);

        }


        bool repeatAuthModeQuestion = false;

        do
        {
            Console.Write("Already using Todo App? Log in [Y/N]: ");

            authMode = Console.ReadLine();

            if (!string.IsNullOrEmpty(authMode))
            {
                if (string.Equals(authMode, "y", StringComparison.CurrentCultureIgnoreCase))
                {
                    do
                    {
                        authMode = "login";

                        Console.Write("Your username: ");
                        string? username = Console.ReadLine();
                        Console.Write("Your password: ");
                        string? password = PasswordUtils.ReadPassword();


                        // login
                        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                        {



                            int result = UserController.LoginUser(username, password);

                            if (result == HttpStatus.NOT_FOUND)
                            {
                                ThrowErrorUtils.ThrowError(HttpStatus.NOT_FOUND, "User not found!");
                            }
                            else if (result == HttpStatus.UNAUTHORIZED)
                            {
                                ThrowErrorUtils.ThrowError(HttpStatus.UNAUTHORIZED, "Invalid credentials!");
                            }
                            else if (result == HttpStatus.OK)
                            {
                                repeatAuthModeQuestion = false;
                                authMode = "";
                                using (var progress = new ProgressBar())
                                {
                                    for (int i = 0; i <= 100; i++)
                                    {
                                        progress.Report((double)i / 100);
                                        Thread.Sleep(20);
                                    }
                                }
                                Console.WriteLine($"Welcome to your dashboard {username} 👋");
                            }
                        }

                    } while (authMode == "login");
                }
                else if (string.Equals(authMode, "n", StringComparison.CurrentCultureIgnoreCase))
                {


                    authMode = "register";
                    Console.Write("Please enter your username: ");
                    string? username = Console.ReadLine();
                    Console.Write("Please enter your password: ");
                    string? password = PasswordUtils.ReadPassword();

                    // register
                    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                    {


                        int result = UserController.RegisterUser(username, PasswordUtils.HashPassword(password));


                        if (result == HttpStatus.CONFLICT)
                        {

                            ThrowErrorUtils.ThrowError(HttpStatus.CONFLICT, "User already exists!");
                            repeatAuthModeQuestion = true;
                            authMode = "login";
                        }
                        else if (result == HttpStatus.CREATED)
                        {
                            using (var progress = new ProgressBar())
                            {
                                for (int i = 0; i <= 100; i++)
                                {
                                    progress.Report((double)i / 100);
                                    Thread.Sleep(20);
                                }
                            }
                            Console.WriteLine($"Welcome to your dashboard {username} 👋");
                            repeatAuthModeQuestion = false;
                        }
                    }

                }
                else
                {
                    Console.WriteLine("Please type Y for Yes or N for No:");
                    authMode = "";
                    repeatAuthModeQuestion = true;
                }
            }
            else
            {
                Console.WriteLine("Please type Y for Yes or N for No:");
                authMode = "";
                repeatAuthModeQuestion = true;
            }
        } while (repeatAuthModeQuestion);

        // Dashboard 



        Console.ReadLine();
    }
}