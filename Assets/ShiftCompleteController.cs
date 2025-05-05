using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShiftCompleteController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI optionA;
    [SerializeField] TextMeshProUGUI optionB;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Called at Instantiate");
        // TODO: fetch next options for run paths;
    }

    public void BreakRoom() {
        SceneManager.LoadSceneAsync(2);
    }

    public void TriggerEvent() {
        SceneManager.LoadSceneAsync(4);
    }
}
