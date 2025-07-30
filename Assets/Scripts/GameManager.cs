using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

// ���� ���¸� ��Ÿ���� ������
public enum GameState
{
    Playing,    // �÷��� ��
    GameOver    // ���� ����
}

// ���� �帧�� ���̵� ����� �����ϴ� �̱��� �Ŵ���
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }  // �̱��� �ν��Ͻ�
    public GameState CurrentState { get; private set; }       // ���� ���� ����

    [SerializeField] private float difficultyInterval = 30f;  // ���̵� ��� ����(��)
    [SerializeField] private float speedIncrement = 0.5f;   // ���̵� ��� �� �ӵ� ������

    private Coroutine difficultyRoutine;                      // ���̵� ��� �ڷ�ƾ ����

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
        }
    }

    private void Start()
    {
        // ���� ���� ���� �ٷ� �÷��� ���·� ��ȯ
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;               // �ð� �帧 ����

        // ���� �ʱ�ȭ
        ScoreManager.Instance.ResetScore();

        // ���̵� ��� �ڷ�ƾ ����
        difficultyRoutine = StartCoroutine(DifficultyCoroutine());
    }

    // ��ֹ� �浹 ������ ���� ���� �� ȣ��
    public void GameOver()
    {
        if (CurrentState != GameState.Playing) return;  // �÷��� ���� �ƴ� �� ����
        CurrentState = GameState.GameOver;              // ���� ����
        Time.timeScale = 0f;                            // �ð� �帧 ����

        // ���̵� ��� �ڷ�ƾ ����
        if (difficultyRoutine != null)
            StopCoroutine(difficultyRoutine);

        // �ְ� ���� ���� �� ���ӿ��� UI ǥ��
        ScoreManager.Instance.SaveHighScore();
        // UIManager.Instance.ShowGameOverUI();
    }

    // ���� ���� �� ����� ��ư���� ȣ��
    public void RestartGame()
    {
        Time.timeScale = 1f;                                              // �ð� �帧 ����
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);       // ���� �� ��ε�
    }

    // ���� �ð����� ���̵��� ���̴� �ڷ�ƾ
    private IEnumerator DifficultyCoroutine()
    {
        while (CurrentState == GameState.Playing)
        {
            yield return new WaitForSeconds(difficultyInterval);
            IncreaseDifficulty();  // �÷��̾� �ӵ� ����
        }
    }

    // ���� ���̵� ��� ó�� (�÷��̾� �ӵ� ����)
    public void IncreaseDifficulty()
    {
        if (PlayerController.Instance != null)
            PlayerController.Instance.moveSpeed += speedIncrement;
    }
}