using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OnSceneChangeImage : MonoBehaviour
{
    [SerializeField] Image imageToChange;
    [SerializeField] Sprite imageUpdate;
    [SerializeField] string sceneName;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == sceneName) {
            imageToChange.sprite = imageUpdate;
        }
    }
}
