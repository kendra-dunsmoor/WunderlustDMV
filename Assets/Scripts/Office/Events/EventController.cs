using UnityEngine;

public class EventController : MonoBehaviour
{
    void Start()
    {
        // TODO: system to check which dialogue to trigger
        gameObject.GetComponent<DialogueTrigger>().TriggerDialogue();
    }

    public void NextShift() {

    }
}
