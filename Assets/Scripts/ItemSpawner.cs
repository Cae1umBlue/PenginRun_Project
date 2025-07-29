using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스폰 포인트 하나를 정의
[System.Serializable]
public class SpawnPoint
{
    public Transform point;      // 씬에 배치해 둔 빈 오브젝트
    public ItemType itemType;    // 이 지점에서 생성할 아이템 타입
}

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private SpawnPoint[] spawnPoints;        // 스폰 지점 목록
    [SerializeField] private GameObject scoreItemPrefab;      // 각 타입별 프리팹
    [SerializeField] private GameObject healItemPrefab;
    [SerializeField] private GameObject speedUpItemPrefab;
    [SerializeField] private GameObject slowDownItemPrefab;

    private void Start()
    {
        SpawnAllItems();
    }

    // 씬 로드 직후 한 번만 호출해서 아이템 배치
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