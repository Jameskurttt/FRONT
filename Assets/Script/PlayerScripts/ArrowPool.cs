using System.Collections.Generic;
using UnityEngine;

public class ArrowPool : MonoBehaviour
{
    public static ArrowPool Instance;

    [Header("Pool Settings")]
    public GameObject arrowPrefab;
    public int poolSize = 20;
    public Transform poolParent;

    private List<GameObject> pool = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (poolParent == null)
            poolParent = transform;

        CreatePool();
    }

    private void CreatePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            
            GameObject arrow = Instantiate(arrowPrefab);

            if (poolParent != null)
                arrow.transform.SetParent(poolParent, false);

            arrow.SetActive(false);
            pool.Add(arrow);
        }
    }

    public GameObject GetArrow()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeInHierarchy)
                return pool[i];
        }

        
        GameObject newArrow = Instantiate(arrowPrefab);

        if (poolParent != null)
            newArrow.transform.SetParent(poolParent, false);

        newArrow.SetActive(false);
        pool.Add(newArrow);

        return newArrow;
    }
}