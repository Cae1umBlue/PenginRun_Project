using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

// 게임 상태를 나타내는 열거형
public enum GameState
{
    Prepare,    // 인트로 대기 상태
    Playing,    // 플레이 중
    GameOver    // 게임 오버
}

// 게임 흐름과 난이도 상승, 종료 처리를 관리하는 싱글톤 매니저
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }  // 싱글톤 인스턴스
    public GameState CurrentState { get; private set; }       // 현재 게임 상태

    [SerializeField] private float difficultyInterval = 30f;  // 난이도 상승 간격(초)
    [SerializeField] private float speedIncrement = 0.5f;     // 난이도 상승 시 속도 증가량

    private Coroutine difficultyRoutine;  // 난이도 상승 코루틴 참조

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
            return;
        }
    }

    private void OnEnable()
    {
        // 씬이 로드될 때마다 초기화
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 완전히 로드된 직후 호출되어 UI/상태를 초기화
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 인트로 UI 보여주기
        UIManager.Instance.ShowIntroUI();
        // UIManager.Instance.HideInGameUI();
        // UIManager.Instance.HideGameOverUI();

        // 게임 상태를 Prepare(인트로)로 설정
        CurrentState = GameState.Prepare;
        Time.timeScale = 0f;

        // 기존 코루틴이 돌고 있으면 중지
        if (difficultyRoutine != null)
            StopCoroutine(difficultyRoutine);
    }

    // "Start" 버튼에서 호출: 인게임 시작
    public void StartGame()
    {
        // UI 전환
        // UIManager.Instance.HideIntroUI();
        UIManager.Instance.ShowInGameUI();

        // 게임 상태 및 시간 흐름 재개
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;

        // 점수 초기화 및 난이도 상승 코루틴 시작
        ScoreManager.Instance.ResetScore();
        difficultyRoutine = StartCoroutine(DifficultyCoroutine());
    }

    // 장애물 충돌 등으로 게임 오버 시 호출
    public void GameOver()
    {
        if (CurrentState != GameState.Playing) return;

        CurrentState = GameState.GameOver;
        Time.timeScale = 0f;

        if (difficultyRoutine != null)
            StopCoroutine(difficultyRoutine);

        ScoreManager.Instance.SaveHighScore();
        UIManager.Instance.ShowGameOverUI();
    }

    // 게임 오버 후 재시작 버튼에서 호출
    public void RestartGame()
    {
        // 씬 로드만! 초기화는 OnSceneLoaded에서 처리
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 난이도 상승 코루틴
    private IEnumerator DifficultyCoroutine()
    {
        while (CurrentState == GameState.Playing)
        {
            yield return new WaitForSeconds(difficultyInterval);
            IncreaseDifficulty();
        }
    }

    // 실제 난이도 상승 처리 (플레이어 속도 증가)
    public void IncreaseDifficulty()
    {
        if (PlayerController.Instance != null)
            PlayerController.Instance.moveSpeed += speedIncrement;
    }

    // UI의 "Quit" 버튼에서 호출
    public void QuitGame()
    {
        Debug.Log("게임 종료 시도");
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}