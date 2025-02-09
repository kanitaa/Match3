using UnityEngine;

public class Tile : MonoBehaviour
{
    public Matchable Matchable { get; set; }
    public Vector2Int GetPosition()
    {
        return new Vector2Int((int)transform.position.x, (int)transform.position.y);
    }
}
