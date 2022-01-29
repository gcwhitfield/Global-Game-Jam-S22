using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapController : Singleton<TilemapController>
{
    public Tilemap backgroundTilemap;
    public Tilemap itemsTilemap;

    public TileBase[] wallTiles;
    public TileBase[] floorTiles;
    public TileBase[] goalTiles;
    public TileBase[] boxTiles;
    public TileBase[] doorTiles;
    public TileBase[] startLocationTiles;

    [System.Serializable]
    public enum BackgroundTileType
    {
        WALL,
        OPEN
    };

    [System.Serializable]
    public enum ItemsTileType
    {
        BOX,
        DOOR,
        START_LOCATION,
        GOAL,
        NONE
    };


    // returns the type of the background tile at 'location'
    BackgroundTileType GetBackgroundTile(Vector3Int location)
    {
        TileBase tile = backgroundTilemap.GetTile(location);
        foreach (TileBase t in wallTiles)
        {
            if (tile == t) return BackgroundTileType.WALL;
        }
        foreach (TileBase t in floorTiles)
        {
            if (tile == t) return BackgroundTileType.OPEN;
        }
        return BackgroundTileType.OPEN;
    }

    // returns the type of the foreground tile at 'location'
    ItemsTileType GetItemsTile(Vector3Int location)
    {
        TileBase tile = itemsTilemap.GetTile(location);
        foreach (TileBase t in goalTiles)
        {
            if (tile == t) return ItemsTileType.GOAL;
        }
        foreach (TileBase t in boxTiles)
        {
            if (tile == t) return ItemsTileType.BOX;
        }
        foreach (TileBase t in startLocationTiles)
        {
            if (tile == t) return ItemsTileType.START_LOCATION;
        }
        foreach (TileBase t in doorTiles)
        {
            if (tile == t) return ItemsTileType.DOOR;
        }

        return ItemsTileType.NONE;
    }


    // Given a current player position, and a movement vector returns the direction
    // that the player moves to, given the constraints of the map. The player will
    // NOT intersect with walls
    public Vector3 MovePlayer(Vector3Int playerLocation, Vector3Int movement, bool isShadow)
    {
        BackgroundTileType nextTileBG = GetBackgroundTile(playerLocation + movement);
        ItemsTileType nextTileItem = GetItemsTile(playerLocation + movement);

        //Vector3 offset = new Vector3(0.5f, 0.5f, 0);
        if (nextTileBG == BackgroundTileType.WALL)
        {
            return  Vector3.zero; // do not allow the player to move into or through walls
        } else
        {
            // Given the player's location, activate the tile that the player is standing on.
            // For example, if the player is standing on the goal, then the game logic for hitting the
            // goal will execute. If the player is standing on a door, then the door will open/close, etc.
            // If the player is moving towards a box, then it will move the box

            // the player is moving into a box
            if (nextTileItem == ItemsTileType.BOX)
            {
                Vector3 nextMovement = MovePlayer(playerLocation + movement, movement, isShadow);
                // TODO: move the box
                TileBase box = itemsTilemap.GetTile(Vector3Int.FloorToInt(playerLocation + movement));
                itemsTilemap.SetTile(Vector3Int.FloorToInt(nextMovement) + playerLocation + movement, box);
                itemsTilemap.SetTile(Vector3Int.FloorToInt(nextMovement) + playerLocation, null);
                return nextMovement;
            }

            // the player is moving into a goal
            if (nextTileItem == ItemsTileType.GOAL)
            {
                if (!isShadow)
                {
                    LevelController.Instance.OnGoalReached();
                }
            }
            // the player is moving into a door
            else if (nextTileItem == ItemsTileType.DOOR)
            {
                // TODO: open or close the door
            }
            // the player is moving into the start location
            else if (nextTileItem == ItemsTileType.START_LOCATION)
            {
                if (!isShadow)
                {
                    LevelController.Instance.OnStartLocationReached();
                }
            }

            return  movement;
        } 
    }
}
