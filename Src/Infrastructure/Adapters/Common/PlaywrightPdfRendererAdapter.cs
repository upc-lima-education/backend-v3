using Backend.Src.Domain.Ports.Common;
using Microsoft.Playwright;

namespace Backend.Src.Infrastructure.Adapters.Common;

public class PlaywrightPdfRendererAdapter : IPdfRendererPort
{
    public async Task<Stream> RenderAsync(string html, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var playwright = await Playwright.CreateAsync();

        var executablePath = Environment.GetEnvironmentVariable("PLAYWRIGHT_CHROMIUM_EXECUTABLE_PATH");
        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = true,
            Args = ["--no-sandbox", "--disable-dev-shm-usage"]
        };
        if (!string.IsNullOrWhiteSpace(executablePath))
            launchOptions.ExecutablePath = executablePath;

        await using var browser = await playwright.Chromium.LaunchAsync(launchOptions);

        var page = await browser.NewPageAsync();

        await page.SetContentAsync(
            html,
            new PageSetContentOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            }
        );

        var pdf = await page.PdfAsync(
            new PagePdfOptions
            {
                Format = "A4",
                PrintBackground = true,
                PreferCSSPageSize = true
            }
        );

        return new MemoryStream(pdf);
    }
}
