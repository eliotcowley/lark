using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The game manager. Handles loading and unloading scenes.
/// </summary>
public class GameManager : Singleton<GameManager>
{
    /// <summary>
    /// True if the game is paused.
    /// </summary>
    [HideInInspector]
    public bool Paused = false;

    /// <summary>
    /// True if playing in left-hand mode, false if playing in right-hand mode.
    /// </summary>
    [HideInInspector]
    public bool LeftHandMode = false;

    /// <summary>
    /// The position of the line (only applicable to mini-game).
    /// </summary>
    [HideInInspector]
    public Vector3 LinePosition;

    /// <summary>
    /// Resource with all strings, can be changed as settings require
    /// </summary>
    public GameStrings PreferredGameStrings;

    /// <summary>
    /// The fader canvas prefab.
    /// </summary>
    [SerializeField]
    private GameObject m_faderPrefab;

    /// <summary>
    /// Current active portal in portal playlist
    /// </summary>
    public int ActivePortal { get; set; } = 0;

    private GameObject m_faderCanvas;   // The actual fader canvas game object.
    private Fader m_faderScript;        // The fader script attached to the fader game object.
    private int m_targetFrameRate = 60; // The target frame rate (should be 60).
    private Scene m_newScene;           // The new scene to switch to.
    private GameObject m_pauseBackground;
    private ScenesEnum m_currentScene;

    private Vector3? m_playerLocationInHome = null;

    // Prevent non-singleton constructor use.
    protected GameManager() { }

    private void Start()
    {
        PreferredGameStrings = GameStringLoader.LoadStrings(GameLanguage.English);
    }

    private void Awake()
    {
        FindOrCreateFader();
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        Application.targetFrameRate = m_targetFrameRate;
        m_currentScene = (ScenesEnum)SceneManager.GetActiveScene().buildIndex;
    }

    /// <summary>
    /// Load the given scene.
    /// </summary>
    /// <param name="scene">The scene to load.</param>
    public void LoadScene(ScenesEnum scene)
    {
        StartCoroutine(FadeOut(scene));
    }

    /// <summary>
    /// Get the last location of the player in the HomeScene. Can return null.
    /// </summary>
    public Vector3? GetPlayerLocationTransform()
    {
        return m_playerLocationInHome;
    }

    /// <summary>
    /// Set the last location of the player in the HomeScene. Can set it to null.
    /// </summary>
    public void SetPlayerLocationTransform(Vector3? location)
    {
        Debug.Log("Setting player location in home " + location.ToString());
        m_playerLocationInHome = location;
    }

    /// <summary>
    /// Pause or unpause the game.
    /// </summary>
    /// <param name="pause">If true, pauses the game; otherwise, unpauses.</param>
    public void Pause(bool pause)
    {
        Time.timeScale = pause ? 0f : 1f;
        Paused = pause;
        m_pauseBackground.SetActive(pause);
    }

    /// <summary>
    /// Restart the current level.
    /// </summary>
    public void RestartLevel()
    {
        LoadScene(m_currentScene);
    }

    /// <summary>
    /// Fade out, then load the given scene.
    /// </summary>
    /// <param name="scene">The scene to load.</param>
    /// <returns>An IEnumerator (this method waits a given amount of time).</returns>
    private IEnumerator FadeOut(ScenesEnum scene)
    {
        m_faderScript.FadeOut();
        yield return new WaitForSeconds(m_faderScript.FadeSeconds);
        SceneManager.LoadSceneAsync((int)scene, LoadSceneMode.Additive);
    }

    /// <summary>
    /// Occurs when a scene is unloaded. Sets the new active scene and fades in.
    /// </summary>
    /// <param name="_scene">The scene that was unloaded.</param>
    private void OnSceneUnloaded(Scene _scene)
    {
        SceneManager.SetActiveScene(m_newScene);
        m_currentScene = (ScenesEnum)m_newScene.buildIndex;
        m_faderScript.FadeIn();
    }

    /// <summary>
    /// Occurs when a scene is loaded. Unloads the currently active scene.
    /// </summary>
    /// <param name="_scene">The scene that was loaded.</param>
    /// <param name="_loadSceneMode">The mode in which the scene was loaded.</param>
    private void OnSceneLoaded(Scene _scene, LoadSceneMode _loadSceneMode)
    {
        m_newScene = _scene;
        Scene currentScene = SceneManager.GetActiveScene();

        if (SceneManager.sceneCount > 1)
        {
            SceneManager.UnloadSceneAsync(currentScene);
        }
    }

    /// <summary>
    /// Find the fader object in the scene, or create it if it doesn't exist.
    /// </summary>
    private void FindOrCreateFader()
    {
        if (m_faderCanvas == null)
        {
            m_faderCanvas = GameObject.FindGameObjectWithTag(Constants.Tag_Fader);

            if (m_faderCanvas == null)
            {
                if (m_faderPrefab == null)
                {
                    m_faderPrefab = Resources.Load<GameObject>(Constants.Prefab_Fader);
                }

                m_faderCanvas = Instantiate(m_faderPrefab, gameObject.transform);
                m_faderScript = m_faderCanvas.GetComponentInChildren<Fader>();
                m_pauseBackground = m_faderCanvas.transform.Find(Constants.Name_PauseBackground).gameObject;
            }
        }
    }
}
