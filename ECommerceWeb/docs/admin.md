# Mobi Admin

- Dashboard: `/admin` (requires the Admin role).
- Login: `/admin/login`.
- Logout: POST `/admin/logout` with an antiforgery token.
- Dashboard reads the current phone catalog. Product editing and persisted orders are not implemented.

## Local account

Run these commands from the repository root. The password helper reads a hidden password and uses ASP.NET Core's built-in PasswordHasher. It requires 12–256 characters.

```sh
dotnet user-secrets set 'Admin:Username' 'admin' --project ECommerceWeb
mobi_password_hash=$(dotnet run scripts/hash-admin-password.cs)
dotnet user-secrets set 'Admin:PasswordHash' "$mobi_password_hash" --project ECommerceWeb
unset mobi_password_hash
dotnet run --project ECommerceWeb --launch-profile http
```

User secrets load automatically in Development and are stored outside the repository. The helper must succeed before saving its output. Restart the app after changing credentials, and sign out existing browser sessions.

## Deployment

Set `Admin__Username` and `Admin__PasswordHash` through the deployment's secret configuration. No default password is supplied. Missing or invalid credentials deny login. Use HTTPS; authentication cookies require it outside Development. If deploying behind a proxy, configure trusted forwarded headers for HTTPS.

Cookies are HttpOnly, SameSite=Lax, non-persistent, and expire after 30 minutes of inactivity. Login attempts are not rate limited. Login/logout use Razor Pages antiforgery validation, redirects accept local URLs only, and admin responses are marked no-store.

Run the focused checks with `MOBI_TEST_ADMIN_USERNAME` and `MOBI_TEST_ADMIN_PASSWORD` in the environment:

```sh
python3 scripts/admin-smoke-test.py http://localhost:5187
```

Platform references: [cookie authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-10.0), [Razor authorization conventions](https://learn.microsoft.com/en-us/aspnet/core/razor-pages/security/authorization/conventions?view=aspnetcore-10.0), [user secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-10.0).
