using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    public Slider slider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void UpdateBar(float currentVal, float maxVal)
    {
        Debug.Log("Set slider to: " + currentVal / maxVal);
        slider.value = currentVal / maxVal;

        MouseOverDescription description = gameObject.GetComponent<MouseOverDescription>();
        if (description != null) description.UpdateDescription(currentVal + "/" + maxVal);
    }
}
