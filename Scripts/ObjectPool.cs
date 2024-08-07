using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
    [SerializeField]
    private List<GameObject> prefabs;

    private List<GameObject> pool;

    // Start is called before the first frame update
    private void Awake()
    {
        pool = new List<GameObject>();
    }

    /// <summary>
    /// Add an object to the pool.
    /// </summary>
    /// <param name="obj">The object to add to the pool.</param>
    public void AddToPool(GameObject obj)
    {
        pool.Add(obj);
        obj.SetActive(false);
    }

    /// <summary>
    /// Get an object from the pool.
    /// </summary>
    /// <param name="tag">The tag of the object to get from the pool.</param>
    /// <returns>A game object with the specified tag.</returns>
    public GameObject GetFromPool(string tag)
    {
        GameObject obj;

        if (pool.Exists(go => go.CompareTag(tag)))
        {
            obj = pool.Find(go => go.CompareTag(tag));
            obj.SetActive(true);
            pool.Remove(obj);
        }
        else
        {
            GameObject prefab = prefabs.Find(go => go.CompareTag(tag));
            obj = Instantiate(prefab);
        }

        return obj;
    }
}
