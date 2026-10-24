#:sdk Microsoft.NET.Sdk.Web

using Microsoft.AspNetCore.Identity;

var password = "";
if (Console.IsInputRedirected)
{
    password = Console.ReadLine() ?? "";
}
else
{
    Console.Error.Write("Nhập mật khẩu admin (ít nhất 12 ký tự): ");
    while (true)
    {
        var key = Console.ReadKey(intercept: true);
        if (key.Key == ConsoleKey.Enter) break;
        if (key.Key == ConsoleKey.Backspace)
        {
            if (password.Length > 0) password = password[..^1];
        }
        else if (!char.IsControl(key.KeyChar)) password += key.KeyChar;
    }
    Console.Error.WriteLine();
}

if (password.Length < 12 || password.Length > 256)
{
    Console.Error.WriteLine("Mật khẩu phải có từ 12 đến 256 ký tự.");
    return 1;
}

Console.WriteLine(new PasswordHasher<string>().HashPassword("admin", password));
return 0;
