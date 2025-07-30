using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

// 게임 상태를 나타내는 열거형
public enum GameState
{
    Ready,      // 시작 전 준비 단계 (인트로 UI 노출)
    Playing,    // 플레이 중
    GameOver    // 게임 오버
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState CurrentState { get; private set; }

    [SerializeField] private float difficultyInterval = 30f;
    [SerializeField] private float speedIncrement = 0.5f;

    private Coroutine difficultyRoutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 준비 단계로 진입
        CurrentState = GameState.Ready;
        Time.timeScale = 0f;

        // 인트로 UI만 노출
        UIManager.Instance.ShowIntroUI();
    }

    /// <summary>
    /// 인트로 Start 버튼에 연결
    /// </summary>
    public void StartGame()
    {
        // UI 전환
        UIManager.Instance.ShowInGameUI();

        // 점수 초기화
        ScoreManager.Instance.ResetScore();

        // 실제 플레이 상태로 진입
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;

        // 난이도 상승 코루틴 시작
        difficultyRoutine = StartCoroutine(DifficultyCoroutine());
    }

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

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator DifficultyCoroutine()
    {
        while (CurrentState == GameState.Playing)
        {
            yield return new WaitForSeconds(difficultyInterval);
            IncreaseDifficulty();
        }
    }

    public void IncreaseDifficulty()
    {
        if (PlayerController.Instance != null)
            PlayerController.Instance.moveSpeed += speedIncrement;
    }

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