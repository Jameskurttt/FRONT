using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NewScene : MonoBehaviour
{
    public void LoadNewScene(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        // Example: just wait 1 second before loading
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(sceneName);
    }
}