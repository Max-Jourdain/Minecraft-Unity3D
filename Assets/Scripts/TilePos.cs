using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePos
{
    int xPos, yPos;

    Vector2[] uvs;

    public TilePos(int xPos, int yPos)
    {
        this.xPos = xPos;
        this.yPos = yPos;
        uvs = new Vector2[]
        {
            new Vector2(xPos/8f + .001f, yPos/8f + .001f),
            new Vector2(xPos/8f+ .001f, (yPos+1)/8f - .001f),
            new Vector2((xPos+1)/8f - .001f, (yPos+1)/8f - .001f),
            new Vector2((xPos+1)/8f - .001f, yPos/8f+ .001f),
        };
    }

    public Vector2[] GetUVs()
    {
        return uvs;
    }


    public static Dictionary<Tile, TilePos> tiles = new Dictionary<Tile, TilePos>()
    {
        // Numbers 1 to 8
        {Tile.Num1, new TilePos(0, 0)},
        {Tile.Num2, new TilePos(0, 1)},
        {Tile.Num3, new TilePos(0, 2)},
        {Tile.Num4, new TilePos(0, 3)},
        {Tile.Num5, new TilePos(0, 4)},
        {Tile.Num6, new TilePos(0, 5)},
        {Tile.Num7, new TilePos(0, 6)},
        {Tile.Num8, new TilePos(0, 7)},

        // Utils
        {Tile.Unplayed, new TilePos(1, 0)},
        {Tile.Played, new TilePos(1, 1)},
        {Tile.Flag, new TilePos(1, 2)},
        {Tile.Mine, new TilePos(1, 3)},

        // Colors
        {Tile.Color1, new TilePos(2, 0)},
        {Tile.Color2, new TilePos(2, 1)},
        {Tile.Color3, new TilePos(2, 2)},
        {Tile.Color4, new TilePos(2, 3)},
        {Tile.Color5, new TilePos(2, 4)}

    };
}

public enum Tile 
{ 
    Num1, Num2, Num3, Num4, Num5, Num6, Num7, Num8,
    Color1, Color2, Color3, Color4, Color5,
    Unplayed, Played, Flag, Mine
}
