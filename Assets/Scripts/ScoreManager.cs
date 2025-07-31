using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    // 스코어 변경/최고점 갱신 이벤트
    public event Action<int> OnScoreChanged;
    public event Action<int> OnHighScoreChanged;

    private int currentScore;
    private int highScore;

    public int CurrentScore => currentScore;
    public int HighScore => highScore;

    private void Awake()
    {
        // 싱글톤 설정
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
    /// 스코어 초기화
    /// </summary>
    public void ResetScore()
    {
        currentScore = 0;
        OnScoreChanged?.Invoke(currentScore);
    }

    /// <summary>
    /// 스코어 증가
    /// </summary>
    public void AddScore(int amount)
    {
        currentScore += amount;
        OnScoreChanged?.Invoke(currentScore);
    }

    /// <summary>
    /// 최고 점수 저장 (게임오버 시 호출)
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
    /// 저장된 최고 점수 로드
    /// </summary>
    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        OnHighScoreChanged?.Invoke(highScore);
    }
}