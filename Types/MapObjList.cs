using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sauce_Engine.Types;

public class MapObjList : IEnumerable<MapObject>
{
	public static readonly MapObjList Instance = new();
	List<MapObject> brushes = new();

	private MapObjList()
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
	IEnumerator IEnumerable.GetEnumerator() => brushes.GetEnumerator();
	public void Clear() => brushes.Clear();

}