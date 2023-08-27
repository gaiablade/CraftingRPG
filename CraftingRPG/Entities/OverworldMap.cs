using System.Collections.Generic;

namespace CraftingRPG.Entities;

public class OverworldMap
{
    public int Width { get; set; }
    public int Height { get; set; }
    public List<List<int>> Tiles { get; set; }

    public OverworldMap(int width, int height)
    {
        Width = width;
        Height = height;
        Tiles = new List<List<int>>();

        for (int i = 0; i < height; i++)
        {
            var row = new List<int>();
            for (int j = 0; j < width; j++)
            {
                row.Add(0);
            }
            Tiles.Add(row);
        }
    }
}
