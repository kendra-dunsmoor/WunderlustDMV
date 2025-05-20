using TMPro;
using UnityEngine;

public class CombatRewardsController : MonoBehaviour
{
    [SerializeField] private GameObject nextShiftCalendar;
    [SerializeField] private TextMeshProUGUI rewardsDescription;
    [SerializeField] private int currency = 50;

    private GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        // Add rewards info
        // idk what these rewards will usually look like, hard code some office bucks for now
        // TODO: add choice of 3 tricks for actions
        rewardsDescription.text = currency + " OfficeBucks";
    }

    public void CheckCalendar() {
        gameManager.UpdateOfficeBucks(currency);
        Instantiate(nextShiftCalendar, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
        // Clear current screen
        Destroy(gameObject);
    }
}
