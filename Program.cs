using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Sauce_Engine.Serialization;
using Sauce_Engine.Types;

namespace Sauce_Engine;

public static class Program
{
	private static void Main()
	{
		MapObjList mapObjects = MapObjList.Instance;
		Deserializer.DeserializeMap(@"C:\Users\Yasuda\Documents\Projects\Sauce-Engine\exported_data.map");

		var nativeWindowSettings = new NativeWindowSettings()
		{
			ClientSize = new Vector2i(1900, 1040),
			Title = "hl3.exe",
		};

		using var window = new Window(GameWindowSettings.Default, nativeWindowSettings);
		window.Run();
	}
}