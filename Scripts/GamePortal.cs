using UnityEngine;
using UnityEngine.UI;

public class GamePortal : MonoBehaviour
{
    bool playerInside = false;
    bool activateMinigame = false;
    ParticleSystem ps;
    public MiniGameResource gameResources;
    public ScenesEnum gameScene = ScenesEnum.FlowScene;
    public GameObject hoverButton;
    private Transform playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();

        if (ps == null)
        {
            ps = GetComponentInChildren<ParticleSystem>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInside && ps.isStopped)
        {
            ps.Play();
            if (hoverButton != null)
            {
                hoverButton.GetComponent<FadeToggleButton>().ToggleFade(true);
                hoverButton.GetComponent<Button>().onClick.AddListener(LoadMinigame);
            } else
            {
                Debug.LogError("Can't find hover button.");
            }
        }
        else if (!playerInside && ps.isPlaying)
        {
            ps.Stop();
            if (hoverButton != null)
            {
                hoverButton.GetComponent<FadeToggleButton>().ToggleFade(false);
                hoverButton.GetComponent<Button>().onClick.RemoveListener(LoadMinigame);
            } else
            {
                Debug.LogError("Can't find hover button.");
            }
        }

        /* Adjust this part to handle whatever input we want to handle. Remove later */
        if (Input.GetKeyDown(KeyCode.M))
        {
            activateMinigame = true;
        } else if (playerInside && activateMinigame)
        {
            activateMinigame = false;
            LoadMinigame();
        }
    }

    public void LoadMinigame()
    {
        // Tell the resource manager what the minigame's rewards and time cost are
        Debug.Log("Has resources Mind: " + gameResources.MindValue);
        ResourceManager.Instance.SetMinigameResource(gameResources);

        // Save player location so that Game Manager can start the player there.
        playerTransform = GameObject.FindGameObjectWithTag(Constants.Tag_Player).GetComponent<Transform>();
        GameManager.Instance.SetPlayerLocationTransform(playerTransform.position);

        // Load the game scene
        GameManager.Instance.LoadScene(gameScene);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.Tag_Player))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Constants.Tag_Player))
        {
            playerInside = false;
        }
    }
}
