using UnityEngine;
using UnityEngine.SceneManagement;

public class BreakRoomManager : MonoBehaviour
{
    void Start()
    {
        // TODO: system to check which dialogue to trigger
        gameObject.GetComponent<DialogueTrigger>().TriggerDialogue();
    }
    public void StartShift()
    {
        SceneManager.LoadSceneAsync(3);
    }

}
