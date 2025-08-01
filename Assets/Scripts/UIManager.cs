using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Button quitButton;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text highScoreText;
    [SerializeField] private Image hpBarImage;

    [SerializeField] private GameObject introUI;
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject gameOverUI;

    [SerializeField] private Text gameOverScoreText;
    [SerializeField] private Text gameOverHighScoreText;

    [SerializeField] private Button restartButton;

    [SerializeField] private Button jumpButton;
    [SerializeField] private Button slideButton;

    [Header("Button Effects")]
    [SerializeField] private ButtonEffect[] buttonEffects;
    [SerializeField] private float revertDelay = 0.5f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        restartButton?.onClick.AddListener(OnRestartButtonPressed);

        // 1) 초기 UI 설정
        HideAllUI();
        introUI.SetActive(true);
        Time.timeScale = 0f;

        // 2) ScoreManager 이벤트 구독
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged += UpdateScoreUI;
            ScoreManager.Instance.OnHighScoreChanged += UpdateHighScoreUI;
            UpdateScoreUI(ScoreManager.Instance.CurrentScore);
            UpdateHighScoreUI(ScoreManager.Instance.HighScore);
        }

        // 3) GameManager 이벤트 구독 (상태 전환 & HP 업데이트)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged += HandleStateChanged;
            GameManager.Instance.OnHPChanged += UpdateHPUI;
        }

        // 4) Quit 버튼
        if (quitButton != null)
            quitButton.onClick.AddListener(GameManager.Instance.QuitGame);

        // 5) 버튼 클릭 이펙트
        foreach (var e in buttonEffects)
        {
            if (e.button == null || e.targetImage == null || e.newSprite == null)
                continue;

            var original = e.targetImage.sprite;
            e.button.onClick.AddListener(() =>
            {
                e.targetImage.sprite = e.newSprite;
                StartCoroutine(RevertAfterDelay(e.targetImage, original));
            });
        }

        // 점프,슬라이드 버튼
        if (jumpButton != null)
            jumpButton.onClick.AddListener(OnJumpButtonPressed);
        if (slideButton != null)
            slideButton.onClick.AddListener(OnSlideButtonPressed);
    }

    private void OnDestroy()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= UpdateScoreUI;
            ScoreManager.Instance.OnHighScoreChanged -= UpdateHighScoreUI;
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= HandleStateChanged;
            GameManager.Instance.OnHPChanged -= UpdateHPUI;
        }
    }

    // 점프 버튼 클릭 시 플레이어 점프
    private void OnJumpButtonPressed()
    {
        if (PlayerController.Instance != null)
        {
            // 슬라이딩 중이 아닐 때만 점프
            if (!PlayerController.Instance.GetIsSliding())
                PlayerController.Instance.JumpByUI();
        }
    }

    // 슬라이드 버튼 클릭 시 플레이어 슬라이드
    private void OnSlideButtonPressed()
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.SlideByUI();
        }
    }

    // GameManager 상태 변화 콜백
    private void HandleStateChanged(GameManager.GameState newState)
    {
        HideAllUI();
        switch (newState)
        {
            case GameManager.GameState.Intro:
                introUI.SetActive(true);
                Time.timeScale = 0f;
                break;
            case GameManager.GameState.Playing:
                inGameUI.SetActive(true);
                Time.timeScale = 1f;
                break;
            case GameManager.GameState.GameOver:
                gameOverUI.SetActive(true);
                Time.timeScale = 0f;

                if (ScoreManager.Instance != null)
                {
                    gameOverScoreText.text = $"이번 게임 점수: {ScoreManager.Instance.CurrentScore}";
                    gameOverHighScoreText.text = $"최고 점수: {ScoreManager.Instance.HighScore}";
                }
                break;
        }
    }

    // Intro 화면 Start 버튼에 연결
    public void OnStartButtonPressed()
    {
        GameManager.Instance.StartGame();
    }

    // Score UI 업데이트
    public void UpdateScoreUI(int score)
    {
        if (scoreText != null)
            scoreText.text = $"점수: {score}";
    }

    // HighScore UI 업데이트
    private void UpdateHighScoreUI(int highScore)
    {
        if (highScoreText != null)
            highScoreText.text = $"최고점: {highScore}";
    }

    // HP Bar UI 업데이트 (0~1)
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

    private void HideAllUI()
    {
        introUI?.SetActive(false);
        inGameUI?.SetActive(false);
        gameOverUI?.SetActive(false);
    }

    [System.Serializable]
    public class ButtonEffect
    {
        public Button button;
        public Image targetImage;
        public Sprite newSprite;
    }

    public void OnRestartButtonPressed()
    {
        GameManager.Instance.RestartGame();
    }
}