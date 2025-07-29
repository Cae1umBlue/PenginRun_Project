using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ���� ���� �� �ְ� ���� ���塤�ε带 ����ϴ� �̱��� �Ŵ���
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }  // �̱��� �ν��Ͻ�

    public int CurrentScore { get; private set; }              // ���� ����
    public int HighScore { get; private set; }              // ����� �ְ� ����

    private const string HighScoreKey = "HighScore";           // PlayerPrefs Ű

    private void Awake()
    {
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // �� ��ȯ �ÿ��� �ı����� ����
        }
        else
        {
            Destroy(gameObject);            // �ߺ� �ν��Ͻ� ����
            return;
        }
    }

    private void Start()
    {
        // ���� ���� �� �ְ� ���� �ε�
        LoadHighScore();
        // ���� �ʱ�ȭ �� UI ����
        ResetScore();
    }

    // ������ �߰��� �� ȣ�� (��: ���� ȹ�� ��)
    public void AddScore(int amount)
    {
        CurrentScore += amount;
        // ���� ���� �� UI ����
        UIManager.Instance.UpdateScoreUI(CurrentScore);
    }

    // ������ 0���� ������ �� ȣ�� (���� ���ۡ������)
    public void ResetScore()
    {
        CurrentScore = 0;
        // ���µ� ���� UI ����
        UIManager.Instance.UpdateScoreUI(CurrentScore);
    }

    // ����� �ְ� ������ �ҷ��� �� ȣ�� (������ 0)
    public void LoadHighScore()
    {
        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        // �ҷ��� �ְ��� UI ����
        UIManager.Instance.UpdateHighScoreUI(HighScore);
    }

    // ���� ������ �ְ� ������ ���� ������ �� ȣ��
    public void SaveHighScore()
    {
        if (CurrentScore > HighScore)
        {
            HighScore = CurrentScore;
            PlayerPrefs.SetInt(HighScoreKey, HighScore);
            PlayerPrefs.Save();
            // ����� �ְ��� UI ����
            UIManager.Instance.UpdateHighScoreUI(HighScore);
        }
    }
}