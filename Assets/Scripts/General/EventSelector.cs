using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class EventSelector : MonoBehaviour
{
    // Scene Groupings to fetch random event from
    [SerializeField] Dialogue[] breakRoomDialogues;
    [SerializeField] Dialogue[] officeEventDialogues;
    [SerializeField] Dialogue introAptDialogue;
    [SerializeField] Dialogue firedAptDialogue;
    [SerializeField] Dialogue reincarnatedAptDialogue;
    [SerializeField] Dialogue winnerAptDialogue;

    [SerializeField] Dialogue introBreakRoomVerrineDialogue;
    [SerializeField] Dialogue introBreakRoomSothothDialogue;
    [SerializeField] GameManager gameManager;
    private System.Random randomGen;

    void Awake()
    {
        randomGen = new System.Random();

    }

    public Dialogue FetchRandomEvent()
    {
        // Note: tutorial structured as landlord, Verrine, Astaroth in combat, then Sothoth
        string scene = SceneManager.GetActiveScene().name;
        switch (scene)
        {
            case "Office_BreakRoom":
                if (gameManager.InTutorial())
                {
                    if (gameManager.FetchCurrentCalendarDay() == 0) return introBreakRoomVerrineDialogue;
                    else return introBreakRoomSothothDialogue;
                }
                else
                    return breakRoomDialogues[GetRandom(0, breakRoomDialogues.Length)];
            case "Apartment":
                // Check result of last run
                GameState.RunStatus lastRunResult = gameManager.FetchRunState();
                switch (lastRunResult)
                {
                    case GameState.RunStatus.FIRED:
                        return firedAptDialogue;
                    case GameState.RunStatus.REINCARNATED:
                        return reincarnatedAptDialogue;
                    case GameState.RunStatus.WON:
                        return winnerAptDialogue;
                }
                return introAptDialogue;
            case "Office_Event":
                return officeEventDialogues[GetRandom(0, officeEventDialogues.Length)];
        }
        return introAptDialogue;
    }

    private int GetRandom(int min, int max)
    {
        return randomGen.Next(min, max);
    }
}
