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
            new Vector2(xPos/16f + .001f, yPos/16f + .001f),
            new Vector2(xPos/16f+ .001f, (yPos+1)/16f - .001f),
            new Vector2((xPos+1)/16f - .001f, (yPos+1)/16f - .001f),
            new Vector2((xPos+1)/16f - .001f, yPos/16f+ .001f),
        };
    }

    public Vector2[] GetUVs()
    {
        return uvs;
    }


    public static Dictionary<Tile, TilePos> tiles = new Dictionary<Tile, TilePos>()
    {
        {Tile.MainSurface, new TilePos(0, 0)},
        {Tile.Num1, new TilePos(0, 1)},
        {Tile.Num2, new TilePos(0, 2)},
        {Tile.Color1, new TilePos(1, 0)},
        {Tile.Color2, new TilePos(1, 1)},
        {Tile.Color3, new TilePos(1, 2)},
        {Tile.Color4, new TilePos(1, 3)},
        {Tile.Color5, new TilePos(1, 4)},

    };
}

public enum Tile 
{ 
    MainSurface,
    Num1, Num2,
    Color1, Color2, Color3, Color4, Color5 
}
