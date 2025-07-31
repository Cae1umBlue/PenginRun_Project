using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    // ���ھ� ����/�ְ��� ���� �̺�Ʈ
    public event Action<int> OnScoreChanged;
    public event Action<int> OnHighScoreChanged;

    private int currentScore;
    private int highScore;

    public int CurrentScore => currentScore;
    public int HighScore => highScore;

    private void Awake()
    {
        // �̱��� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        LoadHighScore();
        ResetScore();
    }

    /// <summary>
    /// ���ھ� �ʱ�ȭ
    /// </summary>
    public void ResetScore()
    {
        currentScore = 0;
        OnScoreChanged?.Invoke(currentScore);
    }

    /// <summary>
    /// ���ھ� ����
    /// </summary>
    public void AddScore(int amount)
    {
        currentScore += amount;
        OnScoreChanged?.Invoke(currentScore);
    }

    /// <summary>
    /// �ְ� ���� ���� (���ӿ��� �� ȣ��)
    /// </summary>
    public void SaveHighScore()
    {
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            OnHighScoreChanged?.Invoke(highScore);
        }
    }

    /// <summary>
    /// ����� �ְ� ���� �ε�
    /// </summary>
    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        OnHighScoreChanged?.Invoke(highScore);
    }
}