using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _tilePrefab;
    private GameObject _gridContainer;
   
    private Dictionary<Vector2Int, Tile> _tiles = new();

    private void Awake()
    {
        _gridContainer = gameObject;
    }

    public Dictionary<Vector2Int, Tile> GetTiles()
    {
        return _tiles;
    }

    public Tile GetTileAtPosition(Vector2Int position)
    {
        return _tiles.TryGetValue(position, out var tile) ? tile : null;
    }

    public void CreateGrid(int rows, int columns)
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                GameObject tile = Instantiate(_tilePrefab.gameObject, new Vector3(row, column), Quaternion.identity, _gridContainer.transform);
                tile.name = "Tile(" + row + "," + column+")";
                _tiles[new Vector2Int(row, column)] = tile.GetComponent<Tile>();
            }
        }
        Camera.main.GetComponent<CameraView>().SetViewBasedOnGrid(rows, columns);
    }

    public void DestroyGrid()
    {
        foreach (Transform child in _gridContainer.transform)
        {
            Destroy(child.gameObject);
        }
        _tiles.Clear();
    }
}
