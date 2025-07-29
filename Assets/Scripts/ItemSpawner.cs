using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ����Ʈ �ϳ��� ����
[System.Serializable]
public class SpawnPoint
{
    public Transform point;      // ���� ��ġ�� �� �� ������Ʈ
    public ItemType itemType;    // �� �������� ������ ������ Ÿ��
}

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private SpawnPoint[] spawnPoints;        // ���� ���� ���
    [SerializeField] private GameObject scoreItemPrefab;      // �� Ÿ�Ժ� ������
    [SerializeField] private GameObject healItemPrefab;
    [SerializeField] private GameObject speedUpItemPrefab;
    [SerializeField] private GameObject slowDownItemPrefab;

    private void Start()
    {
        SpawnAllItems();
    }

    // �� �ε� ���� �� ���� ȣ���ؼ� ������ ��ġ
    private void SpawnAllItems()
    {
        foreach (var sp in spawnPoints)
        {
            GameObject prefab = null;
            switch (sp.itemType)
            {
                case ItemType.Score: prefab = scoreItemPrefab; break;
                case ItemType.Heal: prefab = healItemPrefab; break;
                case ItemType.SpeedUp: prefab = speedUpItemPrefab; break;
                case ItemType.SlowDown: prefab = slowDownItemPrefab; break;
            }

            if (prefab != null)
                Instantiate(prefab, sp.point.position, Quaternion.identity, transform);
        }
    }
}