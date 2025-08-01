using UnityEngine;
using UnityEngine.EventSystems;

public class SlideButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // PlayerController�� Slide ������ UI ��ư���� ����
    private PlayerController player;

    private void Awake()
    {
        player = PlayerController.Instance;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // �����̵� ����
        if (player != null)
        {
            player.StartSlideByUI();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // �����̵� ����
        if (player != null)
        {
            player.EndSlideByUI();
        }
    }
}