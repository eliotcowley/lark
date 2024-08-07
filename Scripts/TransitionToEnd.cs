using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionToEnd : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GoToTheEnd());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator GoToTheEnd()
    {
        yield return new WaitForSeconds(3);
        GameManager.Instance.LoadScene(ScenesEnum.EndScene);
    }
}
