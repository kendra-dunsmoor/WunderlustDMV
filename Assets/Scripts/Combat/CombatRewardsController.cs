using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatRewardsController : MonoBehaviour
{
    [SerializeField] private GameObject nextShiftCalendar;
    [SerializeField] private GameObject earlyFinishText;
    [SerializeField] private TextMeshProUGUI rewardsDescription;
    [SerializeField] private Transform actionUpgradeGrid;
    [SerializeField] private GameObject actionUpgradePrefab;
    [SerializeField] private ActionUpgradeDB actionUpgradeDB;
    private AudioManager audioManager;
    private GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
        gameManager = FindFirstObjectByType<GameManager>();



        int currency = GetCurrencyRewards();
        rewardsDescription.text ="Performance Incentive: " + currency + " Office Obols";
        gameManager.UpdateOfficeBucks(currency);

        int metaCurrency = GetMetaRewards();
        rewardsDescription.text +="\nTake Home Pay: " + metaCurrency + " Chthonic Credits";
        gameManager.UpdateSoulCredits(metaCurrency);

        if(GetVRepRewards() == 1)
        {
            rewardsDescription.text +="\nVerrine is Impressed!";
            gameManager.UpdateVRep(1);
        }
        if(GetARepRewards() == 1)
        {
            rewardsDescription.text +="\nAstaroth is Pleased!";
            gameManager.UpdateARep(1);
        }

        SpawnActionUpgrades();
    }

    private void SpawnActionUpgrades()
    {
        // TODO: just add first action to test, make random later
        List<Action> playerActions = gameManager.FetchActions();
        Debug.Log("Fetched player actions: " + playerActions);
        List<ActionUpgrade> randomUpgrades = actionUpgradeDB.GetRandomUpgrades(3);
        foreach (ActionUpgrade upgrade in randomUpgrades)
        {
            int actionIndex = UnityEngine.Random.Range(0, 4);
            GameObject button = Instantiate(actionUpgradePrefab, actionUpgradeGrid);
            ActionUpgradeUIController upgradeUI = button.GetComponent<ActionUpgradeUIController>();
            upgradeUI.actionName.text = playerActions[actionIndex].actionName;
            upgradeUI.upgradeTitle.text = upgrade.upgradeName;
            upgradeUI.upgradeDescription.text = upgrade.GetDescription();
            button.GetComponent<Button>().onClick.AddListener(() => UpgradeAction(upgrade, actionIndex));
        }
    }

    public void markEarlyFinish(bool finishedEarly)
    {
        earlyFinishText.SetActive(finishedEarly);
    }

    public void UpgradeAction(ActionUpgrade upgrade, int actionIndex)
    {
        if (audioManager != null) audioManager.PlaySFX(audioManager.buyUpgrade);
        gameManager.ApplyActionUpgrade(upgrade, actionIndex);
        CheckCalendar();
    }

    public void CheckCalendar()
    {
        if (audioManager != null) audioManager.PlaySFX(audioManager.buttonClick);
        Instantiate(nextShiftCalendar, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
        // Clear current screen
        Destroy(gameObject);
    }

    private int GetCurrencyRewards()
    {
        float performance = gameManager.FetchPerformance();
        if (performance > 100) return (int) Math.Round((200 - performance) / 2);
        else return (int) Math.Round(performance / 2);
    }

    private int GetMetaRewards()
    {
        int metaCoins = 5;
        List<Certificate> playerCerts = gameManager.FetchCertificates();

        // TODO:Check if finished early, if true set to 2

       
		if (playerCerts.Any(c => c.type == Certificate.CertificateType.SIDE_GIG))
		{
			metaCoins += 2;
		}

       return metaCoins;
    }

    private int GetVRepRewards()
    {
        float performance = gameManager.FetchPerformance();
        if (performance > 130) return 1;
        else return 0;
    }
    
    private int GetARepRewards()
    {
        float performance = gameManager.FetchPerformance();
        if (performance < 70) return 1;
        else return 0;
    }
}
