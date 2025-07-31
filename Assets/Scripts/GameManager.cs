using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

// ���� ���¸� ��Ÿ���� ������
public enum GameState
{
    Prepare,    // ��Ʈ�� ��� ����
    Playing,    // �÷��� ��
    GameOver    // ���� ����
}

// ���� �帧�� ���̵� ���, ���� ó���� �����ϴ� �̱��� �Ŵ���
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }  // �̱��� �ν��Ͻ�
    public GameState CurrentState { get; private set; }       // ���� ���� ����

    [SerializeField] private float difficultyInterval = 30f;  // ���̵� ��� ����(��)
    [SerializeField] private float speedIncrement = 0.5f;     // ���̵� ��� �� �ӵ� ������

    private Coroutine difficultyRoutine;  // ���̵� ��� �ڷ�ƾ ����

    private void Awake()
    {
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // �� �ε� �ÿ��� ����
        }
        else
        {
            Destroy(gameObject);            // �ߺ� �ν��Ͻ� �ı�
            return;
        }
    }

    private void OnEnable()
    {
        // ���� �ε�� ������ �ʱ�ȭ
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ���� ������ �ε�� ���� ȣ��Ǿ� UI/���¸� �ʱ�ȭ
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ��Ʈ�� UI �����ֱ�
        UIManager.Instance.ShowIntroUI();
        // UIManager.Instance.HideInGameUI();
        // UIManager.Instance.HideGameOverUI();

        // ���� ���¸� Prepare(��Ʈ��)�� ����
        CurrentState = GameState.Prepare;
        Time.timeScale = 0f;

        // ���� �ڷ�ƾ�� ���� ������ ����
        if (difficultyRoutine != null)
            StopCoroutine(difficultyRoutine);
    }

    // "Start" ��ư���� ȣ��: �ΰ��� ����
    public void StartGame()
    {
        // UI ��ȯ
        // UIManager.Instance.HideIntroUI();
        UIManager.Instance.ShowInGameUI();

        // ���� ���� �� �ð� �帧 �簳
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;

        // ���� �ʱ�ȭ �� ���̵� ��� �ڷ�ƾ ����
        ScoreManager.Instance.ResetScore();
        difficultyRoutine = StartCoroutine(DifficultyCoroutine());
    }

    // ��ֹ� �浹 ������ ���� ���� �� ȣ��
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

    // ���� ���� �� ����� ��ư���� ȣ��
    public void RestartGame()
    {
        // �� �ε常! �ʱ�ȭ�� OnSceneLoaded���� ó��
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ���̵� ��� �ڷ�ƾ
    private IEnumerator DifficultyCoroutine()
    {
        while (CurrentState == GameState.Playing)
        {
            yield return new WaitForSeconds(difficultyInterval);
            IncreaseDifficulty();
        }
    }

    // ���� ���̵� ��� ó�� (�÷��̾� �ӵ� ����)
    public void IncreaseDifficulty()
    {
        if (PlayerController.Instance != null)
            PlayerController.Instance.moveSpeed += speedIncrement;
    }

    // UI�� "Quit" ��ư���� ȣ��
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