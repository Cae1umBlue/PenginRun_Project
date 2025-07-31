using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // 1) 새로 추가된 게임 스테이트: Intro 단계 포함
    public enum GameState
    {
        Intro,      // 준비 화면 (인트로)
        Playing,    // 플레이 중
        GameOver    // 게임 오버
    }
    public GameState CurrentState { get; private set; }

    // 2) UIManager 등에 전파할 이벤트들
    public event Action<GameState> OnStateChanged;
    public event Action<float> OnHPChanged;
    public event Action<int>   OnScoreChanged;
    public event Action<int>   OnHighScoreChanged;

    [Header("난이도 설정")]
    [SerializeField] private float difficultyInterval = 30f;
    [SerializeField] private float speedIncrement      = 0.5f;

    [Header("HP 설정")]
    [SerializeField] private float initialHP         = 1f;
    [SerializeField] private float hpDecreaseSpeed   = 0.01f; // 초당 HP 감소량
    [SerializeField] private float damageAmount      = 0.2f;  // 피격 시 기본 감소량

    private float   currentHP;
    private Coroutine difficultyRoutine;

    private void Awake()
    {
        // 싱글톤 패턴
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
        // 초기 스테이트: Intro
        SetState(GameState.Intro);

        // ScoreManager 이벤트 연결 → GameManager 이벤트로 중계
        ScoreManager.Instance.OnScoreChanged     += s  => OnScoreChanged?.Invoke(s);
        ScoreManager.Instance.OnHighScoreChanged += hs => OnHighScoreChanged?.Invoke(hs);
    }

    private void Update()
    {
        // 3) Playing 상태에서만 HP 감소 처리
        if (CurrentState != GameState.Playing) return;

        DecreaseHP(); // 매 프레임 기본 감소
    }

    /// <summary>
    /// Start 버튼 눌렀을 때 호출
    /// </summary>
    public void StartGame()
    {
        // 점수 초기화
        ScoreManager.Instance.ResetScore();

        // HP 초기화 및 알림
        currentHP = initialHP;
        OnHPChanged?.Invoke(currentHP);

        // 시간 흐름 복구
        Time.timeScale = 1f;

        // 난이도 상승 코루틴 재시작
        if (difficultyRoutine != null)
            StopCoroutine(difficultyRoutine);
        difficultyRoutine = StartCoroutine(DifficultyCoroutine());

        // 스테이트 전환
        SetState(GameState.Playing);
    }

    /// <summary>
    /// 게임 오버 처리
    /// </summary>
    public void GameOver()
    {
        if (CurrentState != GameState.Playing) return;

        // 난이도 코루틴 정지
        if (difficultyRoutine != null)
            StopCoroutine(difficultyRoutine);

        // 최고 점수 저장 및 알림
        ScoreManager.Instance.SaveHighScore();
        OnHighScoreChanged?.Invoke(ScoreManager.Instance.HighScore);

        // 시간 멈춤
        Time.timeScale = 0f;

        // 스테이트 전환
        SetState(GameState.GameOver);
    }

    /// <summary>
    /// 재시작 버튼 눌렀을 때 호출
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// 일정 시간마다 난이도 상승
    /// </summary>
    private IEnumerator DifficultyCoroutine()
    {
        while (CurrentState == GameState.Playing)
        {
            yield return new WaitForSeconds(difficultyInterval);
            IncreaseDifficulty();
        }
    }

    /// <summary>
    /// 실제 난이도 상승 처리 (플레이어 속도 증가)
    /// </summary>
    public void IncreaseDifficulty()
    {
        if (PlayerController.Instance != null)
            PlayerController.Instance.moveSpeed += speedIncrement;
    }

    /// <summary>
    /// HP 감소 (amount 없음 호출 시 기본 damageAmount 감소, 
    /// amount 지정 시 해당 값만큼 감소)
    /// </summary>
    public void DecreaseHP(float amount = -1f)
    {
        float delta = (amount < 0f) ? hpDecreaseSpeed * Time.deltaTime : amount;
        currentHP = Mathf.Clamp01(currentHP - delta);
        OnHPChanged?.Invoke(currentHP);

        if (currentHP <= 0f)
            GameOver();
    }

    /// <summary>
    /// HP 증가 (힐 아이템 등에서 호출)
    /// </summary>
    public void IncreaseHP(float amount)
    {
        currentHP = Mathf.Clamp01(currentHP + amount);
        OnHPChanged?.Invoke(currentHP);
    }

    /// <summary>
    /// 앱/에디터 종료
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// 내부용: 상태 변경 및 이벤트 호출
    /// </summary>
    private void SetState(GameState newState)
    {
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
    }
}