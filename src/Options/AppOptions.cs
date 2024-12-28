using CommandLine;

namespace SharpStitch.Options;

class AppOptions
{
    [Option('f', "files", Required = true, SetName = "specificFiles", HelpText = "Image files you want stitched together. Images will be stiched in order provided.", Min = 2)]
    public IEnumerable<string>? Files { get; set; }
    
    [Option('i', "input", Required = true, SetName = "allFilesFromDir", HelpText = "Input directory. The directory where images to stitch will be searched for. Stitch order method can be changed with -m.")]
    public string? InputPath { get; set; }

    [Option('m', "stitchOrderMethod", Required = false, HelpText = "When input directory is provided, the method of selecting which image should be stitched first with another can be changed. When picking by date, time taken EXIF data is used if exists, otherwise, date created is used.\nOptions:\nByDateNewestToOldest - Will pick the newest image first\nByDateOldestToNewest - Will pick the oldest image first\nByName - Will pick in alphabetical order", Default = default(StitchOrder))]
    public StitchOrder StitchOrderMethod { get; set; }

    [Option('o', "output", Required = false, HelpText = "Output path. If not provided, stiched image will be placed in the same path as input.")]
    public string? OutputPath { get; set; }

    [Option('e', "encoding", Required = false, HelpText = "Output and input format. Images in input path must be of this file type.", Default = "jpg")]
    public string Encoding { get; set; } = "jpg";

    [Option('r', "rotate", Required = false, HelpText = "Rotate the images N degrees.", Default = 0)]
    public int Rotation { get; set; } = 0;

    [Option('s', "scale", Required = false, HelpText = "Scale the final image. Ex.: 0.5 to scale down 50% or 1.5 to scale up 150%.", Default = 1.0f)]
    public float Scale { get; set; } = 1.0f; 

    [Option('b', "borderSize", Required = false, HelpText = "Add a border around the stitched images. Value in pixels.", Default = 0)]
    public int BorderSize { get; set; } = 0; 
}
