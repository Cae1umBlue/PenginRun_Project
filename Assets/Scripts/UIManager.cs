using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public Button quitButton;


    public TMP_Text scoreText;

    [System.Serializable]
    public class ButtonEffect
    {
        public Button button;
        public Image targetImage;
        public Sprite newSprite;
    }

    public ButtonEffect[] buttonEffects;
    public float revertDelay = 0.5f;

    private void Start()
    {
        quitButton.onClick.AddListener(QuitGame);



        foreach (var effect in buttonEffects)
        {
            Sprite original = effect.targetImage.sprite;

            // 로컬 변수 복사 필요 (클로저 방지)
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
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void UpdateScoreUI(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "점수: " + score.ToString();
        }
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
        UnityEditor.EditorApplication.isPlaying = false;  // 에디터 실행 중단
#else
        Application.Quit();  // 빌드된 게임 종료
#endif
    }


}
