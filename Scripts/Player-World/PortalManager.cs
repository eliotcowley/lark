using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public GameObject[] portalOrder;

    // Start is called before the first frame update
    void Start()
    {
        if(portalOrder.Length >= 1 && GameManager.Instance.ActivePortal < portalOrder.Length)
        {
            // If we happen to run through all of the portals, cycle back to the first one as the next portal.
            GameManager.Instance.ActivePortal = (GameManager.Instance.ActivePortal + 1) % portalOrder.Length;

            portalOrder[GameManager.Instance.ActivePortal].SetActive(true);
            Debug.Log($"PortalManager: {portalOrder[GameManager.Instance.ActivePortal].name} is now active.");
        }
    }
}
