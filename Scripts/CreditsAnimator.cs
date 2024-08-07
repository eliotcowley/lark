using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class CreditsAnimator : MonoBehaviour
{
    public GameObject player;
    public GameObject floor;
    public GameObject cat;
    public GameObject Bathroom;
    public GameObject ExitDoor;
    public Camera cam;
    public Texture[] tex;

    private PlayerController pc;
    private Animator fadeAnim;
    private List<GameObject> gameObjects;
    private NavMeshAgent agent;

    private int fade = -1;
    private float currentFade = 1f;
    private float currentCamX = 99999f;
    private float finalCamPos = 15f;

    // Start is called before the first frame update
    void Start()
    {
        // Init game objects
        gameObjects = new List<GameObject>();
        pc = player.GetComponent<PlayerController>();
        agent = player.GetComponent<NavMeshAgent>();
        fadeAnim = this.GetComponentInChildren<Animator>();

        // Start Credits roll
        StartCoroutine(PoemRoll());
        StartCoroutine(PlayerMovement());
        StartCoroutine(WaitToReturnToTitle());
    }

    // Update is called once per frame
    void Update()
    {
        // If there is a valid fade value, start to fade active credit text
        if(fade != -1)
        {
            currentFade = Mathf.Lerp(currentFade, fade, 1 * Time.deltaTime);

            foreach(GameObject go in gameObjects)
            {
                var goText = go.GetComponent<TextMeshProUGUI>();
                goText.color = new Color(goText.color.r, goText.color.g, goText.color.b, currentFade);
            }
        }

        if (currentCamX != 99999f && currentCamX != finalCamPos)
        {
            currentCamX = Mathf.Lerp(currentCamX, finalCamPos, 1 * Time.deltaTime);

            cam.transform.position = new Vector3(currentCamX, cam.transform.position.y, cam.transform.position.z);
        }
    }

    IEnumerator PoemRoll()
    {
        var poemLines = this.transform.GetChild(0).GetChild(0);

        for (int i = 0; i < poemLines.childCount; i++)
        {
            // If at "Day to Day...", fade to black
            if (i == poemLines.childCount - 2)
            {
                fadeAnim.Play("Fade");
                yield return new WaitForSeconds(1.5f);
                fadeAnim.enabled = false;
            }

            var poemLine = poemLines.GetChild(i);
            yield return new WaitForSeconds(1);
            poemLine.gameObject.SetActive(true);
            fade = 1;
            currentFade = 0;
            gameObjects.Add(poemLine.gameObject);

            yield return new WaitForSeconds(2);

            // Fade Out
            fade = 0;
            currentFade = 1;
            yield return new WaitForSeconds(1);
            poemLine.gameObject.SetActive(false);

            fade = -1;
            gameObjects.Clear();
        }

        // Fade back in and start rolling credits
        fadeAnim.enabled = true;
        StartCoroutine(CreditsRoll());
    }

    IEnumerator PlayerMovement()
    {
        yield return new WaitForSeconds(1);
        var mat = floor.GetComponent<MeshRenderer>().material;

        // Pace around floor and move to window
        pc.MovePlayer(new Vector3(-35.0f, 1.2f, 10.2f));
        yield return new WaitForSeconds(3);
        pc.MovePlayer(new Vector3(45.0f, 1.2f, 10.2f));
        yield return new WaitForSeconds(3);
        pc.MovePlayer(new Vector3(18.7f, 1.2f, -14.8f));
        yield return new WaitForSeconds(3);

        // Go to bathroom
        pc.MovePlayer(new Vector3(19.6f, 1.2f, 33.6f));
        yield return new WaitForSeconds(2);
        Bathroom.SetActive(true); // Change to open door

        // Enter bathroom and change to clean room
        pc.MovePlayer(new Vector3(19.6f, 1.2f, 100.0f));
        yield return new WaitForSeconds(3);
        pc.MovePlayer(new Vector3(19.6f, 1.2f, 33.6f));
        yield return new WaitForSeconds(2);
        Bathroom.SetActive(false);

        // Move to bed and clean it
        pc.MovePlayer(new Vector3(45.4f, 1.2f, 5.4f));
        yield return new WaitForSeconds(2);
        mat.SetTexture("_MainTex", tex[0]); // Change to clean bed

        // Go to laundry and clean up
        pc.MovePlayer(new Vector3(-35.7f, 1.2f, 39.6f));
        yield return new WaitForSeconds(3);
        mat.SetTexture("_MainTex", tex[1]);

        // Go to the cat
        pc.MovePlayer(new Vector3(-128.5f, 1.2f, 5.8f));
        yield return new WaitForSeconds(4);

        // Move to door, open and leave
        pc.MovePlayer(new Vector3(-100.5f, 1.2f, 27f));
        yield return new WaitForSeconds(2);
        ExitDoor.SetActive(true); // Make door appear (anim)
        yield return new WaitForSeconds(1);
        pc.MovePlayer(new Vector3(-136.9f, 1.2f, 27f));
        yield return new WaitForSeconds(2);

        pc.transform.gameObject.SetActive(false);
        ExitDoor.SetActive(false);

        currentCamX = cam.transform.position.x;
    }

    // Run through each section of credits
    IEnumerator CreditsRoll()
    {
        yield return new WaitForSeconds(2);

        var creditObject = this.transform.GetChild(1).GetChild(0);
        for (int i = 0; i < creditObject.childCount; i++)
        {
            var names = creditObject.GetChild(i);
            yield return new WaitForSeconds(2);
            names.gameObject.SetActive(true);

            // Fade In
            fade = 1;
            currentFade = 0;
            SectionFade(names);

            yield return new WaitForSeconds(4);

            // Fade Out
            fade = 0;
            currentFade = 1;
            yield return new WaitForSeconds(2);

            names.gameObject.SetActive(false);

            fade = -1;
            gameObjects.Clear();
        }

        yield return new WaitForSeconds(4);
        fadeAnim.Play("FadeToBlack");
    }

    IEnumerator WaitToReturnToTitle()
    {
        AudioClip clip = floor.GetComponentInParent<AudioSource>().clip;

        yield return new WaitForSeconds(clip.length + 5.0f);
        GameManager.Instance.LoadScene(ScenesEnum.TitleScene);
    }

    // Add an active section's text objects to be faded in/out
    void SectionFade(Transform names)
    {
        for(int i = 0; i < names.childCount; i++)
        {
            var name = names.GetChild(i).gameObject;

            gameObjects.Add(name);
        }
    }
}
