using System.Collections.Concurrent;
using TestTask.API.Helper;
using TestTask.API.Services;

namespace TestTask.API.Authorization
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ConcurrentDictionary<string, RequestCounter> _requestCounters = new ConcurrentDictionary<string, RequestCounter>();
        private class RequestCounter
        {
            public int Count { get; set; }
            public DateTime StartTime { get; set; }
        }

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUserService userService, IJwtHelper jwtHelper)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = jwtHelper.ValidateJwtToken(token);
            if (userId != null)
            {
                context.Items["User"] = userService.GetUserById(userId.Value);
                var userKey = userId.Value.ToString();
                var counter = _requestCounters.GetOrAdd(userKey, new RequestCounter { StartTime = DateTime.UtcNow });

                lock (counter)
                {
                    if ((DateTime.UtcNow - counter.StartTime).TotalMinutes > 1)
                    {
                        counter.StartTime = DateTime.UtcNow;
                        counter.Count = 1;
                    }
                    else
                    {
                        counter.Count++;
                    }
                }

                if (counter.Count > 10)
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.Response.WriteAsync("Too many requests. Please try again later.");
                    return;
                }
            }

            await _next(context);
        }
    }
}
