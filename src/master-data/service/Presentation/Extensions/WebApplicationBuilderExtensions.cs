namespace MasterData.Api.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void SetupConfiguration(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        webApplicationBuilder.Configuration.AddEnvironmentVariables();
    }
}
