using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 게임 상태를 나타내는 열거형
public enum GameState
{
    Playing,    // 플레이 중
    GameOver    // 게임 오버
}

// 게임 흐름(시작·오버·재시작)과 난이도 상승을 관리하는 싱글톤 매니저
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }  // 싱글톤 인스턴스
    public GameState CurrentState { get; private set; }       // 현재 게임 상태

    [SerializeField] private float difficultyInterval = 30f;  // 난이도 상승 간격 (초)
    [SerializeField] private float speedIncrement = 0.5f;   // 난이도 상승 시 속도 증가량

    private Coroutine difficultyRoutine;                      // 난이도 코루틴 참조

    private void Awake()
    {
        // 싱글톤 패턴 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 씬 전환 시에도 유지
        }
        else
        {
            Destroy(gameObject);            // 중복 인스턴스 제거
        }
    }

    private void Start()
    {
        // 로딩 직후 바로 게임 시작
        CurrentState = GameState.Playing;  // 상태를 Playing으로 설정
        Time.timeScale = 1f;               // 시간 흐름 재개

        // 점수 초기화
        ScoreManager.Instance.ResetScore();

        // 난이도 상승 코루틴 시작
        difficultyRoutine = StartCoroutine(DifficultyCoroutine());
    }

    // 장애물 충돌 등으로 게임 오버 시 호출
    public void GameOver()
    {
        if (CurrentState != GameState.Playing) return;
        CurrentState = GameState.GameOver;
        Time.timeScale = 0f;                           // 시간 흐름 멈춤

        // 난이도 코루틴 중단
        if (difficultyRoutine != null)
            StopCoroutine(difficultyRoutine);

        ScoreManager.Instance.SaveHighScore();         // 최고 점수 저장
        UIManager.Instance.ShowGameOverUI();           // 게임오버 화면 표시
    }

    // 게임 오버 후 재시작 버튼에서 호출
    public void RestartGame()
    {
        Time.timeScale = 1f;  // 리스타트 전 시간 정상화
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);  // 현재 씬 재로드
    }

    // 일정 간격마다 난이도를 높이는 코루틴
    private IEnumerator DifficultyCoroutine()
    {
        while (CurrentState == GameState.Playing)
        {
            yield return new WaitForSeconds(difficultyInterval);
            IncreaseDifficulty();  // 속도 증가
        }
    }

    // 실제 난이도(플레이어 속도) 상승 처리
    public void IncreaseDifficulty()
    {
        if (PlayerController.Instance != null)
            PlayerController.Instance.moveSpeed += speedIncrement;
    }
}