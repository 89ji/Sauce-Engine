using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Sauce_Engine.Enums;
using Sauce_Engine.Types;
using Sauce_Engine.Util;

namespace Sauce_Engine.Serialization;

public static class Deserializer
{
	// Loads a .map from a path and places the brushes into the brushlist singleton
	// NOTICE: Currently clears the brushlist before appending
	public static void DeserializeMap(string src)
	{
		if (!File.Exists(src)) throw new FileNotFoundException();
		if (Path.GetExtension(src) != ".map") throw new Exception("Wrong file format");
		
		MapObjList brushes = MapObjList.Instance;
		brushes.Clear();

		string mapname;

		using StreamReader sr = new StreamReader(src);
		while (!sr.EndOfStream)
		{
			string line = sr.ReadLine();
			var tokens = line.Split(':');
			switch (tokens[0])
			{
				case "Mapname":
					mapname = ReadMapname(tokens[1]);
					break;
				case "Brush":
					brushes.AddMapObject(ReadBrush(tokens[1]));
					break;
				case "Entity":
					brushes.AddMapObject(ReadEnt(tokens[1]));
					break;
				default:
					throw new Exception("Unknown token");	
			}
		}
	}

	static string ReadMapname(string line)
	{
		return line.Trim();
	}

	static Brush ReadBrush(string line)
	{
		line = line.Trim().Replace("<", "").Replace(">", "").Replace(",", "");
		string[] numbers = line.Split(' ');

		System.Numerics.Vector3? translation = new(numbers[0].ToFloat() , numbers[1].ToFloat(), numbers[2].ToFloat());
		System.Numerics.Vector3 rotation = new(numbers[3].ToFloat(), numbers[4].ToFloat(), numbers[5].ToFloat());
		System.Numerics.Vector3 scale = new(numbers[6].ToFloat(), numbers[7].ToFloat(), numbers[8].ToFloat());
		
		return new Brush(new Transform(rotation, translation, scale));
	}

	static Entity ReadEnt(string line)
	{
		line = line.Trim().Replace("<", "").Replace(">", "").Replace(",", "");
		var numbers = line.Split(' ');
		
		System.Numerics.Vector3? translation = new(numbers[0].ToFloat(), numbers[1].ToFloat(), numbers[2].ToFloat());
		System.Numerics.Vector3 rotation = new(numbers[3].ToFloat(), numbers[4].ToFloat(), numbers[5].ToFloat());
		Enums.EntityType type = numbers[6] switch
        {
            "Omni" => EntityType.OmniLight,
            "Spot" => EntityType.DirectLight,
            _ => throw new NotImplementedException(),
        };
		
		var ret = new Entity(type);
		ret.TranslateTo(translation.Value);
		ret.RotateTo(rotation);
		return ret;
	}
}