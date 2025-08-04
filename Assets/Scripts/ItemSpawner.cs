using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("스폰 포인트 컨테이너")]
    [Tooltip("Score 아이템들의 포지션을 자식 오브젝트로 가진 부모")]
    [SerializeField] private Transform scorePointsParent;
    [Tooltip("Heal 아이템들의 포지션을 자식 오브젝트로 가진 부모")]
    [SerializeField] private Transform healPointsParent;
    [Tooltip("SpeedUp 아이템들의 포지션을 자식 오브젝트로 가진 부모")]
    [SerializeField] private Transform speedUpPointsParent;
    [Tooltip("SlowDown 아이템들의 포지션을 자식 오브젝트로 가진 부모")]
    [SerializeField] private Transform slowDownPointsParent;

    [Header("아이템 프리팹")]
    [SerializeField] private GameObject scoreItemPrefab;
    [SerializeField] private GameObject healItemPrefab;
    [SerializeField] private GameObject speedUpItemPrefab;
    [SerializeField] private GameObject slowDownItemPrefab;

    private void Start()
    {
        SpawnFromParent(scorePointsParent, scoreItemPrefab);
        SpawnFromParent(healPointsParent, healItemPrefab);
        SpawnFromParent(speedUpPointsParent, speedUpItemPrefab);
        SpawnFromParent(slowDownPointsParent, slowDownItemPrefab);
    }

    /// <summary>
    /// 지정된 부모(컨테이너)의 모든 자식 위치에 prefab을 Instantiate 합니다.
    /// </summary>
    private void SpawnFromParent(Transform parent, GameObject prefab)
    {
        if (parent == null || prefab == null)
            return;

        foreach (Transform point in parent)
        {
            Instantiate(prefab, point.position, Quaternion.identity, transform);
        }
    }
}