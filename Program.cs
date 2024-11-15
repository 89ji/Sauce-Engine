using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using Sauce_Engine.Serialization;
using Sauce_Engine.Types;
using StbImageSharp;
using System.Drawing;

namespace Sauce_Engine;

public static class Program
{
	private static void Main()
	{
		MapObjList mapObjects = MapObjList.Instance;
		Deserializer.DeserializeMap(@"asdasd.map", out string Mapname);

		using Stream stream = File.OpenRead("ficon.png");
		var img = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
		var image = new Image(img.Width, img.Height, img.Data);
		var icon = new WindowIcon(image);


		var nativeWindowSettings = new NativeWindowSettings()
		{
			ClientSize = new Vector2i(1900, 1040),
			Title = $"hl3.exe - {Mapname}",
			Icon = icon
		};

		using var window = new Window(GameWindowSettings.Default, nativeWindowSettings);
		window.Run();
	}
}