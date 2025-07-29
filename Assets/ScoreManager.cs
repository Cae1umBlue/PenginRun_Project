using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 점수 누적 관리 및 최고 점수 저장·로드를 담당하는 싱글톤 매니저
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }  // 싱글톤 인스턴스

    public int CurrentScore { get; private set; }              // 현재 점수
    public int HighScore { get; private set; }              // 저장된 최고 점수

    private const string HighScoreKey = "HighScore";           // PlayerPrefs 키

    private void Awake()
    {
        // 싱글톤 패턴 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 씬 전환 시에도 파괴되지 않음
        }
        else
        {
            Destroy(gameObject);            // 중복 인스턴스 제거
            return;
        }
    }

    private void Start()
    {
        // 게임 시작 시 최고 점수 로드
        LoadHighScore();
        // 점수 초기화 및 UI 갱신
        ResetScore();
    }

    // 점수를 추가할 때 호출 (예: 코인 획득 시)
    public void AddScore(int amount)
    {
        CurrentScore += amount;
        // 점수 변경 시 UI 갱신
        UIManager.Instance.UpdateScoreUI(CurrentScore);
    }

    // 점수를 0으로 리셋할 때 호출 (게임 시작·재시작)
    public void ResetScore()
    {
        CurrentScore = 0;
        // 리셋된 점수 UI 갱신
        UIManager.Instance.UpdateScoreUI(CurrentScore);
    }

    // 저장된 최고 점수를 불러올 때 호출 (없으면 0)
    public void LoadHighScore()
    {
        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        // 불러온 최고점 UI 갱신
        UIManager.Instance.UpdateHighScoreUI(HighScore);
    }

    // 현재 점수를 최고 점수와 비교해 저장할 때 호출
    public void SaveHighScore()
    {
        if (CurrentScore > HighScore)
        {
            HighScore = CurrentScore;
            PlayerPrefs.SetInt(HighScoreKey, HighScore);
            PlayerPrefs.Save();
            // 저장된 최고점 UI 갱신
            UIManager.Instance.UpdateHighScoreUI(HighScore);
        }
    }
}