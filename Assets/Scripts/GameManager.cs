using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // 1) ���� �߰��� ���� ������Ʈ: Intro �ܰ� ����
    public enum GameState
    {
        Intro,      // �غ� ȭ�� (��Ʈ��)
        Playing,    // �÷��� ��
        GameOver    // ���� ����
    }
    public GameState CurrentState { get; private set; }

    // 2) UIManager � ������ �̺�Ʈ��
    public event Action<GameState> OnStateChanged;
    public event Action<float> OnHPChanged;
    public event Action<int>   OnScoreChanged;
    public event Action<int>   OnHighScoreChanged;

    [Header("���̵� ����")]
    [SerializeField] private float difficultyInterval = 30f;
    [SerializeField] private float speedIncrement      = 0.5f;

    [Header("HP ����")]
    [SerializeField] private float initialHP         = 1f;
    [SerializeField] private float hpDecreaseSpeed   = 0.01f; // �ʴ� HP ���ҷ�
    [SerializeField] private float damageAmount      = 0.2f;  // �ǰ� �� �⺻ ���ҷ�

    private float   currentHP;
    private Coroutine difficultyRoutine;

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
        // �ʱ� ������Ʈ: Intro
        SetState(GameState.Intro);

        // ScoreManager �̺�Ʈ ���� �� GameManager �̺�Ʈ�� �߰�
        ScoreManager.Instance.OnScoreChanged     += s  => OnScoreChanged?.Invoke(s);
        ScoreManager.Instance.OnHighScoreChanged += hs => OnHighScoreChanged?.Invoke(hs);
    }

    private void Update()
    {
        // 3) Playing ���¿����� HP ���� ó��
        if (CurrentState != GameState.Playing) return;

        DecreaseHP(); // �� ������ �⺻ ����
    }

    /// <summary>
    /// Start ��ư ������ �� ȣ��
    /// </summary>
    public void StartGame()
    {
        // ���� �ʱ�ȭ
        ScoreManager.Instance.ResetScore();

        // HP �ʱ�ȭ �� �˸�
        currentHP = initialHP;
        OnHPChanged?.Invoke(currentHP);

        // �ð� �帧 ����
        Time.timeScale = 1f;

        // ���̵� ��� �ڷ�ƾ �����
        if (difficultyRoutine != null)
            StopCoroutine(difficultyRoutine);
        difficultyRoutine = StartCoroutine(DifficultyCoroutine());

        // ������Ʈ ��ȯ
        SetState(GameState.Playing);
    }

    /// <summary>
    /// ���� ���� ó��
    /// </summary>
    public void GameOver()
    {
        if (CurrentState != GameState.Playing) return;

        // ���̵� �ڷ�ƾ ����
        if (difficultyRoutine != null)
            StopCoroutine(difficultyRoutine);

        // �ְ� ���� ���� �� �˸�
        ScoreManager.Instance.SaveHighScore();
        OnHighScoreChanged?.Invoke(ScoreManager.Instance.HighScore);

        // �ð� ����
        Time.timeScale = 0f;

        // ������Ʈ ��ȯ
        SetState(GameState.GameOver);
    }

    /// <summary>
    /// ����� ��ư ������ �� ȣ��
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// ���� �ð����� ���̵� ���
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
    /// ���� ���̵� ��� ó�� (�÷��̾� �ӵ� ����)
    /// </summary>
    public void IncreaseDifficulty()
    {
        if (PlayerController.Instance != null)
            PlayerController.Instance.moveSpeed += speedIncrement;
    }

    /// <summary>
    /// HP ���� (amount ���� ȣ�� �� �⺻ damageAmount ����, 
    /// amount ���� �� �ش� ����ŭ ����)
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
    /// HP ���� (�� ������ ��� ȣ��)
    /// </summary>
    public void IncreaseHP(float amount)
    {
        currentHP = Mathf.Clamp01(currentHP + amount);
        OnHPChanged?.Invoke(currentHP);
    }

    /// <summary>
    /// ��/������ ����
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
    /// ���ο�: ���� ���� �� �̺�Ʈ ȣ��
    /// </summary>
    private void SetState(GameState newState)
    {
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
    }
}