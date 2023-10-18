using HalloweenSkin.Properties;
using PastelExtended;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloweenSkin;
internal sealed class Application
{
    private readonly string[] _args;
    public Application(string[] args)
    {
        _args = args;
    }

    bool DoesHaveFlag(char flag)
    {
        if (_args.Length == 0)
            return false;

        var flags = _args.AsSpan()[1..];
        return flags.Contains($"-{char.ToLower(flag)}");
    }

    public void Run()
    {
        Console.Title = "Halloween Skin Creator - Minecraft Java";

        if (PastelEx.Settings.Enabled = !DoesHaveFlag('N'))
        {
            PastelEx.Background = Color.Black;
            PastelEx.Foreground = Color.LightGray;
            PastelEx.Clear();
        }

        Console.WriteLine();

        if (_args.Length == 0)
        {
            Console.WriteLine("""
                  You must specify a path to the image of your skin as a first argument. You
                  can also specify more optional flags as a next arguments.
                
                  If you don't know how, you can just simple drag-n-drop the image file
                  into this executable, this will have the exactly same effect.
                """.Fg(Color.White));
            return;
        }

        string templatePath = _args[0];
        if (!File.Exists(templatePath))
        {
            Console.WriteLine("""
                  Well, you specified a file, but it likely doesn't exist, or I just don't
                  have access to it on your disk. Make sure the image you specified exists
                  and try it once again.
                """.Fg(Color.White));
            return;
        }

        var allowedExtensions = new[] { ".png", ".jpg", ".bmp", ".webp" };
        if (!allowedExtensions.Any(x => x.Equals(Path.GetExtension(templatePath))))
        {
            Console.WriteLine($"""
                  The file format you specified is likely not supported. Make sure the extension
                  of your file is one of the following (not case sensitive):
                
                {string.Join(", ", allowedExtensions)}
                """.Fg(Color.White));
            return;
        }

        Image bitmap;
        try
        {
            bitmap = Image.FromFile(templatePath);
        }
        catch
        {
            Console.WriteLine("""
                  The image you specified might look like it's an image, but it seems to be
                  corrupted. Make sure the image is really an image of your skin and try again.
                """.Fg(Color.White));
            return;
        }

        if (bitmap is not
            {
                Width: 64,
                Height: 64
            })
        {
            Console.WriteLine("""
                  Sorry buddy, but minecraft skins are 64 pixels high and 64 pixels wide.
                  Your image seems to be different size, than supported.
                """.Fg(Color.White));
            return;
        }

        const int InnerLayerLeft = 8;
        const int InnerLayerTop = 8;
        const int LayerSize = 8;

        const int OuterLayerLeft = 40;
        const int OuterLayerTop = 8;

        Console.WriteLine("""
              Your skin is being processed. This usually takes less than a second, but
              if it takes a bit longer, please, be patient for at least one minute.

            """.Fg(Color.CornflowerBlue));
        Console.WriteLine("  Loading resources and adding skin mask ...");
        var pumpkinLayer = Resources.PumpkinMask;

        using (var g = Graphics.FromImage(bitmap))
        {
            g.DrawImage(pumpkinLayer, 0, 0, 64, 64);
        }

        Console.WriteLine("  Creating skin head preview ...");
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

        Console.WriteLine($"  Saving as '{newFileName}' ...");
        bitmap.Save(Path.Combine(directoryName, newFileName));

        Console.WriteLine();
        if (!PastelEx.Supported)
        {
            Console.WriteLine("""
                Sorry buddy, but your terminal doesn't support ANSI color codes, which means we can't
                draw the skin preview because it's not supported. Anyways, your new skin has been
                saved, we just can't display it in this terminal. Maybe try using some more *modern*
                terminal, or stop using legacy windows command prompt, if you're using.
                """.Fg(Color.White));
            return;
        }

        PastelEx.Settings.Enabled = true;
        for (int top = 0; top < LayerSize; top++)
        {
            Console.Write("   ");
            for (int left = 0; left < LayerSize; left++)
            {
                var pixel = headBitmap.GetPixel(left, top);
                Console.Write("  ".Bg(Color.FromArgb(pixel.R, pixel.G, pixel.B)));
            }

            if (top == 1)
            {
                Console.Write("  " + "This is the skin preview!".Fg(Color.White)
                    .Deco(Decoration.Bold));
            }
            if (top == 3)
            {
                Console.Write("  Do you like it? Leave a star on my github".Fg(Color.LightGray));
            }
            if (top == 4)
            {
                Console.Write("  and share it with your friends. Thanks!".Fg(Color.LightGray));
            }
            if (top == 6)
            {
                Console.Write("  github.com/k-iro/HalloweenSkinCreator".Fg(Color.LightGray)
                    .Deco(Decoration.Italic));
            }

            Console.WriteLine();
        }
    }
}
