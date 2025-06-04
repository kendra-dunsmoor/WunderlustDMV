using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatRewardsController : MonoBehaviour
{
    [SerializeField] private GameObject nextShiftCalendar;
    [SerializeField] private TextMeshProUGUI rewardsDescription;
    [SerializeField] private int currency = 50;
    [SerializeField] private Transform actionUpgradeGrid;
    [SerializeField] private GameObject actionUpgradePrefab;
    [SerializeField] private ActionUpgradeDB actionUpgradeDB;
    private GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        rewardsDescription.text = currency + " OfficeBucks";

        // TODO: just add first action to test, make random later
        List<Action> playerActions = gameManager.FetchActions();
        Debug.Log("Fetched player actions: " + playerActions);
        ActionUpgrade[] randomUpgrades = actionUpgradeDB.GetRandomUpgrades();
        foreach (ActionUpgrade upgrade in randomUpgrades) {
            int actionIndex = Random.Range(0,4);
            GameObject button = Instantiate(actionUpgradePrefab, actionUpgradeGrid);
            ActionUpgradeUIController upgradeUI = button.GetComponent<ActionUpgradeUIController>();
            upgradeUI.actionName.text = playerActions[actionIndex].actionName;
            upgradeUI.upgradeTitle.text = upgrade.upgradeName;
            upgradeUI.upgradeDescription.text = upgrade.GetDescription();
            button.GetComponent<Button>().onClick.AddListener(() => UpgradeAction(upgrade, actionIndex));
        }

    }

    public void UpgradeAction(ActionUpgrade upgrade, int actionIndex) {
        gameManager.ApplyActionUpgrade(upgrade, actionIndex);
        CheckCalendar();
    }

    public void CheckCalendar() {
        gameManager.UpdateOfficeBucks(currency);
        Instantiate(nextShiftCalendar, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
        // Clear current screen
        Destroy(gameObject);
    }
}
