using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("���� ����Ʈ �����̳�")]
    [Tooltip("Score �����۵��� �������� �ڽ� ������Ʈ�� ���� �θ�")]
    [SerializeField] private Transform scorePointsParent;
    [Tooltip("Heal �����۵��� �������� �ڽ� ������Ʈ�� ���� �θ�")]
    [SerializeField] private Transform healPointsParent;
    [Tooltip("SpeedUp �����۵��� �������� �ڽ� ������Ʈ�� ���� �θ�")]
    [SerializeField] private Transform speedUpPointsParent;
    [Tooltip("SlowDown �����۵��� �������� �ڽ� ������Ʈ�� ���� �θ�")]
    [SerializeField] private Transform slowDownPointsParent;

    [Header("������ ������")]
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
    /// ������ �θ�(�����̳�)�� ��� �ڽ� ��ġ�� prefab�� Instantiate �մϴ�.
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