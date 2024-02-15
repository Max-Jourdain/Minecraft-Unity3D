using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public Tile top, side, bottom;

    public TilePos topPos, sidePos, bottomPos;

    public Block(Tile tile)
    {
        top = side = bottom = tile;
        GetPositions();
    }

    public Block(Tile top, Tile side, Tile bottom)
    {
        this.top = top;
        this.side = side;
        this.bottom = bottom;
        GetPositions();
    }

    void GetPositions()
    {
        topPos = TilePos.tiles[top];
        sidePos = TilePos.tiles[side];
        bottomPos = TilePos.tiles[bottom];
    }


    public static Dictionary<BlockType, Block> blocks = new Dictionary<BlockType, Block>()
    {
        {BlockType.MainSurface, new Block(Tile.MainSurface, Tile.MainSurface, Tile.MainSurface)},
        {BlockType.Mine, new Block(Tile.Color1, Tile.MainSurface, Tile.MainSurface)},
        {BlockType.Num1, new Block(Tile.Num1, Tile.MainSurface, Tile.MainSurface)},
        {BlockType.Num2, new Block(Tile.Num2, Tile.MainSurface, Tile.MainSurface)},
        {BlockType.Color1, new Block(Tile.Color1, Tile.Color1, Tile.Color1)},
        {BlockType.Color2, new Block(Tile.Color2, Tile.Color2, Tile.Color2)},
        {BlockType.Color3, new Block(Tile.Color3, Tile.Color3, Tile.Color3)},
        {BlockType.Color4, new Block(Tile.Color4, Tile.Color4, Tile.Color4)},
        {BlockType.Color5, new Block(Tile.Color5, Tile.Color5, Tile.Color5)},
    };
}

public enum BlockType 
{
    Air, 
    MainSurface,
    Mine,
    Num1, Num2, 
    Color1, Color2, Color3, Color4, Color5
}