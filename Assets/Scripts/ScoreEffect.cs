using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreEffect : MonoBehaviour
{
    [SerializeField] 
    private Image _matchableSprite;
    public Sprite MatchableSprite{
        get => _matchableSprite.sprite;
        set => _matchableSprite.sprite = value;
    }

    [SerializeField] 
    private TextMeshProUGUI _matchableAmount;
    public string MatchableAmount
    {
        get => _matchableAmount.text;
        set => _matchableAmount.text = value;
    }

    float _movementSpeed = 255.5f;

    void Start()
    {
        Invoke("DestroyEffect", 10);

        int margin = 80; 
        int halfScreenWidth = Screen.width / 2;

        int xPos = Random.Range(-halfScreenWidth + margin, halfScreenWidth - margin);
        transform.GetChild(0).localPosition = new Vector3(xPos, transform.GetChild(0).localPosition.y, 0f);

    }

    void Update()
    {
        transform.GetChild(0).Translate(transform.up * _movementSpeed * Time.deltaTime);
    }

    void DestroyEffect()
    {
        Destroy(gameObject);
    }
}
