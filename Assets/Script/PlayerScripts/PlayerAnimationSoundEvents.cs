using UnityEngine;

public class PlayerAnimationSoundEvents : MonoBehaviour
{
    private PlayerMovement playerMovement;

    void Awake()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();

        if (playerMovement == null)
            Debug.LogError("PlayerMovement NOT found in parent!");
        else
            Debug.Log("PlayerMovement found by Animation Events.");
    }

    public void PlaySwordCombo1Sound()
    {
        Debug.Log("Animation Event: Combo 1 Sound");
        playerMovement?.PlaySwordCombo1Sound();
    }

    public void PlaySwordCombo2Sound()
    {
        Debug.Log("Animation Event: Combo 2 Sound");
        playerMovement?.PlaySwordCombo2Sound();
    }

    public void PlaySwordCombo3Sound()
    {
        Debug.Log("Animation Event: Combo 3 Sound");
        playerMovement?.PlaySwordCombo3Sound();
    }

    public void PlayBowShootSound()
    {
        Debug.Log("Animation Event: Bow Shoot Sound");
        playerMovement?.PlayBowShootSound();
    }
}