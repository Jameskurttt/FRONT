using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPool : MonoBehaviour
{
    public static ArrowPool Instance;

    [Header("Pool Settings")]
    public GameObject arrowPrefab;
    public int poolSize = 20;
    public Transform poolParent;

    [Header("Bow Shoot Delay")]
    [Tooltip("Delay before the arrow is released")]
    public float shootDelay = 0.3f;

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

        CreatePool();
    }

    private void CreatePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab, poolParent);
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

        GameObject newArrow = Instantiate(arrowPrefab, poolParent);
        newArrow.SetActive(false);
        pool.Add(newArrow);

        return newArrow;
    }

 
    public IEnumerator SpawnArrowWithDelay(
        Transform shootPoint,
        Quaternion rotation,
        float arrowSpeed)
    {
        yield return new WaitForSeconds(shootDelay);

        GameObject arrow = GetArrow();

        arrow.transform.position = shootPoint.position;
        arrow.transform.rotation = rotation;

        arrow.SetActive(true);

        Rigidbody rb = arrow.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.linearVelocity = shootPoint.forward * arrowSpeed;
        }
    }
}