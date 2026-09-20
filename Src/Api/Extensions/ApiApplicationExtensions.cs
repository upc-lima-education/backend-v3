using Backend.Src.Api.Middleware;

namespace Backend.Src.Api.Extensions;

public static class ApiApplicationExtensions
{
    public static WebApplication UseApi(this WebApplication app)
    {
        app.UseCors("FrontendPolicy");

        app.UseMiddleware<ExceptionsMiddleware>();

        app.UseSwagger();
        app.UseSwaggerUI();

        if (!app.Environment.IsDevelopment())
            app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}
