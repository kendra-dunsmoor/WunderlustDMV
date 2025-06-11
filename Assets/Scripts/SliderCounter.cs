using UnityEngine;
using UnityEngine.UI;

public class SliderCounter : MonoBehaviour
{
    public Slider slider;
    private GameManager gameManager;
    [SerializeField] private float transitionDuration = 0.6f;
    [SerializeField] private float targetValue = 100f;
    private float currentValue;
    private float elapsedTime;

    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void Start()
    {
        Debug.Log("Start and set counter to " + FetchUpdatedCount());
        elapsedTime = 0f;
        UpdateBar(FetchUpdatedCount());
        Debug.Log("Slider updated" + slider.value);
    }

    void Update()
    {
        if (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;
            slider.value = Mathf.Lerp(currentValue, targetValue, t);
        }
    }
    
    public void UpdateBar(float newVal)
    {
        Debug.Log("Set slider to: " + newVal + "/" + slider.maxValue);
        targetValue = newVal;
        currentValue = slider.value;
        elapsedTime = 0f;

        string title = gameObject.tag == "PerformanceMeter" ? "Performance" : "Will";
        MouseOverDescription description = gameObject.GetComponent<MouseOverDescription>();
        if (description != null) description.UpdateDescription(newVal + "/" + slider.maxValue, title);
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
