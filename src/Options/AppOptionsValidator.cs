using Microsoft.Extensions.Logging;

namespace SharpStitch.Options;

static class AppOptionsValidator
{
    public static bool ValidateOptions(AppOptions appOptions, ILogger logger, int imageCount)
    {
        var errors = new List<string>();

        var wXh = appOptions.GetNumOfColumnsAndRows(imageCount);

        if (appOptions.Files?.Count() < 2 && string.IsNullOrEmpty(appOptions.InputPath))
            errors.Add("An input path must be provided. Either provide at least two input images or an input directory.");
        if (imageCount == 1)
            errors.Add("Image count should be larger than 1.");
        if (appOptions.Rotation > 359)
            errors.Add("Rotation should be between 0 and 359.");
        if (appOptions.Scale < 0.1f || appOptions.Scale > 2.0f)
            errors.Add("Scale should be between 0.1 and 2.0.");
        if (wXh.Rows < 1)
            errors.Add("Height should be at least 1.");
        if (wXh.Columns < 1)
            errors.Add("Width should be at least 1.");
        if (wXh.Rows == 1 && wXh.Columns == 1)
            errors.Add("Width and height set to 1 results in a 1x1 grid. Either one should be larger than 1.");

        foreach (var error in errors)
            logger.LogError(error);

        return errors.Count == 0;
    }
}