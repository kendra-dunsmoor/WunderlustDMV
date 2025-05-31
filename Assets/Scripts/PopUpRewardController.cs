using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpRewardController : MonoBehaviour
{
    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI description;
    public void Close()
    {
        Destroy(gameObject);
    }

    public void AddRewardInfo(Sprite image, string text) {
        // Might need to add some more complicated logic here if there are multiple rewards?
        itemImage.sprite = image;
        description.text = "Gained: " + text;
    }
}
