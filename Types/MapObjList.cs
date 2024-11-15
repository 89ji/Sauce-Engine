using System.Collections;
using OpenTK.Mathematics;

namespace Sauce_Engine.Types;

public class MapObjList : IEnumerable<MapObject>
{
	public static readonly MapObjList Instance = new();
	List<MapObject> brushes = new();
	public Vector3 eyepos = new();

	[Obsolete("Use the singleton instance instead when needed")]
	public MapObjList()
	{
	}

	public void AddMapObject(MapObject brush)
	{
		brushes.Add(brush);
	}

	public void DeleteMapObject(MapObject brush)
	{
		brushes.Remove(brush);
	}

	public IEnumerator<MapObject> GetEnumerator() =>  brushes.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator()
	 {
		List<MapObject> solids = new();
		List<(MapObject, float)> trans = new();

		foreach(var item in brushes)
		{
			if (item is Entity e) solids.Add(item);
			if (item is Brush b)
			{
				if (b.texture != Textures.Glass) solids.Add(item);
				else trans.Add((item, Vector3.Distance(item.GetTranslate, eyepos)));
			}
		}

		while(trans.Count > 0)
		{
			var minItem = trans[0];
			foreach(var item in trans)
			{
				if (item.Item2 < minItem.Item2) minItem = item;
			}
			solids.Add(minItem.Item1);
			trans.Remove(minItem);
		}
		brushes = solids;

		return brushes.GetEnumerator();
	}
	public void Clear() => brushes.Clear();

}