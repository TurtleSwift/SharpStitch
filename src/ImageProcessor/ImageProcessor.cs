using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using Microsoft.Extensions.Logging;

namespace SharpStitch;

internal class ImageProcessor
{
    private readonly ILogger<ImageProcessor> logger;

    public ImageProcessor(ILogger<ImageProcessor> logger)
    {
        this.logger = logger;
    }

    public void StitchImages(string imgPath1, string imgPath2, string outputPath, ImageProcessOptions processOptions)
    {
        using var img1 = Image.Load<Rgba32>(imgPath1);
        using var img2 = Image.Load<Rgba32>(imgPath2);
        var frameSize = processOptions.BorderSize;

        if (processOptions.Rotate > 0)
        {
            img1.Mutate(o => o.Rotate(processOptions.Rotate));
            img2.Mutate(o => o.Rotate(processOptions.Rotate));
        }

        var width = img1.Width + img2.Width + (frameSize * 3);
        var height = Math.Max(img1.Height, img2.Height) + (frameSize * 2);

        using var stitchedImage = new Image<Rgba32>(width, height, Color.White);
        stitchedImage.Mutate(ctx => ctx
            .DrawImage(img1, new Point(frameSize, frameSize), 1f)
            .DrawImage(img2, new Point(img1.Width + (frameSize * 2), frameSize), 1f));

        if (processOptions.Scale != 1.0f)
        {
            var scaledWidth = (int)(width * processOptions.Scale);
            var scaledHeight = (int)(height * processOptions.Scale);

            stitchedImage.Mutate(ctx => ctx.Resize(scaledWidth, scaledHeight));
        }

        var savedImagePath = GetStitchedFilePath(imgPath1, imgPath2, outputPath, processOptions.SavedImageExtension);
        stitchedImage.Save(savedImagePath);

        logger.LogInformation("Image {path1} and {path2} successfully stitched to {outputImage}.",
            Path.GetFileNameWithoutExtension(imgPath1),
            Path.GetFileNameWithoutExtension(imgPath2),
            Path.GetFileNameWithoutExtension(savedImagePath));
    }

    private string GetStitchedFilePath(string origPath1, string origPath2, string outputPath, string extension)
    {
        var fileName1 = Path.GetFileNameWithoutExtension(origPath1);
        var fileName2 = Path.GetFileNameWithoutExtension(origPath2);
        var extWithoutPeriod = extension.Replace(".", string.Empty).Trim();

        return Path.Combine(outputPath, $"{fileName1}_{fileName2}.{extWithoutPeriod}");
    }
}