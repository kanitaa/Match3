using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MatchableMovement
{
    private GameManager _gameManager;

    private Tile _tileA;
    private Tile _tileB;

    private Matchable _matchableA;
    private Matchable _matchableB;

    private float _movementSpeed;

    public MatchableMovement()
    {
    }

    public MatchableMovement(GameManager gameManager, Tile tileA, Tile tileB, Matchable matchableA, Matchable matchableB, float movementSpeed)
    {
        _gameManager = gameManager;

        _tileA = tileA;
        _tileB = tileB;
        _matchableA = matchableA;
        _matchableB = matchableB;

        _movementSpeed = movementSpeed;

    }

    public async void MoveAndCheckPaths()
    {
        await Task.WhenAll(
            MoveMatchableToPosition(_matchableA, _tileB, _movementSpeed),
            MoveMatchableToPosition(_matchableB, _tileA, _movementSpeed)
        );
        
        var matchedTilesA = await _gameManager.CheckForMatches(_tileA, _tileB, _matchableA);
        var matchedTilesB = await _gameManager.CheckForMatches(_tileB, _tileA, _matchableB);

        HashSet<Tile> allMatchedTiles = new HashSet<Tile>(matchedTilesA);
        allMatchedTiles.UnionWith(matchedTilesB);

        if (allMatchedTiles.Count > 0)
        {
            await _gameManager.DestroyMatchables(allMatchedTiles);
        }
        else
        {
            SwapBackTiles();
        }

       _gameManager.IsInteractable = true;
        
    }

    private async void SwapBackTiles()
    {
        await Task.WhenAll(
            MoveMatchableToPosition(_matchableA, _tileA, _movementSpeed),
            MoveMatchableToPosition(_matchableB, _tileB, _movementSpeed)
       );
      
    }

    public async Task MoveMatchableToPosition(Matchable matchable, Tile tile, float duration)
    {
        Vector3 startPosition = matchable.transform.position;
        Vector3 targetPosition = tile.transform.position;

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            matchable.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            await Task.Yield(); 
        }

        matchable.transform.position = targetPosition;
        matchable.SetTile(tile);

    }
}
