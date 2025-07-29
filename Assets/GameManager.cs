using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// ���� ���¸� ��Ÿ���� ������
public enum GameState
{
    Playing,    // �÷��� ��
    GameOver    // ���� ����
}

// ���� �帧(���ۡ������������)�� ���̵� ����� �����ϴ� �̱��� �Ŵ���
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }  // �̱��� �ν��Ͻ�
    public GameState CurrentState { get; private set; }       // ���� ���� ����

    [SerializeField] private float difficultyInterval = 30f;  // ���̵� ��� ���� (��)
    [SerializeField] private float speedIncrement = 0.5f;   // ���̵� ��� �� �ӵ� ������

    private Coroutine difficultyRoutine;                      // ���̵� �ڷ�ƾ ����

    private void Awake()
    {
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // �� ��ȯ �ÿ��� ����
        }
        else
        {
            Destroy(gameObject);            // �ߺ� �ν��Ͻ� ����
        }
    }

    private void Start()
    {
        // �ε� ���� �ٷ� ���� ����
        CurrentState = GameState.Playing;  // ���¸� Playing���� ����
        Time.timeScale = 1f;               // �ð� �帧 �簳

        // ���� �ʱ�ȭ
        ScoreManager.Instance.ResetScore();

        // ���̵� ��� �ڷ�ƾ ����
        difficultyRoutine = StartCoroutine(DifficultyCoroutine());
    }

    // ��ֹ� �浹 ������ ���� ���� �� ȣ��
    public void GameOver()
    {
        if (CurrentState != GameState.Playing) return;
        CurrentState = GameState.GameOver;
        Time.timeScale = 0f;                           // �ð� �帧 ����

        // ���̵� �ڷ�ƾ �ߴ�
        if (difficultyRoutine != null)
            StopCoroutine(difficultyRoutine);

        ScoreManager.Instance.SaveHighScore();         // �ְ� ���� ����
        UIManager.Instance.ShowGameOverUI();           // ���ӿ��� ȭ�� ǥ��
    }

    // ���� ���� �� ����� ��ư���� ȣ��
    public void RestartGame()
    {
        Time.timeScale = 1f;  // ����ŸƮ �� �ð� ����ȭ
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);  // ���� �� ��ε�
    }

    // ���� ���ݸ��� ���̵��� ���̴� �ڷ�ƾ
    private IEnumerator DifficultyCoroutine()
    {
        while (CurrentState == GameState.Playing)
        {
            yield return new WaitForSeconds(difficultyInterval);
            IncreaseDifficulty();  // �ӵ� ����
        }
    }

    // ���� ���̵�(�÷��̾� �ӵ�) ��� ó��
    public void IncreaseDifficulty()
    {
        if (PlayerController.Instance != null)
            PlayerController.Instance.moveSpeed += speedIncrement;
    }
}