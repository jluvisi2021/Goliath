using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Goliath
{
    /*
-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
   _____       _ _       _   _
  / ____|     | (_)     | | | |
 | |  __  ___ | |_  __ _| |_| |__
 | | |_ |/ _ \| | |/ _` | __| '_ \
 | |__| | (_) | | | (_| | |_| | | |
  \_____|\___/|_|_|\__,_|\__|_| |_|
          Password Manager

-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

    -> By Jacob Luvisi
    -> GitHub: https://github.com/jluvisi2021/Goliath
    -> C# 9.0 - .NET Core 5

    Goliath is an ASP.NET Core 5 based password manager based on the
    MVC design pattern.
    This is my first attempt at an ASP.NET Core project.

    Planned Features:
    - User Accounts
    - Encryption
    - Bootstrap CSS (4.6)
    - Microsoft Azure Database (MSSQL)
    - Themes
    - 2 Factor Authentication
    - Categories for secure information

    Bootstrap Layouts:
    -> https://jluvisi2021.github.io/Bubblegum/

    To modify this project and make your own fork of Goliath there is documentation on the Wiki
    -> https://github.com/jluvisi2021/Goliath/wiki
    */

    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}