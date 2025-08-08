namespace HasatEmlak.Middleware
{
    public class AdminAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AdminAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();

            // Admin area kontrol et
            if (path.StartsWith("/admin") && !path.StartsWith("/admin/auth"))
            {
                var isLoggedIn = context.Session.GetString("AdminLoggedIn");
                if (isLoggedIn != "true")
                {
                    context.Response.Redirect("/Admin/Auth/Login");
                    return;
                }
            }

            await _next(context);
        }
    }
}