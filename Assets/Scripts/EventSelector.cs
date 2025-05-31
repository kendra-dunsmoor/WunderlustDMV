using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class EventSelector : MonoBehaviour
{
    // Scene Groupings to fetch random event from
    [SerializeField] Dialogue[] breakRoomDialogues;
    [SerializeField] Dialogue[] apartmentDialogues;
    [SerializeField] Dialogue[] officeEventDialogues;
    [SerializeField] Dialogue introAptDialogue;
    [SerializeField] Dialogue introBreakRoomDialogue;
    [SerializeField] GameManager gameManager;

    public Dialogue FetchRandomEvent()
    {
        // TODO: add more complicated logic, need to determine if first time, etc
        string scene = SceneManager.GetActiveScene().name;
        switch(scene) {
            // TODO: gotta double check names
            case "Office_BreakRoom":
                if (gameManager.inTutorial) {
                    gameManager.inTutorial = false;
                    return introBreakRoomDialogue;
                } else
                    return breakRoomDialogues[Random.Range(0, breakRoomDialogues.Length)];
            case "Apartment":
                if (gameManager.inTutorial) {
                    return introAptDialogue;
                } else
                    return apartmentDialogues[Random.Range(0, apartmentDialogues.Length)];
            case "Office_Event":
                return officeEventDialogues[Random.Range(0, officeEventDialogues.Length)];
        }
        return introAptDialogue;
    }
}
