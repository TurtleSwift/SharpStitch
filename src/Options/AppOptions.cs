using CommandLine;

namespace SharpStitch.Options;

class AppOptions
{
    [Option('f', "files", Required = true, SetName = "specificFiles", HelpText = "Image files you want stitched together. Images will be stiched in order provided.", Min = 2)]
    public IEnumerable<string>? Files { get; set; }
    
    [Option('i', "input", Required = true, SetName = "allFilesFromDir", HelpText = "Input directory. The directory where images to stitch will be searched for. Stitch order method can be changed with -m.")]
    public string? InputPath { get; set; }

    [Option('t', "take", Required = false, HelpText = "Supply the number of images to be stitched together. When not set, all input images will be stitched into one image. Value should be at least 2.")]
    public uint? TakeImageCount { get; set; } = null;

    [Option('m', "stitchOrderMethod", Required = false, HelpText = "When input directory is provided, the method of selecting which image should be stitched first with another can be changed. When picking by date, time taken EXIF data is used if exists, otherwise, date created is used.\nOptions:\nByDateNewestToOldest - Will pick the newest image first\nByDateOldestToNewest - Will pick the oldest image first\nByName - Will pick in alphabetical order", Default = default(StitchOrder))]
    public StitchOrder StitchOrderMethod { get; set; }

    [Option('o', "output", Required = false, HelpText = "Output path. If not provided, stiched image will be placed in the same path as input.")]
    public string? OutputPath { get; set; }

    [Option('e', "encoding", Required = false, HelpText = "Output and input format. Images in input path must be of this file type.", Default = "jpg")]
    public string Encoding { get; set; } = "jpg";

    [Option('r', "rotate", Required = false, HelpText = "Rotate the images N degrees.", Default = 0)]
    public uint Rotation { get; set; } = 0;

    [Option('s', "scale", Required = false, HelpText = "Scale the final image. Ex.: 0.5 to scale down 50% or 1.5 to scale up 150%.", Default = 1.0f)]
    public float Scale { get; set; } = 1.0f; 

    [Option('b', "borderSize", Required = false, HelpText = "Add a border around the stitched images. Value in pixels.", Default = 0)]
    public uint BorderSize { get; set; } = 0; 

    [Option('w', "gridWidth", Required = false, HelpText = "Number of images to stack vertically (columns). Leave 0 for best guess.", Default = 0)]
    public uint Width { get; set; } = 0;

    [Option('h', "gridHeight", Required = false, HelpText = "Number of images to stack horizontally (rows). Leave 0 for best guess.", Default = 0)]
    public uint Height { get; set; } = 0;

    [Option("wh", Required = false, HelpText = "Alternative way to supply width and height of grid. Format: WxH Ex.: 2x1")]
    public string? WidthHeight { get; set; } = null;
}
