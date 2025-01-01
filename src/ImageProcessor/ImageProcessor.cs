using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using Microsoft.Extensions.Logging;
using System.Data;

namespace SharpStitch;

internal class ImageProcessor
{
    private readonly ILogger<ImageProcessor> logger;

    public ImageProcessor(ILogger<ImageProcessor> logger)
    {
        this.logger = logger;
    }

    public void StitchImages(string[] imagePaths, string outputPath, ImageProcessOptions processOptions)
    {
        // create images and do rotations
        var images = new List<(string ImagePath, Image<Rgba32> Image)>();
        foreach (var imagePath in imagePaths)
        {
            var img = Image.Load<Rgba32>(imagePath);
            
            if (processOptions.Rotate > 0)
                img.Mutate(o => o.Rotate(processOptions.Rotate));

            images.Add((imagePath, img));
        }
        
        // add images to rows and columns dictionary
        var imagesPerRow = new Dictionary<int, List<(string ImagePath, Image<Rgba32> Image)>>();
        var columns = (int)processOptions.Columns;
        var rows = (int)processOptions.Rows;

        for (int r = 0; r < rows; r++)
        {
            if (r * columns > images.Count)
            {
                rows = r; // actual no. of rows is smaller than provided by user
                break; // dont add blank rows (not enough images)
            }

            imagesPerRow.Add(r, []);

            for (int c = 0; c < columns; c++)
            {
                var currentIndex = r * columns + c;
                if (currentIndex >= images.Count)
                    break;

                imagesPerRow[r].Add(images[currentIndex]);
            }
        }

        //calculate width and height of final image based on rows/columns + border
        var widthsAndHeightPerRow = imagesPerRow.ToDictionary
        (
            kvp => kvp.Key,
            kvp => (SumWidths: kvp.Value.Sum(img => img.Image.Width), MaxHeight: kvp.Value.Max(img => img.Image.Height))
        );
        var imagesWidth = widthsAndHeightPerRow.Values.Select(v => v.SumWidths).Max();
        var imagesHeight = widthsAndHeightPerRow.Values.Select(v => v.MaxHeight).Sum();

        var frameSize = processOptions.BorderSize;
        var width = imagesWidth + (frameSize * (columns + 1));
        var height = imagesHeight + (frameSize * (rows + 1));

        //create final image
        using var stitchedImage = new Image<Rgba32>(width, height, Color.White);
        var stitchedImagePaths = new List<string>();

        stitchedImage.Mutate(ctx => 
        {
            foreach (var row in imagesPerRow)
            {
                var currentWidthPos = 0;
                foreach (var img in row.Value)
                {
                    var x = currentWidthPos + frameSize;
                    var y = widthsAndHeightPerRow[row.Key].MaxHeight * row.Key + ((row.Key + 1) * frameSize);  

                    ctx.DrawImage(img.Image, new Point(x, y), 1f);
                    stitchedImagePaths.Add(img.ImagePath);

                    currentWidthPos += img.Image.Width + frameSize;
                }
            }
        });

        if (processOptions.Scale != 1.0f)
        {
            var scaledWidth = (int)(width * processOptions.Scale);
            var scaledHeight = (int)(height * processOptions.Scale);

            stitchedImage.Mutate(ctx => ctx.Resize(scaledWidth, scaledHeight));
        }
        
        var savedImagePath = GetStitchedFilePath(stitchedImagePaths, outputPath, processOptions.SavedImageExtension);
        stitchedImage.Save(savedImagePath);

        logger.LogInformation("Images {images} successfully stitched to {outputImage}.",
            string.Join(", ", stitchedImagePaths.Select(fn => Path.GetFileNameWithoutExtension(fn))),
            Path.GetFileNameWithoutExtension(savedImagePath));

        foreach (var img in images)
            img.Image.Dispose();
    }

    private string GetStitchedFilePath(List<string> origPaths, string outputPath, string extension)
    {
        var origFileNames = origPaths.Select(fn => Path.GetFileNameWithoutExtension(fn));        
        var extWithoutPeriod = extension.Replace(".", string.Empty).Trim();

        return Path.Combine(outputPath, $"{string.Join('_', origFileNames)}.{extWithoutPeriod}");
    }
}