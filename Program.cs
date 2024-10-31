using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Sauce_Engine;

public static class Program
{
	private static void Main()
	{
		var nativeWindowSettings = new NativeWindowSettings()
		{
			ClientSize = new Vector2i(1900, 1040),
			Title = "hl3.exe",
		};

		using var window = new Window(GameWindowSettings.Default, nativeWindowSettings);
		window.Run();
	}
}