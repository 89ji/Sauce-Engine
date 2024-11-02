using System.Collections.Generic;
using System.Numerics;

namespace Sauce_Engine.Types;

public class Brush : MapObject
{
	public readonly Textures texture = Textures.Crate;
	public Brush(Transform transform)
	{
		this.transform = transform;
	}

	public Brush(Transform transform, Textures texture)
	{
		this.transform = transform;
		this.texture = texture;
	}
}