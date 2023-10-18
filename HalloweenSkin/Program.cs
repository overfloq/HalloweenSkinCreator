using HalloweenSkin;
using HalloweenSkin.Properties;
using PastelExtended;
using System.Drawing;

try
{
    var application = new Application(args);
    application.Run();
}
finally
{
    while (Console.ReadKey(true).Key != ConsoleKey.Enter)
    {
    }
}