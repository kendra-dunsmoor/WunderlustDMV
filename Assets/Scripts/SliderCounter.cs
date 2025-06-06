using UnityEngine;
using UnityEngine.UI;

public class SliderCounter : MonoBehaviour
{
    public Slider slider;    
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        UpdateBar(FetchUpdatedCount());
    }

    public void UpdateBar(float newVal)
    {
        Debug.Log("Set slider to: " + newVal);
        slider.value = newVal;

        MouseOverDescription description = gameObject.GetComponent<MouseOverDescription>();
        if (description != null) description.UpdateDescription(newVal + "/" + slider.maxValue);
    }

    private float FetchUpdatedCount()
    {
        if (gameManager != null)
        {
            if (gameObject.tag == "PerformanceMeter") return gameManager.FetchPerformance();
            else if (gameObject.tag == "WillMeter") return gameManager.FetchWill();
        }
        Debug.LogError("Failed to get game manager will/performance");
        return 100f;  
    }
}
