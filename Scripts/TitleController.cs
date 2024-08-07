using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleController : MonoBehaviour
{
    [SerializeField]
    private Animator titleTextAnimator;

    [SerializeField]
    private Animator startButtonAnimator;

    [SerializeField]
    private Animator startTextAnimator;

    [SerializeField]
    private GameObject chooseHandednessObject;

    [SerializeField]
    private GameObject poemObject;

    [SerializeField]
    private GameObject playerObject;

    [SerializeField]
    private GameObject lighthouseObject;

    [SerializeField]
    private GameObject blurObject;

    [SerializeField]
    private GameObject minigamePartsObject;

    [SerializeField]
    private float titleToPoemPause = 1f;

    [SerializeField]
    private float pauseBeforeGame = 2f;

    [SerializeField]
    private float timeBetweenPoemLines = 2f;

    [SerializeField]
    private float pauseBeforeShowStartButton = 6f;

    [SerializeField]
    private float oceanVolumeWhenGameStarts = 0.15f;

    [SerializeField]
    private float alarmEndVolume = 0.25f;

    [SerializeField]
    private float minigameLength = 141f;

    [SerializeField]
    private float endTransitionLength = 3f;

    [SerializeField]
    private GameObject headphonesWarning;

    [SerializeField]
    private GameObject logo;

    private AudioSource oceanSounds;
    private AudioSource alarmSounds;
    private float oceanVol, alarmVol;
    private bool IsMinigameActive = false;
    private bool IsMinigameStarted = false;
    private bool IsMinigameFinished = false;
    private bool HasPoemBeenDismissed = false;
    private Animator[] chooseHandednessAnimators;
    private bool hasChosenHandedness = false;
    private List<GameObject> poemChildren;
    private bool isPoemDone = false;
    private Animator logoAnimator;

    private void Start()
    {
        GameObject mg = GameObject.Find(Constants.Name_Minigame);
        oceanSounds = mg.GetComponent<AudioSource>();
        alarmSounds = playerObject.GetComponent<AudioSource>();
        oceanVol = oceanSounds.volume;
        alarmVol = 0f;
        chooseHandednessAnimators = chooseHandednessObject.GetComponentsInChildren<Animator>();
        poemChildren = new List<GameObject>();
        logoAnimator = logo.GetComponent<Animator>();

        // Get children of poem object
        for (int i = 0; i < poemObject.transform.childCount; i++)
        {
            poemChildren.Add(poemObject.transform.GetChild(i).gameObject);
        }
        StartCoroutine(ShowStartButton());
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if (hasChosenHandedness && !isPoemDone)
            {
                isPoemDone = true;

                // Fade out poem text
                foreach (GameObject poemChild in poemChildren)
                {
                    if (!poemChild.activeSelf)
                    {
                        poemChild.SetActive(true);
                    }
                }
            }
            else if (isPoemDone && !HasPoemBeenDismissed)
            {
                HasPoemBeenDismissed = true;

                foreach (GameObject child in poemChildren)
                {
                    child.GetComponent<Animator>().SetTrigger(Constants.AnimParam_FadeOut);
                }

                IsMinigameActive = true;
            }
        }

        if (IsMinigameActive && !IsMinigameStarted)
        {
            IsMinigameStarted = true;
            StartCoroutine(StartGame());
            StartCoroutine(EndGame());
        }

        if (oceanSounds != null)
        {
            if (oceanSounds.volume != oceanVol)
            {
                oceanSounds.volume = Mathf.Lerp(oceanSounds.volume, oceanVol, 1.0f * Time.deltaTime);
            }
        }

        if (alarmSounds != null)
        {
            if (alarmSounds.volume != alarmVol)
            {
                alarmSounds.volume = Mathf.Lerp(alarmSounds.volume, alarmVol, 0.25f * Time.deltaTime);
            }
        }

        if (IsMinigameFinished)
        {
            IsMinigameFinished = false;
            GameManager.Instance.LoadScene(ScenesEnum.TransitionScene);
        }
    }

    public void PressStart()
    {
        StartCoroutine(TitleToPoem());
    }

    private IEnumerator TitleToPoem()
    {
        titleTextAnimator.SetTrigger(Constants.AnimParam_FadeOut);
        startTextAnimator.SetTrigger(Constants.AnimParam_FadeOut);
        startButtonAnimator.SetTrigger(Constants.AnimParam_FadeOut);
        logoAnimator.SetTrigger(Constants.AnimParam_FadeOut);
        yield return new WaitForSeconds(titleToPoemPause);
        chooseHandednessObject.SetActive(true);
        startButtonAnimator.gameObject.SetActive(false);
        logo.SetActive(false);
    }

    private IEnumerator StartGame()
    {
        EnableBackgroundAnimations();
        yield return new WaitForSeconds(pauseBeforeGame);
        oceanVol = oceanVolumeWhenGameStarts;
        minigamePartsObject.SetActive(true);
        playerObject.SetActive(true);
        lighthouseObject.SetActive(true);
	    blurObject.SetActive(false);
    }

    private IEnumerator EndGame()
    {
        var alarm = playerObject.GetComponent<AudioSource>();
        yield return new WaitForSeconds(minigameLength);
        
        alarm.Play();
        alarmVol = alarmEndVolume;
        
        yield return new WaitForSeconds(endTransitionLength);
        IsMinigameFinished = true;
    }

    private void EnableBackgroundAnimations()
    {
        var sky = GameObject.Find(Constants.Name_Sky);

        foreach (var anim in sky.GetComponentsInChildren<Animator>())
        {
            anim.enabled = true;
        }
    }

    public void SelectLeftHandedness()
    {
        GameManager.Instance.LeftHandMode = true;
        TransitionToPoem();
    }

    public void SelectRightHandedness()
    {
        GameManager.Instance.LeftHandMode = false;
        TransitionToPoem();
    }

    private void TransitionToPoem()
    {
        foreach (var animator in chooseHandednessAnimators)
        {
            animator.SetTrigger(Constants.AnimParam_FadeOut);
        }

        StartCoroutine(StartPoem());
    }

    private IEnumerator StartPoem()
    {
        yield return new WaitForSeconds(titleToPoemPause);
        chooseHandednessObject.SetActive(false);
        hasChosenHandedness = true;

        foreach (GameObject poemChild in poemChildren)
        {
            if (!isPoemDone)
            {
                poemChild.SetActive(true);
                yield return new WaitForSeconds(timeBetweenPoemLines);
            }
            else
            {
                break;
            }
        }

        isPoemDone = true;
    }

    private IEnumerator ShowStartButton()
    {
        yield return new WaitForSeconds(pauseBeforeShowStartButton);
        headphonesWarning.SetActive(false);
        startButtonAnimator.gameObject.SetActive(true);
        logo.SetActive(true);
    }
}
