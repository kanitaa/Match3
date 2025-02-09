using UnityEngine;

public class GameServices : MonoBehaviour
{
    public static GameServices Instance { get; private set; }

    public GridManager GridManager { get; private set; }
    public GameManager GameManager { get; private set; }
    public UIManager UIManager { get; private set; }
    public MatchablePathFinder PathFinder { get; private set; }
    public MiniAudioManager AudioManager { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            UIManager = FindObjectOfType<UIManager>();
            GridManager = FindObjectOfType<GridManager>();
            GameManager = FindObjectOfType<GameManager>();
            AudioManager = FindObjectOfType<MiniAudioManager>();
            PathFinder = new MatchablePathFinder(GridManager, GameManager);
            
            GameManager.Init(GridManager, UIManager, PathFinder, AudioManager);
            UIManager.Init(GridManager, GameManager, AudioManager);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
