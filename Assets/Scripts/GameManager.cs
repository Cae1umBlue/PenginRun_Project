using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        Intro,
        Playing,
        GameOver
    }
    public GameState CurrentState { get; private set; }

    public event Action<GameState> OnStateChanged;
    public event Action<float> OnHPChanged;
    public event Action<int> OnScoreChanged;
    public event Action<int> OnHighScoreChanged;

    [Header("난이도 설정")]
    [SerializeField] private float difficultyInterval = 30f;
    [SerializeField] private float speedIncrement = 0.5f;

    [Header("HP 설정")]
    [SerializeField] private float initialHP = 1f;
    [SerializeField] private float hpDecreaseSpeed = 0.01f; // 초당 HP 자동 감소량
    [SerializeField] private float damageAmount = 0.2f;  // 피격 시 기본 감소량

    private float currentHP;
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
            return;
        }
    }

    private void Start()
    {
        SetState(GameState.Intro);

        ScoreManager.Instance.OnScoreChanged += s => OnScoreChanged?.Invoke(s);
        ScoreManager.Instance.OnHighScoreChanged += hs => OnHighScoreChanged?.Invoke(hs);
    }

    private void Update()
    {
        if (CurrentState != GameState.Playing) return;

        // 매 프레임 자동으로 감소시키려면 hpDecreaseSpeed * deltaTime만큼 전달
        DecreaseHP(hpDecreaseSpeed * Time.deltaTime);
    }

    public void StartGame()
    {
        ScoreManager.Instance.ResetScore();

        currentHP = initialHP;
        OnHPChanged?.Invoke(currentHP);

        Time.timeScale = 1f;

        if (difficultyRoutine != null)
            StopCoroutine(difficultyRoutine);
        difficultyRoutine = StartCoroutine(DifficultyCoroutine());

        SetState(GameState.Playing);
    }

    public void GameOver()
    {
        if (CurrentState != GameState.Playing) return;

        if (difficultyRoutine != null)
            StopCoroutine(difficultyRoutine);

        ScoreManager.Instance.SaveHighScore();
        OnHighScoreChanged?.Invoke(ScoreManager.Instance.HighScore);

        Time.timeScale = 0f;
        SetState(GameState.GameOver);
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

    /// <summary>
    /// amount > 0 : 그만큼 HP 감소  
    /// amount < 0 : 피격 데미지(damageAmount) 적용  
    /// </summary>
    public void DecreaseHP(float amount = -1f)
    {
        float delta = (amount < 0f) ? damageAmount : amount;
        currentHP = Mathf.Clamp01(currentHP - delta);
        OnHPChanged?.Invoke(currentHP);

        if (currentHP <= 0f)
            GameOver();
    }

    public void IncreaseHP(float amount)
    {
        currentHP = Mathf.Clamp01(currentHP + amount);
        OnHPChanged?.Invoke(currentHP);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void SetState(GameState newState)
    {
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
    }
}