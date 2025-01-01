namespace SharpStitch.Options;

static class AppOptionsExtensions
{
    public static (int Columns, int Rows) GetNumOfColumnsAndRows(this AppOptions appOptions, int numberOfImages)
    {
        var heightXHeight = appOptions.WidthHeight?.Split('x');

        // -wh provided
        if (heightXHeight is not null && heightXHeight.Length == 2 &&
            int.TryParse(heightXHeight[0], out var width) &&
            int.TryParse(heightXHeight[1], out var height))
        {
            return (width, height);
        }

        // -w -h provided
        if(appOptions.Width != 0 && appOptions.Height != 0)
        {
            return ((int)appOptions.Width, (int)appOptions.Height);
        }

        // width and height not provided, make a best guess based on image count
        return FindGoodGridFactor(numberOfImages);
    }

    private static (int Width, int Height) FindGoodGridFactor(int numberOfImages)
    {
        int columns = (int) Math.Sqrt(numberOfImages);
        int lines = (int) Math.Ceiling(numberOfImages / (float)columns);

        return (lines, columns);
    }
}