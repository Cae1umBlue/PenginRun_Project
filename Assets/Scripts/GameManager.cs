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

    [Header("���̵� ����")]
    [SerializeField] private float difficultyInterval = 30f;
    [SerializeField] private float speedIncrement = 0.5f;

    [Header("HP ����")]
    [SerializeField] private float initialHP = 1f;
    [SerializeField] private float hpDecreaseSpeed = 0.01f; // �ʴ� HP �ڵ� ���ҷ�
    [SerializeField] private float damageAmount = 0.2f;  // �ǰ� �� �⺻ ���ҷ�

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

        // �� ������ �ڵ����� ���ҽ�Ű���� hpDecreaseSpeed * deltaTime��ŭ ����
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
    /// amount > 0 : �׸�ŭ HP ����  
    /// amount < 0 : �ǰ� ������(damageAmount) ����  
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