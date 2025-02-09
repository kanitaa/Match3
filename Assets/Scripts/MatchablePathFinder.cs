using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchablePathFinder
{
    private readonly GridManager _gridManager;
    private readonly GameManager _gameManager;

    public MatchablePathFinder(GridManager gridManager, GameManager gameManager)
    {
        _gridManager = gridManager;
        _gameManager = gameManager;
    }

    public HashSet<Tile> FindAllPaths(Tile startTile, Matchable startMatchable)
    {
        Queue<Tile> queue = new Queue<Tile>();
        queue.Enqueue(startTile);

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        HashSet<Tile> matchedTiles = new HashSet<Tile>();

        while (queue.Count > 0)
        {
            Tile currentTile = queue.Dequeue();
            Matchable currentMatchable = currentTile.Matchable;

            foreach (Vector2Int direction in directions)
            {
                var pos = (Vector2)currentTile.transform.position;
                var currentPos = new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
                Vector2Int neighborPos = currentPos + direction;

                if (!_gridManager.GetTiles().ContainsKey(neighborPos))
                {
                    continue;
                }
                   
                Tile neighborTile = _gridManager.GetTiles()[neighborPos];

                if (matchedTiles.Contains(neighborTile))
                {
                    continue;
                }
                   
                Matchable neighborMatchable = neighborTile.Matchable;
              
                if (AreMatchablesMatch(currentMatchable, neighborMatchable) &&
                    AreMatchablesConnected(currentPos, neighborPos))
                {
                    matchedTiles.Add(neighborTile);
                    queue.Enqueue(neighborTile);
                }
            }
        }
        return GetValidRowAndColumnMatches(matchedTiles);

    }
    private bool AreMatchablesConnected(Vector2Int selected, Vector2Int testSelect)
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var direction in directions)
        {
            Vector2Int adjacentPos = selected + direction;

            if (adjacentPos == testSelect)
            {
                if (adjacentPos.x >= 0 && adjacentPos.x < _gameManager.GridRows &&
                    adjacentPos.y >= 0 && adjacentPos.y < _gameManager.GridColumns)
                {
                    return true;
                }
            }
        }
        return false;

    }

    private bool AreMatchablesMatch(Matchable first, Matchable second)
    {
        if (first.MatchableType == second.MatchableType)
        {
            return true;
        }
        return false;

    }

    private HashSet<Tile> GetValidRowAndColumnMatches(HashSet<Tile> tiles)
    {
        Dictionary<int, List<Tile>> rowGroups = new Dictionary<int, List<Tile>>();
        Dictionary<int, List<Tile>> colGroups = new Dictionary<int, List<Tile>>();
        HashSet<Tile> validMatches = new HashSet<Tile>();

        foreach (Tile tile in tiles)
        {
            Vector2Int tilePos = new Vector2Int(
                Mathf.RoundToInt(tile.transform.position.x),
                Mathf.RoundToInt(tile.transform.position.y)
            );

            if (!rowGroups.ContainsKey(tilePos.y))
            {
                rowGroups[tilePos.y] = new List<Tile>();
            }
            rowGroups[tilePos.y].Add(tile);

            if (!colGroups.ContainsKey(tilePos.x))
            {
                colGroups[tilePos.x] = new List<Tile>();
            }
            colGroups[tilePos.x].Add(tile);
        }

        foreach (var row in rowGroups.Values)
        {
            if (row.Count >= 3)
            {
                validMatches.UnionWith(row.ToHashSet());
            }
        }
            
        foreach (var col in colGroups.Values)
        {
            if (col.Count >= 3)
            {
                validMatches.UnionWith(col.ToHashSet());
            }
        }
        return validMatches;
    }
}


