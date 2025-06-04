using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpRewardController : MonoBehaviour
{
    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI flavorTextDescription;

    private AudioManager audioManager;

    void Start()
    {
        audioManager = 	FindFirstObjectByType<AudioManager>();
    }
    public void Close()
    {
        if (audioManager != null) audioManager.PlaySFX(audioManager.buttonClick);
        Destroy(gameObject);
    }

    public void AddRewardInfo(Sprite image, string rewardText, string flavorText) {
        // Might need to add some more complicated logic here if there are multiple rewards?
        if (image != null) itemImage.sprite = image;
        if (flavorText != null || flavorText != "") flavorTextDescription.text = flavorText;
        description.text = "Got: " + rewardText;
    }
}
