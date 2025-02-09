using System.Threading.Tasks;
using TMPro;
using UnityEngine;


public class Matchable : MonoBehaviour
{
    [System.Serializable]
    public class MatchableData
    {
        public MatchableType MatchableType;
        public Sprite MatchableSprite;
        public SpriteRenderer SpriteR;
    }

    private GameManager _gameManager;
    private GridManager _gridManager;
    private MiniAudioManager _audioManager;

    [SerializeField] 
    private MatchableData _matchableData;
    public MatchableType MatchableType => _matchableData.MatchableType;
    public Sprite MatchableSprite => _matchableData.MatchableSprite;
    private SpriteRenderer spriteR => _matchableData.SpriteR;

    public Tile Tile { get; private set; }

    private Vector2 startTouchPosition; 
    private Vector2 endTouchPosition;
   
    private MatchableMovement _movement;
    private float _movementSpeed = 0.2f;
   
    public void Init(GameManager gameManager, GridManager gridManager, MiniAudioManager audioManager, Tile tile)
    {
        _gameManager = gameManager;
        _gridManager = gridManager;
        _audioManager = audioManager;
        spriteR.sprite = MatchableSprite;
       
        _movement = new MatchableMovement();
       
        SetTile(tile);
    }

    public void SetTile(Tile tile)
    {
        Tile = tile;
        Tile.Matchable = this;
        transform.parent = Tile.transform;

    }

    void OnMouseDown()
    {
        if (_gameManager.IsInteractable)
        {
            startTouchPosition = Input.mousePosition;
        }

    }

    private void OnMouseUp()
    {
        if (!_gameManager.IsInteractable)
        {
            return;
        }

        endTouchPosition = Input.mousePosition;
        Vector2 swipeDirection = endTouchPosition - startTouchPosition;

        if (swipeDirection.magnitude < 10) 
        { 
            return; 
        }

        Vector2Int moveDirection = GetSwipeDirection(swipeDirection);

        if (moveDirection == Vector2Int.zero)
        {
            return;
        }

        Vector2Int otherTilePos = Tile.GetPosition() + moveDirection;
        Tile otherTile = _gridManager.GetTileAtPosition(otherTilePos);

        if (otherTile != null && otherTile.Matchable != null)
        {
            _movement = new MatchableMovement(_gameManager, Tile, otherTile, this, otherTile.Matchable, _movementSpeed);
            _gameManager.IsInteractable = false;
            _movement.MoveAndCheckPaths();
        }

    }

    private Vector2Int GetSwipeDirection(Vector2 swipe)
    {
        if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y)) 
        {
            return swipe.x > 0 ? Vector2Int.right : Vector2Int.left;
        }
        else 
        {
            return swipe.y > 0 ? Vector2Int.up : Vector2Int.down;
        }

    }

    public async Task MoveToPosition(Tile tile, float movementSpeed)
    {
        await _movement.MoveMatchableToPosition(this, tile, movementSpeed);
    }

    public void DestroyMatchable()
    {
        _audioManager.PlaySound("377018__elmasmalo1__bubble-pop-high-pitched-short", true);
        Tile.Matchable = null;
        Destroy(gameObject);

    }
}
