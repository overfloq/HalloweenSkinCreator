using HalloweenSkin.Properties;
using PastelExtended;
using System.Drawing;

if (args.Length != 1)
{
    Console.WriteLine("Please, specify a path to the skin template.");
    return;
}

string templatePath = args[0];
if (!File.Exists(templatePath))
{
    Console.WriteLine("Template specified must exist.");
    return;
}

var allowedExtensions = new[] { ".png", ".jpg", ".bmp", ".webp" };
if (!allowedExtensions.Any(x => x.Equals(Path.GetExtension(templatePath))))
{
    Console.WriteLine($"Invalid image format. Supported formats are {string.Join(", ", allowedExtensions)}");
    return;
}

Image bitmap;
try
{
    bitmap = Image.FromFile(templatePath);
}
catch
{
    Console.WriteLine("Failed to load image due to corrupted format.");
    return;
}

if (bitmap is not
    {
        Width: 64,
        Height: 64
    })
{
    Console.WriteLine("Invalid image resolution. Minecraft skins are 64x64 pixels big.");
    return;
}

const int InnerLayerLeft = 8;
const int InnerLayerTop = 8;
const int LayerSize = 8;

const int OuterLayerLeft = 40;
const int OuterLayerTop = 8;

Console.WriteLine("Loading resources and adding image mask ...");
var pumpkinLayer = Resources.PumpkinMask;

using (var g = Graphics.FromImage(bitmap))
{
    g.DrawImage(pumpkinLayer, 0, 0, 64, 64);
}

Console.WriteLine("Creating modified profile picture of your new skin ...");
var headBitmap = new Bitmap(LayerSize, LayerSize);
using (var g = Graphics.FromImage(headBitmap))
{
    g.DrawImage(((Bitmap)bitmap).Clone(new Rectangle(
        new Point(InnerLayerLeft, InnerLayerTop), new Size(LayerSize, LayerSize)), bitmap.PixelFormat),
        0, 0);
    g.DrawImage(((Bitmap)bitmap).Clone(new Rectangle(
        new Point(OuterLayerLeft, OuterLayerTop), new Size(LayerSize, LayerSize)), bitmap.PixelFormat),
        0, 0);
}

var directoryName = Path.GetDirectoryName(templatePath)!;
var newFileName = $"{Path.GetFileNameWithoutExtension(templatePath)} - Halloween Mask{Path.GetExtension(templatePath)}";

Console.WriteLine($"Saving new skin as '{newFileName}' ...");
bitmap.Save(Path.Combine(directoryName, newFileName));

Console.WriteLine();
for (int top = 0; top < LayerSize; top++)
{
    Console.Write("  ");
    for (int left = 0; left < LayerSize; left++)
    {
        var pixel = headBitmap.GetPixel(left, top);
        Console.Write("  ".Bg(Color.FromArgb(pixel.R, pixel.G, pixel.B)));
    }
    Console.WriteLine();
}