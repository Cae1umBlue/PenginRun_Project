using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

// ���� ���¸� ��Ÿ���� ������
public enum GameState
{
    Ready,      // ���� �� �غ� �ܰ� (��Ʈ�� UI ����)
    Playing,    // �÷��� ��
    GameOver    // ���� ����
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
        // �غ� �ܰ�� ����
        CurrentState = GameState.Ready;
        Time.timeScale = 0f;

        // ��Ʈ�� UI�� ����
        UIManager.Instance.ShowIntroUI();
    }

    /// <summary>
    /// ��Ʈ�� Start ��ư�� ����
    /// </summary>
    public void StartGame()
    {
        // UI ��ȯ
        UIManager.Instance.ShowInGameUI();

        // ���� �ʱ�ȭ
        ScoreManager.Instance.ResetScore();

        // ���� �÷��� ���·� ����
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;

        // ���̵� ��� �ڷ�ƾ ����
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
        Debug.Log("���� ���� �õ�");
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}