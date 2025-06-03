using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpRewardController : MonoBehaviour
{
    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI flavorText;
    public void Close()
    {
        Destroy(gameObject);
    }

    public void AddRewardInfo(Sprite image, string rewardName, string flavorText) {
        // Might need to add some more complicated logic here if there are multiple rewards?
        if (image != null) itemImage.sprite = image;
        if (flavorText != null) this.flavorText.text = flavorText;
        description.text = "Gained: " + rewardName;
    }
}
