## SharpStitch

SharpStitch is a simple command line tool to stitch two images together.

**Original usecase**: When I was selling Pokemon cards on CardMarket, I needed a way to quickly stitch two photos of the front and back of the card together into one image. I coudln't find a fast solution to do it so I just made this tool.

## Usage examples

### Stitch two images together ( -f )

*Command*: `./SharpStitch.exe -f C:\Users\Turtle\Desktop\cards\front.jpg C:\Users\Turtle\Desktop\cards\back.jpg`

*Produces*: new image front_back.jpg in the same directory.

### Specify output ( -o )

*Command*: `./SharpStitch.exe -f C:\Users\Turtle\Desktop\cards\front.jpg C:\Users\Turtle\Desktop\cards\back.jpg -o C:\Users\Turtle\Desktop\stitched_cards`

*Produces*: new image front_back.jpg in the specified stitched_cards directory.

### Stitch all images in folder ( -i )

*Command*: `./SharpStitch.exe -i C:\Users\Turtle\Desktop\cards`

*Produces*: stitched images in the same folder. Stitches images based on time taken. By default, oldest image will be first, stitched to second oldest. This can be changed with -m flag like so.

``./SharpStitch.exe -i C:\Users\Turtle\Desktop\cards -m ByDateNewestToOldest``

``./SharpStitch.exe -i C:\Users\Turtle\Desktop\cards -m ByDateOldestToNewest``

### Rotate images before stitching ( -r )

*Command*: `./SharpStitch.exe -i C:\Users\Turtle\Desktop\cards -r 90`

*Produces*: stitched images in the same folder, rotated by 90 degrees.

### Scale final stitched image ( -s )

*Command*: `./SharpStitch.exe -i C:\Users\Turtle\Desktop\cards -s 0.6`

*Produces*: stitched images in the same folder, scaled down to 60% of original size. To scale up, use number larger than 1. Useful for making smaller sized images.

### Add a border ( -b )

*Command*: `./SharpStitch.exe -i C:\Users\Turtle\Desktop\cards -b 50`

*Produces*: stitched images in the same folder with border all around and between the stitched images. Border size is as provided 50px.

### Use different encoding ( -e )

*Command*: `./SharpStitch.exe -i C:\Users\Turtle\Desktop\cards -e png`

*Produces*: stitched images in the same folder in the png format. Input images must have the png file extension as well.

### Multiple options in conjunction

*Command*: `./SharpStitch.exe -i C:\Users\Turtle\Desktop\cards -o C:\Users\Turtle\Desktop\stitched_cards -r 90 -s 0.6 -b 50`

## Command line options 
| Option                 | Required | Default              | Description                                                                                                             |
| ---------------------- | -------- | -------------------- | ------------------------------------------------------------------------------------------------------------------------|
|-f, --files             | Yes |                      | Image files you want stitched together. Images will be stiched in order provided.                                       |
|-i, --input             | Yes |                      | Input directory. The directory where images to stitch will be searched for. Stitch order method can be changed with -m. |
|-m, --stitchOrderMethod |          | ByDateOldestToNewest | When input directory is provided, the method of selecting which image should be stitched first with another can be changed. When picking by date, time taken EXIF data is used if exists, otherwise, date created is used. <br>Options:<br>ByDateNewestToOldest - Will pick the newest image first<br>ByDateOldestToNewest - Will pick the oldest image first<br>ByName - Will pick in alphabetical order|
|-o, --output            |          |                      |   Output path. If not provided, stiched image will be placed in the same path as input.                                 |
|-e, --encoding          |          |          jpg         |   Output and input format. Images in input path must be of this file type.                                              |
|-r, --rotate            |          |           0          |   Rotate the images N degrees.                                                                                          |
|-s, --scale             |          |           1          |   Scale the final image. Ex.: 0.5 to scale down 50% or 1.5 to scale up 150%.                                            |
|-b, --borderSize        |          |           0          |   Add a border around the stitched images. Value in pixels.                                                             |
|--help                  |          |                      |   Display this help screen.                                                                                             |
|--version               |          |                      |   Display version information.                                                                                          |