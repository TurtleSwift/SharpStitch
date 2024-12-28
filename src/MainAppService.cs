using System.Reflection;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharpStitch.Options;
using Directory = System.IO.Directory;

namespace SharpStitch;

internal class MainAppService : IHostedService
{
    private readonly ILogger<MainAppService> logger;
    private readonly AppOptions options;
    private readonly ImageProcessor imageProcessor;
    private readonly IHostApplicationLifetime hostApplicationLifetime;

    public MainAppService(
        ILogger<MainAppService> logger,
        AppOptions options,
        ImageProcessor imageProcessor,
        IHostApplicationLifetime hostApplicationLifetime)
    {
        this.logger = logger;
        this.options = options;
        this.imageProcessor = imageProcessor;
        this.hostApplicationLifetime = hostApplicationLifetime;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (!AppOptionsValidator.ValidateOptions(options, logger))
        {
            hostApplicationLifetime.StopApplication();
            return Task.CompletedTask;
        }

        var outputPath = GetOutputPathAndEnsureExists();
        var processOptions = new ImageProcessOptions 
        {
            Rotate = options.Rotation,
            Scale = options.Scale,
            SavedImageExtension = options.Encoding,
            BorderSize = options.BorderSize
        };

        //first and second image provided, just stitch them
        if (options.Files?.Count() > 1)
        {
            var fileList = options.Files.ToList();
            imageProcessor.StitchImages(fileList[0], fileList[1], outputPath, processOptions);
        }
        //stitch multiple images from input path
        else if (!string.IsNullOrEmpty(options.InputPath))
        {
            var dirInfo = new DirectoryInfo(options.InputPath);
            var imageFiles = dirInfo.GetFiles($"*.{options.Encoding}");
            
            var imageFilesSorted = (options.StitchOrderMethod switch
            {
                StitchOrder.ByName => imageFiles.OrderBy(fi => fi.Name),
                StitchOrder.ByDateNewestToOldest => imageFiles.OrderByDescending(fi => GetDateTakenOrCreationTime(fi)),
                StitchOrder.ByDateOldestToNewest => imageFiles.OrderBy(fi => GetDateTakenOrCreationTime(fi)),
                _ => throw new NotSupportedException($"Selection method {options.StitchOrderMethod} not supported.")
            }).ToList();

            for (int i = 0; i < imageFilesSorted.Count - 1; i += 2)
                imageProcessor.StitchImages(imageFilesSorted[i].FullName, imageFilesSorted[i+1].FullName, outputPath, processOptions);
        }
        else
        {
            logger.LogError("An input path must be provided. Either provide two input images or an input directory.");
        }

        logger.LogInformation("Done.");

        hostApplicationLifetime.StopApplication();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    //get time taken from image exif or take time created
    private DateTime GetDateTakenOrCreationTime(FileInfo fileInfo)
    {
        var metadata = ImageMetadataReader.ReadMetadata(fileInfo.FullName);
        var exifSubIfDir = metadata.OfType<ExifSubIfdDirectory>().FirstOrDefault();
        
        if (exifSubIfDir?.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out var dateTimeTaken) ?? false)
            return dateTimeTaken;

        return fileInfo.CreationTime;
    }

    private string GetOutputPathAndEnsureExists()
    {
        string? outputPath;

        //output path provided, use that
        if (!string.IsNullOrEmpty(options.OutputPath))
            outputPath = options.OutputPath;
        //no output path, try use path of first image input
        else if (options.Files?.Any() ?? false)
            outputPath =  Path.GetDirectoryName(options.Files.First())!;
        //maybe input path is provided?
        else if (!string.IsNullOrEmpty(options.InputPath))
            outputPath =  options.InputPath;
        //default output is executing assembly location, shouldnt come to this..
        else
            outputPath = Path.GetDirectoryName(AppContext.BaseDirectory)!;

        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        return outputPath;
    }
}