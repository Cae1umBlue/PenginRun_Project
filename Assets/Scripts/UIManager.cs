using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI References")]
    public Button quitButton;
    public Text scoreText;
    public Text highScoreText;
    public Image hpBarImage;

    public GameObject introUI;
    public GameObject inGameUI;
    public GameObject gameOverUI;

    [Header("Button Effects")]
    public ButtonEffect[] buttonEffects;
    public float revertDelay = 0.5f;

    private bool isGameOver = false;
    private float currentHP = 1f;
    private float hpDecreaseSpeed = 0.01f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // 1) 초기 UI 세팅
        HideAllUI();
        ShowIntroUI();
        Time.timeScale = 0f;

        // 2) Score 이벤트 구독 (Null 체크)
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged += UpdateScoreUI;
            ScoreManager.Instance.OnHighScoreChanged += UpdateHighScoreUI;

            // 초기값 표시
            UpdateScoreUI(ScoreManager.Instance.CurrentScore);
            UpdateHighScoreUI(ScoreManager.Instance.HighScore);
        }

        // 3) Quit 버튼
        if (quitButton != null)
            quitButton.onClick.AddListener(GameManager.Instance.QuitGame);

        // 4) 버튼 이펙트
        foreach (var e in buttonEffects)
        {
            if (e.button == null || e.targetImage == null || e.newSprite == null) continue;
            var original = e.targetImage.sprite;
            e.button.onClick.AddListener(() =>
            {
                e.targetImage.sprite = e.newSprite;
                StartCoroutine(RevertAfterDelay(e.targetImage, original));
            });
        }
    }

    private void OnDestroy()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= UpdateScoreUI;
            ScoreManager.Instance.OnHighScoreChanged -= UpdateHighScoreUI;
        }
    }

    // 인트로 UI의 Start 버튼에 연결
    public void OnStartButtonPressed()
    {
        HideAllUI();
        ShowInGameUI();

        // GameManager 쪽에 Ready→Playing 전환 로직을 구현하세요
        GameManager.Instance.StartGame();
        Time.timeScale = 1f;
    }

    private void Update()
    {
        // Playing 상태에서만 HP 감소
        if (GameManager.Instance.CurrentState != GameState.Playing) return;

        currentHP -= hpDecreaseSpeed * Time.deltaTime;
        currentHP = Mathf.Max(currentHP, 0f);
        UpdateHPUI(currentHP);

        if (currentHP <= 0f && !isGameOver)
        {
            isGameOver = true;
            HideAllUI();
            ShowGameOverUI();
            Time.timeScale = 0f;
        }
    }

    // UI 갱신 메서드들
    public void UpdateScoreUI(int score)
    {
        if (scoreText != null)
            scoreText.text = $"점수: {score}";
    }

    public void UpdateHighScoreUI(int highScore)
    {
        if (highScoreText != null)
            highScoreText.text = $"최고점: {highScore}";
    }

    public void UpdateHPUI(float hpRatio)
    {
        if (hpBarImage != null)
            hpBarImage.fillAmount = Mathf.Clamp01(hpRatio);
    }

    private IEnumerator RevertAfterDelay(Image img, Sprite orig)
    {
        yield return new WaitForSeconds(revertDelay);
        if (img != null)
            img.sprite = orig;
    }

    // UI 토글 헬퍼
    private void HideAllUI()
    {
        introUI?.SetActive(false);
        inGameUI?.SetActive(false);
        gameOverUI?.SetActive(false);
    }

    public void ShowIntroUI()
    {
        HideAllUI();
        introUI?.SetActive(true);
    }

    public void ShowInGameUI()
    {
        HideAllUI();
        inGameUI?.SetActive(true);
    }

    public void ShowGameOverUI()
    {
        HideAllUI();
        gameOverUI?.SetActive(true);
    }

    [System.Serializable]
    public class ButtonEffect
    {
        public Button button;
        public Image targetImage;
        public Sprite newSprite;
    }
}