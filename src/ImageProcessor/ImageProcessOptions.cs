namespace SharpStitch;

internal record ImageProcessOptions
{
    public int Rotate { get; set; }
    public float Scale { get; set; } = 1.0f;
    public string SavedImageExtension { get; set; } = "jpg";
    public int BorderSize { get; set; } = 0;
}