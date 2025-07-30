using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public Button quitButton;
    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public Image hpBarImage;

    private float currentHP = 1f;
    private float hpDecreaseSpeed = 0.01f; // 초당 감소값
    private bool isDead = false;



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

            // 클로저 방지용 로컬 복사
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
            scoreText.text = "점수: " + score.ToString();
        }
    }

    public void UpdateHighScoreUI(int highScore)
    {
        if (highScoreText != null)
            highScoreText.text = "최고점: " + highScore;
    }

    IEnumerator RevertAfterDelay(Image image, Sprite originalSprite)
    {
        yield return new WaitForSeconds(revertDelay);
        image.sprite = originalSprite;
    }

    public void QuitGame()
    {
        Debug.Log("게임 종료 시도");

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; //에디터에서 종료 누를 시
        #else
        Application.Quit(); //실제 빌드에서 종료 누를시
        #endif
    }

    // UI 전환 제어

    public void HideAllUI()
    {
        introUI.SetActive(false);
        inGameUI.SetActive(false);
        gameOverUI.SetActive(false);
    }

    public void ShowUI(GameObject targetUI)
    {
        HideAllUI();
        targetUI.SetActive(true);
        targetUI.transform.SetAsLastSibling();
    }

    public void UpdateHPUI(float hpRatio)
    {
        hpRatio = Mathf.Clamp01(hpRatio); // 0 ~ 1 사이로 제한
        if (hpBarImage != null)
            hpBarImage.fillAmount = hpRatio;
    }

    private void Update()
    {
        // 임시: 시간이 지날수록 줄어듦
        currentHP -= hpDecreaseSpeed * Time.deltaTime;
        currentHP = Mathf.Max(currentHP, 0f);

        UpdateHPUI(currentHP);
    }

    

    public void ShowIntroUI() => ShowUI(introUI);
    public void ShowInGameUI() => ShowUI(inGameUI);
    public void ShowGameOverUI() => ShowUI(gameOverUI);
}
