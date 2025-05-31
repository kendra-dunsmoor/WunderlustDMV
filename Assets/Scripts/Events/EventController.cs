using UnityEngine;
using UnityEngine.SceneManagement;

public class EventController : MonoBehaviour
{
    void Start()
    {
    }

    public void NextShift() {
        SceneManager.LoadSceneAsync(3);
    }
}
