using System.Reflection;

namespace MilesManager.Application.Email;

public sealed class EmailTemplateService
{
    private const string TemplateNamespace = "MilesManager.Application.Email.Templates";

    public async Task<string> RenderAsync(string templateName, IReadOnlyDictionary<string, string> values, CancellationToken cancellationToken = default)
    {
        var resourceName = $"{TemplateNamespace}.{templateName}.html";
        await using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Email template '{templateName}' was not found.");

        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync(cancellationToken);

        foreach (var value in values)
        {
            content = content.Replace($"{{{{{value.Key}}}}}", value.Value, StringComparison.Ordinal);
        }

        return content;
    }
}
