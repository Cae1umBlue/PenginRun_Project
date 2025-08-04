using UnityEngine;
using UnityEngine.EventSystems;

public class SlideButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // PlayerController의 Slide 동작을 UI 버튼으로 제어
    private PlayerController player;

    private void Awake()
    {
        player = PlayerController.Instance;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 슬라이드 시작
        if (player != null)
        {
            player.StartSlideByUI();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 슬라이드 종료
        if (player != null)
        {
            player.EndSlideByUI();
        }
    }
}