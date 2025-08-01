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

        // 1) �ʱ� UI ����
        HideAllUI();
        introUI.SetActive(true);
        Time.timeScale = 0f;

        // 2) ScoreManager �̺�Ʈ ����
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged += UpdateScoreUI;
            ScoreManager.Instance.OnHighScoreChanged += UpdateHighScoreUI;
            UpdateScoreUI(ScoreManager.Instance.CurrentScore);
            UpdateHighScoreUI(ScoreManager.Instance.HighScore);
        }

        // 3) GameManager �̺�Ʈ ���� (���� ��ȯ & HP ������Ʈ)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged += HandleStateChanged;
            GameManager.Instance.OnHPChanged += UpdateHPUI;
        }

        // 4) Quit ��ư
        if (quitButton != null)
            quitButton.onClick.AddListener(GameManager.Instance.QuitGame);

        // 5) ��ư Ŭ�� ����Ʈ
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

        // ����,�����̵� ��ư
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

    // ���� ��ư Ŭ�� �� �÷��̾� ����
    private void OnJumpButtonPressed()
    {
        if (PlayerController.Instance != null)
        {
            // �����̵� ���� �ƴ� ���� ����
            if (!PlayerController.Instance.GetIsSliding())
                PlayerController.Instance.JumpByUI();
        }
    }

    // �����̵� ��ư Ŭ�� �� �÷��̾� �����̵�
    private void OnSlideButtonPressed()
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.SlideByUI();
        }
    }

    // GameManager ���� ��ȭ �ݹ�
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
                    gameOverScoreText.text = $"�̹� ���� ����: {ScoreManager.Instance.CurrentScore}";
                    gameOverHighScoreText.text = $"�ְ� ����: {ScoreManager.Instance.HighScore}";
                }
                break;
        }
    }

    // Intro ȭ�� Start ��ư�� ����
    public void OnStartButtonPressed()
    {
        GameManager.Instance.StartGame();
    }

    // Score UI ������Ʈ
    public void UpdateScoreUI(int score)
    {
        if (scoreText != null)
            scoreText.text = $"����: {score}";
    }

    // HighScore UI ������Ʈ
    private void UpdateHighScoreUI(int highScore)
    {
        if (highScoreText != null)
            highScoreText.text = $"�ְ���: {highScore}";
    }

    // HP Bar UI ������Ʈ (0~1)
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