using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;



public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public Button quitButton;
    public Text scoreText;
    public Text highScoreText;
    public Image hpBarImage;

    private bool isGameOver = false;

    private float currentHP = 1f;
    private float hpDecreaseSpeed = 0.01f; // �ʴ� ���Ұ�

    private int currentScore = 0;
    private int highScore = 0;

    public static event Action OnGameRestarted;


    public void OnStartButtonPressed()
    {

        introUI.SetActive(false);   // ��Ʈ�� ����
        inGameUI.SetActive(true);   // �ΰ��� UI ǥ��
        Time.timeScale = 1f;
    }

    [System.Serializable]

    public class ButtonEffect
    {
        public Button button;
        public Image targetImage;
        public Sprite newSprite;
    }
    public ButtonEffect[] buttonEffects;
    public float revertDelay = 0.5f;


    public GameObject introUI;
    public GameObject inGameUI;
    public GameObject gameOverUI;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {


        quitButton.onClick.AddListener(QuitGame);

        foreach (var effect in buttonEffects)
        {
            Sprite original = effect.targetImage.sprite;

            // Ŭ���� ������ ���� ����
            Button localButton = effect.button;
            Image localImage = effect.targetImage;
            Sprite newSprite = effect.newSprite;

            localButton.onClick.AddListener(() =>
            {
                localImage.sprite = newSprite;
                StartCoroutine(RevertAfterDelay(localImage, original));
            });
        }
    }

    public void UpdateScoreUI(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "����: " + score.ToString();
        }
    }

    public void UpdateHighScoreUI(int highScore)
    {
        if (highScoreText != null)
            highScoreText.text = "�ְ���: " + highScore;
    }

    IEnumerator RevertAfterDelay(Image image, Sprite originalSprite)
    {
        yield return new WaitForSeconds(revertDelay);
        image.sprite = originalSprite;
    }

    public void QuitGame()
    {
        Debug.Log("���� ���� �õ�");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; //�����Ϳ��� ���� ���� ��
#else
        Application.Quit(); //���� ���忡�� ���� ������
#endif
    }

    // UI ��ȯ ����

   

    public void ShowUI(GameObject targetUI)
    {
       
        targetUI.SetActive(true);
        targetUI.transform.SetAsLastSibling();
    }

    public void UpdateHPUI(float hpRatio)
    {
        hpRatio = Mathf.Clamp01(hpRatio); // 0 ~ 1 ���̷� ����
        if (hpBarImage != null)
            hpBarImage.fillAmount = hpRatio;
    }

    private void Update()
    {
        if (!isGameOver)
        {
            ApplyDamage(hpDecreaseSpeed * Time.deltaTime);
        }
    }

    // ü�°���

    public void ApplyDamage(float amount)
    {
        if (isGameOver) return;

        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0f, 1f);

        UpdateHPUI(currentHP);

        if (currentHP <= 0f)
        {
            HandleGameOver();
        }
    }

    public void Heal(float amount)
    {
        if (isGameOver) return;

        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0f, 1f);

        UpdateHPUI(currentHP);
    }
    private void HandleGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        inGameUI.SetActive(false);
        gameOverUI.SetActive(true);
        Time.timeScale = 0f;
    }
    public void OnRestartButtonPressed()
    {
        // �ְ��� ������Ʈ
        if (currentScore > highScore)
        {
            highScore = currentScore;
            UpdateHighScoreUI(highScore);
        }

        // ���� �ʱ�ȭ
        currentScore = 0;
        UpdateScoreUI(currentScore);

        // ü�� �ʱ�ȭ
        currentHP = 1f;
        isGameOver = false;
        UpdateHPUI(currentHP);

        // UI ��ȯ
        gameOverUI.SetActive(false);
        inGameUI.SetActive(true);

        // �ð� �簳
        Time.timeScale = 1f;


    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScoreUI(currentScore);
    }

    public void ShowIntroUI() => ShowUI(introUI);
    public void ShowInGameUI() => ShowUI(inGameUI);
    public void ShowGameOverUI() => ShowUI(gameOverUI);
}
