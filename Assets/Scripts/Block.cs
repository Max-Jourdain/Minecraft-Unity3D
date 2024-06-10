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
        //* Numbers 0 to 8
        {BlockType.Num1, new Block(Tile.Num1, Tile.Unplayed, Tile.Unplayed)},
        {BlockType.Num2, new Block(Tile.Num2, Tile.Unplayed, Tile.Unplayed)},
        {BlockType.Num3, new Block(Tile.Num3, Tile.Unplayed, Tile.Unplayed)},
        {BlockType.Num4, new Block(Tile.Num4, Tile.Unplayed, Tile.Unplayed)},
        {BlockType.Num5, new Block(Tile.Num5, Tile.Unplayed, Tile.Unplayed)},
        {BlockType.Num6, new Block(Tile.Num6, Tile.Unplayed, Tile.Unplayed)},
        {BlockType.Num7, new Block(Tile.Num7, Tile.Unplayed, Tile.Unplayed)},
        {BlockType.Num8, new Block(Tile.Num8, Tile.Unplayed, Tile.Unplayed)},

        // Utils
        {BlockType.Unplayed, new Block(Tile.Unplayed, Tile.Unplayed, Tile.Unplayed)},
        {BlockType.Played, new Block(Tile.Played, Tile.Unplayed, Tile.Unplayed)},
        {BlockType.Flag, new Block(Tile.Flag, Tile.Unplayed, Tile.Unplayed)},
        {BlockType.Mine, new Block(Tile.Color5, Tile.Unplayed, Tile.Unplayed)},

        
        // Colors
        {BlockType.Color1, new Block(Tile.Color1, Tile.Color1, Tile.Color1)},
        {BlockType.Color2, new Block(Tile.Color2, Tile.Color2, Tile.Color2)},
        {BlockType.Color3, new Block(Tile.Color3, Tile.Color3, Tile.Color3)},
        {BlockType.Color4, new Block(Tile.Color4, Tile.Color4, Tile.Color4)},
        {BlockType.Color5, new Block(Tile.Color5, Tile.Color5, Tile.Color5)}
    };

    // function tha twill update the first tile of a blocktype
    public static void UpdateTile(BlockType blockType, Tile tile)
    {
        Block block = blocks[blockType];
        block.top = block.side = block.bottom = tile;
        block.GetPositions();
    }
    
}


public enum BlockType 
{
    Num1, Num2, Num3, Num4, Num5, Num6, Num7, Num8,
    Air, 
    Unplayed, Played, Mine, Flag,
    Color1, Color2, Color3, Color4, Color5
}