using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CraftingRPG.Entities;

public class OverworldMap
{
    public int Width { get; set; }
    public int Height { get; set; }
    public List<List<int>> Tiles { get; set; }
    public List<List<int>> CollisionMap { get; set; }

    public OverworldMap(int width, int height)
    {
        Width = width;
        Height = height;
        Tiles = new();
        CollisionMap = new();

        for (int i = 0; i < height; i++)
        {
            var row = new List<int>();
            for (int j = 0; j < width; j++)
            {
                row.Add(-1);
            }
            Tiles.Add(row);
        }

        for (int i = 0; i < height; i++)
        {
            var row = new List<int>();
            for (int j = 0; j < width; j++)
            {
                row.Add(1);
            }
            CollisionMap.Add(row);
        }
    }

    public string Serialize()
    {
        var json = JsonSerializer.Serialize(this, options: new JsonSerializerOptions
        {
            WriteIndented = true
        });
        return json;
    }

    public static OverworldMap FromFile(string filename)
    {
        return JsonSerializer.Deserialize<OverworldMap>(File.ReadAllText(filename));
    }
}
