using UnityEngine;
using UnityEngine.UI;

public class DoorPortal : MonoBehaviour
{
    bool playerInside = false;
    bool activateEndDay = false;
    ParticleSystem ps;
    public GameObject hoverButton;

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
                hoverButton.GetComponent<Button>().onClick.AddListener(LoadEndDay);
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
                hoverButton.GetComponent<Button>().onClick.RemoveListener(LoadEndDay);
            } else
            {
                Debug.LogError("Can't find hover button.");
            }
        }

        /* Adjust this part to handle whatever input we want to handle. Remove later */
        if (Input.GetKeyDown(KeyCode.M))
        {
            activateEndDay = true;
        } else if (playerInside && activateEndDay)
        {
            activateEndDay = false;
            LoadEndDay();
        }
    }

    public void LoadEndDay()
    {
        // TODO
        Debug.Log("Ending day");
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
