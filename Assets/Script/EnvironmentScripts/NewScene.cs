using UnityEngine;
using UnityEngine.SceneManagement;

public class NewScene : MonoBehaviour
{
   public void LoadNewScene(string Game)
    {
        SceneManager.LoadScene(Game);
    }
    
}
