using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
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
        foreach (var effect in buttonEffects)
        {
            Sprite original = effect.targetImage.sprite;

            // 로컬 변수 복사 필요 (클로저 문제 방지)
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

    IEnumerator RevertAfterDelay(Image image, Sprite originalSprite)
    {
        yield return new WaitForSeconds(revertDelay);
        image.sprite = originalSprite;
    }
}
