using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
public enum MatchableType
{
    Water,
    Shadow,
    Nature,
    Poison,
    Fire,
    Sun
}
public class GameManager : MonoBehaviour
{
    private UIManager _uiManager;
    private GridManager _gridManager;
    private MatchablePathFinder _pathFinder;
    private MiniAudioManager _audioManager;

    [SerializeField] List<Matchable> _allMatchablePrefabs = new();
    private List<Matchable> _matchablesInGame = new();
    public bool IsInteractable { get; set; }

    public int GridRows { get; private set; }
    public int GridColumns { get; private set; }
    private int _colorAmount;

    private int _score = 0;
    private int _scoreMultiplier = 0;


    #region Init/Settings
    public void Init(GridManager gridManager, UIManager uiManager, MatchablePathFinder pathFinder, MiniAudioManager audioManager)
    {
        _gridManager = gridManager;
        _uiManager = uiManager;
        _pathFinder = pathFinder;
        _audioManager = audioManager;
    }
    public void SetGameValues(int rows, int columns, int colors)
    {
        //Set values from settings.
        GridRows = rows;
        GridColumns = columns;
        _colorAmount = colors;
        _gridManager.CreateGrid(rows, columns);
        _audioManager.PlayMusic("Final Fantasy 7  Fiddle De Chocobo (Chocobo Racing Theme)");
        InitializeMatchables();
    }
    void InitializeMatchables()
    {
        if (_colorAmount != 6)
        {
            Matchable tempMatchable;
            for (int i = 0; i < _allMatchablePrefabs.Count; i++)
            {
                int rnd = Random.Range(0, _allMatchablePrefabs.Count);
                tempMatchable = _allMatchablePrefabs[rnd];
                _allMatchablePrefabs[rnd] = _allMatchablePrefabs[i];
                _allMatchablePrefabs[i] = tempMatchable;
            }
        }
        for(int i=0; i<_colorAmount; i++)
        {
            _matchablesInGame.Add(_allMatchablePrefabs[i]);
        }
        InitMatchableSpawn();
    }
   
    void UpdateScore(Sprite matchable)
    {
        int scoreIncrease = _scoreMultiplier * _scoreMultiplier; 
        _score += scoreIncrease;
        _uiManager.UpdateScore(_score, _scoreMultiplier, matchable);
        _scoreMultiplier = 0;
    }

    public void ResetGame()
    {
        IsInteractable = false;
        _matchablesInGame = new();
        _scoreMultiplier = 0;
        _score = 0;
    }

    #endregion

    #region Matchable Spawn
    private async void InitMatchableSpawn()
    {
        await SpawnMatchablesAsync();
        IsInteractable = true;
    }

    private async Task SpawnMatchablesAsync()
    {
        float spawnY = -2f;
        for (int column = GridColumns - 1; column >= 0; --column)
        {
            for (int row = GridRows - 1; row >= 0; --row)
            {
                Tile tempTile = _gridManager.GetTileAtPosition(new Vector2Int(row, column));

                if (tempTile.Matchable == null)
                {
                    int random = Random.Range(0, _matchablesInGame.Count);

                    Vector3 spawnPosition = new Vector3(tempTile.transform.position.x, spawnY, 0);
                    Matchable matchable = Instantiate(_matchablesInGame[random], spawnPosition, Quaternion.identity, tempTile.transform);

                    matchable.Init(this, _gridManager, _audioManager, tempTile);
                    _ = matchable.MoveToPosition(tempTile, 0.1f);
                }
            }
            await Task.Delay(100);
        }
        await Task.Delay(100);
        await CheckForNewMatchesAsync();
        IsInteractable = true;
    }
    #endregion

   
    // Path check after matchable swap
    public async Task<HashSet<Tile>> CheckForMatches(Tile currentTile, Tile oldTile, Matchable matchable)
    {
        var matchedTiles = _pathFinder.FindAllPaths(currentTile, matchable);
        await Task.Yield();
        return matchedTiles;
    }

    // Path check after spawning new matchables
    private async Task CheckForNewMatchesAsync()
    {
        HashSet<Tile> matchedTiles = new HashSet<Tile>();
   
        foreach (Tile tile in _gridManager.GetTiles().Values)
        {
            var matches = await CheckForMatches(tile, null, tile.Matchable);
            matchedTiles.UnionWith(matches);
        }

        if (matchedTiles.Count > 0)
        {
            await DestroyMatchables(matchedTiles);
        }
    }

    private async Task MoveAllMatchablesUp(Dictionary<Vector2Int, Tile> tiles)
    {
        HashSet<Tile> movedMatchables = new HashSet<Tile>();

        int minY = tiles.Keys.Min(t => t.y);
        int maxY = tiles.Keys.Max(t => t.y);
        int minX = tiles.Keys.Min(t => t.x);
        int maxX = tiles.Keys.Max(t => t.x);

        for (int y = minY; y <= maxY; y++) 
        {
           
            for (int x = minX; x <= maxX; x++) 
            {
                Vector2Int currentPos = new Vector2Int(x, y);
                if (!tiles.ContainsKey(currentPos))
                    continue;

                Tile currentTile = tiles[currentPos];
                if (currentTile.Matchable == null)
                    continue; 

                Vector2Int abovePos = currentPos + Vector2Int.up;
                while (tiles.ContainsKey(abovePos)) 
                {
                    Tile aboveTile = tiles[abovePos];

                    if (aboveTile.Matchable == null)
                    {
                        aboveTile.Matchable = currentTile.Matchable;
                        aboveTile.Matchable.transform.parent = aboveTile.transform;
                        aboveTile.Matchable.SetTile(aboveTile);

                        await aboveTile.Matchable.MoveToPosition(aboveTile, 0.05f);
                        currentTile.Matchable = null;
                        movedMatchables.Add(currentTile);
                       
                        currentTile = aboveTile;
                        abovePos += Vector2Int.up;

                     
                        y = minY - 1; 
                        break; 
                    }
                    else
                    {
                        break; 
                    }
                }
            }
        }
        await Task.Yield();
    }

    public async Task DestroyMatchables(HashSet<Tile> allMatchedTiles)
    {
        Sprite sprite = null;
        foreach (Tile tile in allMatchedTiles)
        {
            _score++;
            _scoreMultiplier++;
            sprite = tile.Matchable.MatchableSprite;
            tile.Matchable.DestroyMatchable();
        }
        UpdateScore(sprite);
        await MoveAllMatchablesUp(_gridManager.GetTiles());
        await SpawnMatchablesAsync();
    }
}


