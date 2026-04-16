using UnityEngine;
using TMPro;

public class KillCounterManager : MonoBehaviour
{
    public static KillCounterManager Instance;

    public TMP_Text killText;
    public int totalKills = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();
    }

    public void AddKill()
    {
        totalKills++;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (killText != null)
            killText.text = totalKills.ToString();
    }
}