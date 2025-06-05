using TMPro;
using UnityEngine;

public class CurrencyCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI count;

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        FetchUpdatedCount();
    }

    public void RefreshCounter()
    {
        FetchUpdatedCount();
    }

    private void FetchUpdatedCount()
    {
        if (gameManager != null)
        {
            if (gameObject.tag == "Counter_SoulCredits") count.text = gameManager.FetchSoulCredits().ToString();
            else if (gameObject.tag == "Counter_OfficeBucks") count.text = gameManager.FetchOfficeBucks().ToString();
        }
        else count.text = "00";  
    }
}
