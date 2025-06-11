using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    public Slider slider;
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private float targetValue = 0.5f;
    private float currentValue;
    private float elapsedTime;
    void Start()
    {
        elapsedTime = 0f;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void UpdateBar(float newVal, float maxVal)
    {

        targetValue = newVal / maxVal;
        currentValue = slider.value;
        elapsedTime = 0f;

        MouseOverDescription description = gameObject.GetComponent<MouseOverDescription>();
        if (description != null) description.UpdateDescription(newVal + "/" + maxVal, "Customer Frustration");
    }
}
