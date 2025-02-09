using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameManager _gameManager;
    private GridManager _gridManager;
    private MiniAudioManager _audioManager;

    [Header("Menu")]
    [SerializeField] GameObject _menuPanel;
    [SerializeField] GameObject _settingsPanel;
    [SerializeField] Button _startButton, _settingsButton, _quitButton;

    [Header("Settings")]
    [SerializeField] Slider _rowSlider;
    [SerializeField] Slider _columnSlider;
    [SerializeField] Slider _colorSlider;
    [SerializeField] Slider _audioSlider;
    [SerializeField] TextMeshProUGUI _rowAmount, _columnAmount, _colorAmount, _volumeAmount;
    [SerializeField] Button _menuButton;

    [Header("GameUI")]
    [SerializeField] Button _gameMenuButton, _speedMultiplierButton;
    [SerializeField] Button _leaveGameButton, _cancelButton;
    [SerializeField] Toggle _muteToggle;
    [SerializeField] TextMeshProUGUI _speedText;
    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] GameObject _scoreEffect;
    [SerializeField] GameObject _menuWarningPanel;

    public void Init(GridManager gridManager, GameManager gameManager, MiniAudioManager audioManager)
    {
        _gridManager = gridManager;
        _gameManager = gameManager;
        _audioManager = audioManager;

        AddListeners();
        SetupPlayerPrefs();

        ChangeVolume(_audioSlider.value);
        _rowAmount.text = _rowSlider.value.ToString();
        _columnAmount.text = _columnSlider.value.ToString();
        _colorAmount.text = _colorSlider.value.ToString();
        _scoreText.text = "";
    }

    #region Listeners
    private void AddListeners()
    {
        //Menu
        _startButton.onClick.AddListener(StartGame);
        _settingsButton.onClick.AddListener(ToggleSettings);
        _quitButton.onClick.AddListener(QuitGame);

        //Settings
        _rowSlider.onValueChanged.AddListener(ChangeRowAmount);
        _columnSlider.onValueChanged.AddListener(ChangeColumnAmount);
        _colorSlider.onValueChanged.AddListener(ChangeColorAmount);
        _audioSlider.onValueChanged.AddListener(ChangeVolume);
        _menuButton.onClick.AddListener(ToggleSettings);

        //GameUI
        _gameMenuButton.onClick.AddListener(ToggleMenuWarning);
        _leaveGameButton.onClick.AddListener(ReturnMenu);
        _cancelButton.onClick.AddListener(ToggleMenuWarning);
        _muteToggle.onValueChanged.AddListener(ToggleMute);
    }
    #endregion

    #region PlayerPrefs
    private void SetupPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey("Rows"))
        {
            PlayerPrefs.SetInt("Rows", 7);
        }
        if (!PlayerPrefs.HasKey("Columns"))
        {
            PlayerPrefs.SetInt("Columns", 5);

        }
        if (!PlayerPrefs.HasKey("Colors"))
        {
            PlayerPrefs.SetInt("Colors", 4);
        }
        _rowSlider.value = PlayerPrefs.GetInt("Rows");
        _columnSlider.value = PlayerPrefs.GetInt("Columns");
        _colorSlider.value = PlayerPrefs.GetInt("Colors");
    }

    #endregion

    #region Menu
    void StartGame()
    {
        _gameManager.SetGameValues(PlayerPrefs.GetInt("Rows"), PlayerPrefs.GetInt("Columns"), PlayerPrefs.GetInt("Colors"));
        _menuPanel.SetActive(false);
        _audioManager.PlaySound("448081__breviceps__tic-toc-click");
    }

    void ToggleSettings()
    {
        if (!_settingsPanel.activeSelf) _settingsPanel.SetActive(true);
        else _settingsPanel.SetActive(false);
        _audioManager.PlaySound("448081__breviceps__tic-toc-click");
    }

    void QuitGame()
    {
        _audioManager.PlaySound("448081__breviceps__tic-toc-click");
        Application.Quit();
    }
    #endregion

    #region Settings
    void ChangeRowAmount(float value)
    {
        _rowAmount.text = value.ToString();
        _audioManager.PlaySound("683587__yehawsnail__bubble-pop", true);
        PlayerPrefs.SetInt("Rows", (int)value);
    }

    void ChangeColumnAmount(float value)
    {
        _columnAmount.text = value.ToString();
        _audioManager.PlaySound("683587__yehawsnail__bubble-pop",true);
        PlayerPrefs.SetInt("Columns", (int)value);
    }

    void ChangeColorAmount(float value)
    {
        _colorAmount.text = value.ToString();
        _audioManager.PlaySound("683587__yehawsnail__bubble-pop", true);
        PlayerPrefs.SetInt("Colors", (int)value);
    }

    void ChangeVolume(float value)
    {
        if (!_muteToggle.interactable && value != -35 || !_muteToggle.isOn && value!=35)
        {
            _muteToggle.interactable = true;
            _muteToggle.isOn = true;
        }

        _audioManager.PlaySound("683587__yehawsnail__bubble-pop", true);
        _audioManager.SetVolume(value);

        if (value == _audioSlider.minValue)
        {
            _muteToggle.isOn = false;
            _muteToggle.interactable = false;
        }
        
        value = Mathf.Clamp(value, _audioSlider.minValue, _audioSlider.maxValue);

        float normalizedValue = ((value - _audioSlider.minValue) / (_audioSlider.maxValue - _audioSlider.minValue)) * (100 - 0) + 0;
        _volumeAmount.text = Mathf.RoundToInt(normalizedValue).ToString();

    }

    #endregion

    #region GameUI

    void ToggleMenuWarning()
    {
        if (!_menuWarningPanel.activeSelf) _menuWarningPanel.SetActive(true);
        else _menuWarningPanel.SetActive(false);
    }

    void ReturnMenu()
    {
        _audioManager.PlaySound("448081__breviceps__tic-toc-click");
        _audioManager.PlayMusic("LOOP_Party Down South (live)");
        _gridManager.DestroyGrid();
        _gameManager.ResetGame();
        _menuPanel.SetActive(true);
        _scoreText.text = "";

        if (!_muteToggle.isOn)
        {
            _audioSlider.value = _audioSlider.minValue;
        }

        ToggleMenuWarning();
    }

    public void UpdateScore(int score, int matchableAmount, Sprite matchableSprite)
    {
        _scoreText.text = score.ToString();
        GameObject go = Instantiate(_scoreEffect);
        ScoreEffect scoreEffect = go.GetComponent<ScoreEffect>();
        scoreEffect.MatchableSprite = matchableSprite;
        scoreEffect.MatchableAmount = matchableAmount.ToString()+" x";
    }

    void ToggleMute(bool isOn)
    {
        _audioManager.ToggleMute();
    }

    #endregion

}
