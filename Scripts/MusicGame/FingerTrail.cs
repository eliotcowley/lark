using System.Collections;
using UnityEngine;

/// <summary>
/// Manages player interaction with the minigame.
/// </summary>
public class FingerTrail : MonoBehaviour
{
    /// <summary>
    /// The dot manager for this level.
    /// </summary>
    [SerializeField]
    private DotManager dotManager;

    /// <summary>
    /// The maximum distance that the player's finger can be from a dot to hit it.
    /// </summary>
    [SerializeField]
    private float maxDistanceToDot = 1f;

    private ParticleSystem trailParticles;
    private bool isPlaying = false; // Whether the trail particles are playing.
    private float linePosX;

    private void Start()
    {
        trailParticles = GetComponent<ParticleSystem>();
        linePosX = GameManager.Instance.LinePosition.x;
    }

    private void Update()
    {
        // Using touch
        if (Input.touchCount > 0)
        {
            Vector3 worldPos = InteractWithDots(Input.touches[0].position);
            transform.position = worldPos;

            if (!isPlaying)
            {
                isPlaying = true;
            }
        }
        else
        {
            if (isPlaying)
            {
                isPlaying = false;
            }

            // DEBUG ONLY: Play with mouse
            InteractWithDots(Input.mousePosition);
        }
    }

    /// <summary>
    /// Handles whether the player hits any dots.
    /// </summary>
    /// <param name="inputVector">The input vector, either the finger or mouse position.</param>
    /// <returns>The world position of the input vector.</returns>
    private Vector3 InteractWithDots(Vector2 inputVector)
    {
        Vector3 screenPos = new Vector3(inputVector.x, inputVector.y, 10);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        if (worldPos.x > (linePosX - Constants.LineXZone) 
            && worldPos.x < (linePosX + Constants.LineXZone)
            && !GameManager.Instance.Paused)
        {
            for (int i = 0; i < dotManager.CurrentDots.Count; i++)
            {
                if (Vector3.Distance(worldPos, dotManager.CurrentDots[i].transform.position) < maxDistanceToDot)
                {
                    dotManager.DotsHit++;
                    dotManager.Combo++;
                    dotManager.SetCombo(dotManager.Combo);

                    if (dotManager.CurrentDots[i].DotIndex >= dotManager.DotsToUse.Count)
                    {
                        dotManager.ShowScoreText();
                    }

                    CreateDotBurst(i);
                    dotManager.CurrentDots[i].RemoveDot();
                    dotManager.SetFailing(false);
                }
            }
        }

        return worldPos;
    }

    private void CreateDotBurst(int dotIndex)
    {
        GameObject dotBurst = ObjectPool.Instance.GetFromPool(Constants.Tag_DotBurst);
        dotBurst.transform.position = dotManager.CurrentDots[dotIndex].transform.position;
        ParticleSystem ps = dotBurst.GetComponent<ParticleSystem>();
        ps.Play();
        StartCoroutine(RemoveBurst(ps.main.startLifetime.constant, dotBurst));
    }

    private IEnumerator RemoveBurst(float seconds, GameObject burst)
    {
        yield return new WaitForSeconds(seconds);
        ObjectPool.Instance.AddToPool(burst);
    }
}
