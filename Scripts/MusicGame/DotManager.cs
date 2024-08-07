using RhythmTool;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// The dot manager for the level, which is a singleton.
/// </summary>
[RequireComponent(typeof(ObjectPool))]
[RequireComponent(typeof(AudioSource))]
public class DotManager : Singleton<DotManager>
{
    /// <summary>
    /// Whether to show the index above each dot.
    /// </summary>
    public bool ShowDotIndex = false;

    /// <summary>
    /// The list of dots currently onscreen.
    /// </summary>
    [HideInInspector]
    public List<MoveDots> CurrentDots;

    /// <summary>
    /// The number of dots the player has hit.
    /// </summary>
    [HideInInspector]
    public float DotsHit = 0;

    /// <summary>
    /// The index of the dot from the list that we are currently on.
    /// </summary>
    [HideInInspector]
    public int DotIndex = 0;

    /// <summary>
    /// The list of dots to actually use (whether from JSON or the editor).
    /// </summary>
    [HideInInspector]
    public List<Dot> DotsToUse;

    /// <summary>
    /// The speed of the dots.
    /// </summary>
    public int DotSpeed;

    /// <summary>
    /// The current sub-beat count.
    /// </summary>
    [HideInInspector]
    public int SubBeatNum = 0;

    /// <summary>
    /// The current combo of notes hit.
    /// </summary>
    [HideInInspector]
    public int Combo = 0;

    /// <summary>
    /// The list of dots scheduled to appear.
    /// </summary>
    [SerializeField]
    private List<Dot> dots;

    /// <summary>
    /// The filename for the Json dot list.
    /// </summary>
    [SerializeField]
    private string dotListFilename;

    /// <summary>
    /// Whether to use the Json dot list or not.
    /// </summary>
    [SerializeField]
    private bool useJson = false;

    /// <summary>
    /// The score text component.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI scoreText;

    /// <summary>
    /// The text component showing mind, body, and soul values.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI mbsText;

    /// <summary>
    /// The text component showing a completion message.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI completeText;

    /// <summary>
    /// The string to display when the player completes the minigame.
    /// </summary>
    [SerializeField]
    private string completeMessage = "You win!";

    /// <summary>
    /// The audio track to be played in the foreground, which will get muted when the player starts messing up.
    /// </summary>
    [SerializeField]
    private AudioSource foregroundTrack;

    /// <summary>
    /// The audio track to be played in the background.
    /// </summary>
    [SerializeField]
    private AudioSource backgroundTrack;

    /// <summary>
    /// The maximum mind value the player can get if they have a perfect run.
    /// </summary>
    [SerializeField]
    private int mindMax = 10;

    /// <summary>
    /// The maximum body value the player can get if they have a perfect run.
    /// </summary>
    [SerializeField]
    private int bodyMax = 10;

    /// <summary>
    /// The maximum soul value the player can get if they have a perfect run.
    /// </summary>
    [SerializeField]
    private int soulMax = 10;

    /// <summary>
    /// The number of beats before the level should begin.
    /// </summary>
    [SerializeField]
    private int initialOffset = 0;

    /// <summary>
    /// The text to display the current combo.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI comboText;

    /// <summary>
    /// The speed at which to fast forward (debug only).
    /// </summary>
    [SerializeField]
    private float fastForwardSpeed = 2f;

    /// <summary>
    /// The resume button, which will only appear when the game is paused.
    /// </summary>
    [SerializeField]
    private GameObject resumeButton;

    /// <summary>
    /// The quit button, which will only appear when the game is paused.
    /// </summary>
    [SerializeField]
    private GameObject quitButton;

    /// <summary>
    /// The pause button on the minigame screen.
    /// </summary>
    [SerializeField]
    private RectTransform pauseButton;

    /// <summary>
    /// Provides rhythm events, like OnBeat.
    /// </summary>
    [SerializeField]
    private RhythmEventProvider eventProvider;

    private ObjectPool pool;
    private int beatNum;
    private bool done = false;
    private DotList dotListFromJson;

    private void Start()
    {
        pool = GetComponent<ObjectPool>();
        backgroundTrack = GetComponent<AudioSource>();
        LoadDotList();
        DotsToUse = useJson ? dotListFromJson.dots : dots;

        if (GameManager.Instance.LeftHandMode)
        {
            pauseButton.anchorMin = new Vector2(0.8f, 0f);
            pauseButton.anchorMax = new Vector2(1f, 0.1f);
        }
        else
        {
            pauseButton.anchorMin = new Vector2(0f, 0f);
            pauseButton.anchorMax = new Vector2(0.2f, 0.1f);
        }

        eventProvider.Register<Beat>(OnSubBeat);
        StartCoroutine(PlaySong());
    }

    private void Update()
    {
        if (Input.GetButton(Constants.Input_TimeScrub) && (!GameManager.Instance.Paused))
        {
            Time.timeScale = fastForwardSpeed;
            backgroundTrack.time += (Time.deltaTime / 2);

            if (foregroundTrack != null)
            {
                foregroundTrack.time += (Time.deltaTime / 2);
            }
        }
        else if (Input.GetButtonUp(Constants.Input_TimeScrub) && (!GameManager.Instance.Paused))
        {
            Time.timeScale = 1f;
        }

        if (Input.GetButtonDown(Constants.Input_Pause))
        {
            TogglePause();
        }
    }

    /// <summary>
    /// Load the title scene.
    /// </summary>
    public void Quit()
    {
        TogglePause();
        GameManager.Instance.LoadScene(ScenesEnum.TitleScene);
    }

    /// <summary>
    /// Show the score text at the end of the level.
    /// </summary>
    public void ShowScoreText()
    {
        scoreText.SetText("Score: " + DotsHit + " / " + DotsToUse.Count);
        float dotsHitPercent = DotsHit / DotsToUse.Count;
        int mind = (int)Mathf.Round(dotsHitPercent * mindMax);
        int body = (int)Mathf.Round(dotsHitPercent * bodyMax);
        int soul = (int)Mathf.Round(dotsHitPercent * soulMax);
        mbsText.SetText("M: " + mind + " B: " + body + " S: " + soul);
        ResourceManager.Instance.mindResource += mind;
        ResourceManager.Instance.bodyResource += body;
        ResourceManager.Instance.spiritResource += soul;
        completeText.SetText(completeMessage);
    }

    /// <summary>
    /// Set whether the player is currently failing.
    /// </summary>
    /// <param name="isFailing">True if the player is failing.</param>
    public void SetFailing(bool isFailing)
    {
        if (foregroundTrack != null)
        {
            foregroundTrack.mute = isFailing;
        }
    }

    /// <summary>
    /// Set the current combo of notes hit.
    /// </summary>
    /// <param name="combo">The number to set the combo at.</param>
    public void SetCombo(int combo)
    {
        Combo = combo;
    }

    /// <summary>
    /// Pause or unpause the game.
    /// </summary>
    public void TogglePause()
    {
        if (!GameManager.Instance.Paused)
        {
            backgroundTrack.Pause();
        }
        else
        {
            backgroundTrack.UnPause();
        }

        resumeButton.SetActive(!GameManager.Instance.Paused);
        quitButton.SetActive(!GameManager.Instance.Paused);
        GameManager.Instance.Pause(!GameManager.Instance.Paused);
    }

    /// <summary>
    /// Hide the pause menu.
    /// </summary>
    public void HidePauseMenu()
    {
        quitButton.SetActive(false);
        resumeButton.SetActive(false);
    }

    private void OnSubBeat(Beat beat)
    {
        beatNum++;
        SubBeatNum++;

        // If we are on the first dot and we have reached the offset, or we have reached any subsequent dot, create a dot.
        if ((DotIndex == 0 && beatNum + initialOffset == DotsToUse[0].BeatNum)
            || (!done && DotsToUse[DotIndex].BeatNum == beatNum))
        {
            CreateDot(DotsToUse[DotIndex++].Position);
            done = (DotIndex >= DotsToUse.Count);
            beatNum = 0;
        }
    }

    private void OnSongLoaded()
    {
        StartCoroutine(PlaySong());
    }

    /// <summary>
    /// Add a dot to the screen.
    /// </summary>
    /// <param name="position">The position of the dot.</param>
    private void CreateDot(Position position)
    {
        GameObject dotObj = pool.GetFromPool(Constants.Tag_Dot);
        MoveDots dot = dotObj.GetComponent<MoveDots>();
        dot.Position = position;
        dot.ResetDot();
        CurrentDots.Add(dot);
    }

    private void OnDestroy()
    {
        eventProvider.Unregister<Beat>(OnSubBeat);
    }

    /// <summary>
    /// Play the song after a given amount of time.
    /// </summary>
    /// <returns>An IEnumerator (co-routine).</returns>
    private IEnumerator PlaySong()
    {
        yield return new WaitForSeconds(2f);
        backgroundTrack.Play();

        if (foregroundTrack != null)
        {
            foregroundTrack.Play();
        }
    }

    /// <summary>
    /// Converts the given DotList to a Json string.
    /// </summary>
    /// <param name="dots">The list of dots to turn into Json.</param>
    /// <returns>A Json string of the given dot list.</returns>
    private string ConvertToJson(DotList dots)
    {
        return JsonUtility.ToJson(dots);
    }

    /// <summary>
    /// Load the dot list from the Resources folder and convert it to a usable game object.
    /// </summary>
    private void LoadDotList()
    {
        TextAsset text = Resources.Load<TextAsset>(dotListFilename);
        dotListFromJson = JsonUtility.FromJson<DotList>(text.text);
    }
}

/// <summary>
/// The position of the dot on the screen (which track it is on).
/// </summary>
public enum Position
{
    High,
    Upper,
    Middle,
    Lower,
    Low
}