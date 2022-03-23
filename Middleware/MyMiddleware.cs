using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TodoApi2.Middleware
{
    public class MyMiddleware
    {
        private readonly RequestDelegate _next;

        public MyMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            string token = context.Request.Headers["Token"];

            if (token == null || token.Equals("12345") == false)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Wrong Token!");
                return;
            }

            await _next(context);
        }
    }
}