using System.Collections;
using UnityEngine;

public class EnemyHitFlash : MonoBehaviour
{
    public Color hitColor = Color.red;
    public float flashDuration = 0.15f;

    private Renderer[] renderers;
    private Color[] originalColors;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();

        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
            {
                originalColors[i] = renderers[i].material.color;
            }
        }
    }

    public void FlashRed()
    {
        StopAllCoroutines();
        StartCoroutine(FlashCoroutine());
    }

    IEnumerator FlashCoroutine()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
            {
                renderers[i].material.color = hitColor;
            }
        }

        yield return new WaitForSeconds(flashDuration);

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
            {
                renderers[i].material.color = originalColors[i];
            }
        }
    }
}