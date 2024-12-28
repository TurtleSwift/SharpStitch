using Microsoft.Extensions.Logging;

namespace SharpStitch.Options;

static class AppOptionsValidator
{
    public static bool ValidateOptions(AppOptions appOptions, ILogger logger)
    {
        var errors = new List<string>();

        if (appOptions.Rotation < 0 || appOptions.Rotation > 359)
            errors.Add("Rotation should be between 0 and 359.");
        if (appOptions.Scale < 0.1f || appOptions.Scale > 2.0f)
            errors.Add("Scale should be between 0.1 and 2.0.");
        if (appOptions.BorderSize < 0)
            errors.Add("Border size should not be a negative number.");

        foreach (var error in errors)
            logger.LogError(error);

        return errors.Count == 0;
    }
}