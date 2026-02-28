using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Managed MonoBehaviour that reads ECS singletons and updates UI.
/// Attach to a Canvas GameObject.
/// </summary>
public class GameUIController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI WaveText;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI CastleHPText;
    public Slider CastleHPBar;
    public TextMeshProUGUI EnemiesAliveText;
    public GameObject GameOverPanel;
    public TextMeshProUGUI GameOverText;
    public Button RestartButton;

    [Header("Settings")]
    public float UIUpdateInterval = 0.1f;

    private float _updateTimer;
    private EntityManager _em;
    private bool _initialized;

    private void Start()
    {
        if (GameOverPanel != null)
            GameOverPanel.SetActive(false);

        if (RestartButton != null)
            RestartButton.onClick.AddListener(OnRestartClicked);
    }

    private void Update()
    {
        // Lazy init - wait for World to be ready
        if (!_initialized)
        {
            if (World.DefaultGameObjectInjectionWorld == null) return;
            _em = World.DefaultGameObjectInjectionWorld.EntityManager;
            _initialized = true;
        }

        _updateTimer -= Time.deltaTime;
        if (_updateTimer > 0f) return;
        _updateTimer = UIUpdateInterval;

        UpdateUI();
    }

    private void UpdateUI()
    {
        // Read GameState singleton
        var gsQuery = _em.CreateEntityQuery(typeof(GameState));
        if (gsQuery.CalculateEntityCount() == 0) return;

        var gameState = gsQuery.GetSingleton<GameState>();
        var waveManager = _em.GetComponentData<WaveManager>(gsQuery.GetSingletonEntity());

        // Wave info
        if (WaveText != null)
            WaveText.text = $"Wave {waveManager.CurrentWave}/{waveManager.TotalWaves}";

        // Score
        if (ScoreText != null)
            ScoreText.text = $"Score: {gameState.Score}";

        // Enemies alive
        if (EnemiesAliveText != null)
            EnemiesAliveText.text = $"Enemies: {waveManager.EnemiesAlive}";

        // Castle HP
        var castleQuery = _em.CreateEntityQuery(typeof(CastleTag), typeof(Health));
        if (castleQuery.CalculateEntityCount() > 0)
        {
            var castleHealth = castleQuery.GetSingleton<Health>();

            if (CastleHPText != null)
                CastleHPText.text = $"Castle: {(int)castleHealth.CurrentHealth}/{(int)castleHealth.Max}";

            if (CastleHPBar != null)
                CastleHPBar.value = math.clamp(castleHealth.CurrentHealth / castleHealth.Max, 0f, 1f);
        }

        // Game Over
        if (gameState.IsGameOver && GameOverPanel != null)
        {
            GameOverPanel.SetActive(true);
            if (GameOverText != null)
                GameOverText.text = gameState.IsVictory ? "VICTORY!" : "DEFEAT!";
        }
    }

    private void OnRestartClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}