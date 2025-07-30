using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

// 게임 상태를 나타내는 열거형
public enum GameState
{
    Playing,    // 플레이 중
    GameOver    // 게임 오버
}

// 게임 흐름과 난이도 상승을 관리하는 싱글톤 매니저
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }  // 싱글톤 인스턴스
    public GameState CurrentState { get; private set; }       // 현재 게임 상태

    [SerializeField] private float difficultyInterval = 30f;  // 난이도 상승 간격(초)
    [SerializeField] private float speedIncrement = 0.5f;   // 난이도 상승 시 속도 증가량

    private Coroutine difficultyRoutine;                      // 난이도 상승 코루틴 참조

    private void Awake()
    {
        // 싱글톤 패턴 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 씬 로드 시에도 유지
        }
        else
        {
            Destroy(gameObject);            // 중복 인스턴스 파괴
        }
    }

    private void Start()
    {
        // 게임 시작 직후 바로 플레이 상태로 전환
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;               // 시간 흐름 정상

        // 점수 초기화
        ScoreManager.Instance.ResetScore();

        // 난이도 상승 코루틴 시작
        difficultyRoutine = StartCoroutine(DifficultyCoroutine());
    }

    // 장애물 충돌 등으로 게임 오버 시 호출
    public void GameOver()
    {
        if (CurrentState != GameState.Playing) return;  // 플레이 중이 아닐 때 무시
        CurrentState = GameState.GameOver;              // 상태 변경
        Time.timeScale = 0f;                            // 시간 흐름 멈춤

        // 난이도 상승 코루틴 종료
        if (difficultyRoutine != null)
            StopCoroutine(difficultyRoutine);

        // 최고 점수 저장 및 게임오버 UI 표시
        ScoreManager.Instance.SaveHighScore();
        // UIManager.Instance.ShowGameOverUI();
    }

    // 게임 오버 후 재시작 버튼에서 호출
    public void RestartGame()
    {
        Time.timeScale = 1f;                                              // 시간 흐름 복구
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);       // 현재 씬 재로드
    }

    // 일정 시간마다 난이도를 높이는 코루틴
    private IEnumerator DifficultyCoroutine()
    {
        while (CurrentState == GameState.Playing)
        {
            yield return new WaitForSeconds(difficultyInterval);
            IncreaseDifficulty();  // 플레이어 속도 증가
        }
    }

    // 실제 난이도 상승 처리 (플레이어 속도 증가)
    public void IncreaseDifficulty()
    {
        if (PlayerController.Instance != null)
            PlayerController.Instance.moveSpeed += speedIncrement;
    }
}