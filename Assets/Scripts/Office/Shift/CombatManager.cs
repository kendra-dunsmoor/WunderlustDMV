using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ActionEffect;

/*
* Combat Manager
* ~~~~~~~~~~~~~~~
* Initialize and manager queue of customers, meters, and goals/progress
*/
public class CombatManager : MonoBehaviour
{
    private AudioManager audioManager;
    private GameManager gameManager;

    // Prefabs
    [SerializeField] private GameObject nextShiftCalendar;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject customer; // temp will need to have data for diff enemy types
    [SerializeField] private GameObject paperwork; // temp object placeholder
    [SerializeField] private GameObject currentEffectPrefab;

    // UI objects/meters
    [SerializeField] private Slider performanceMeter;
    [SerializeField] private Slider willMeter;
    [SerializeField] private TextMeshProUGUI customerGoalText;
    [SerializeField] private TextMeshProUGUI remainingTurnsText;
    [SerializeField] private GameObject[] customerIconQueue;
    [SerializeField] private Transform currentEffectsPanel;

    // Spawn Points
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform offScreenPoint;
    [SerializeField] private Transform frontOfLinePoint;

    // Combat trackers
    private Queue<Customer> customersInLine = new Queue<Customer>();
    private Customer currCustomer;
    private int CUSTOMER_GOAL = 3;
    private float performanceLevel = 25; // temp range 0 to 50
    private float willLevel = 50; // temp value
    private int remainingTurns = 10; // temp value
    private Dictionary<EffectType, GameObject> activeEffects = new Dictionary<EffectType, GameObject>();

    void Start()
    {
        performanceMeter.value = 25f;
        willMeter.value = 50f;
        gameManager = FindFirstObjectByType<GameManager>();
        InitializeCustomerQueue();
    }

    void Update()
    {
        
    }

    private void EndShift()
    {
        // Pop up end screen
        gameManager.ShiftCompleted();
        Instantiate(nextShiftCalendar, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
    }

    private void GameOver()
    {
        // Pop up end screen
        Instantiate(gameOverMenu, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
    }

    private void NextCustomer()
    {
        Debug.Log("Next Customer");
        Debug.Log("Customers remaining: " + customersInLine.Count);
        customerGoalText.text = "Customers remaining: " + customersInLine.Count;
        if (customersInLine.Count == 0) EndShift();
        else {
            currCustomer = customersInLine.Dequeue();
            currCustomer.SendToFront(frontOfLinePoint);
            customerIconQueue[customersInLine.Count].SetActive(false); // this won't work if more customers than queue spots
        }
    }
    
    public void SpawnPaperwork()
    {
        // TODO: Paperwork will be unique/randomized and instantiated in, just for looks rn
        paperwork.SetActive(true);
    }

    // TODO: add logic to randomize/select queue of customers for the shift
    private void InitializeCustomerQueue() {
        // Temp: Spawn all customers off screen and add to in game and icon queues
        Debug.Log("Initialize customer queue: " + CUSTOMER_GOAL);
        for (int i = 0; i < CUSTOMER_GOAL; i++) {
            customersInLine.Enqueue(Instantiate(customer, spawnPoint).GetComponent<Customer>());
            customerIconQueue[i].SetActive(true);
        }
        currCustomer = customersInLine.Dequeue();
        customerIconQueue[CUSTOMER_GOAL - 1].SetActive(false);
        currCustomer.SendToFront(frontOfLinePoint);
    }

    private void UpdatePerformance(float diff) {
        Debug.Log("Change performance by " + diff);
        performanceLevel += diff;
        // TODO: check if in range for meter
        performanceMeter.value = performanceLevel;
        Debug.Log("Performance" + performanceLevel);
        if (performanceLevel <= 0) GameOver(); // Fired
        if (performanceLevel >= 50) GameOver(); // Reincarnated
    }
    private void UpdateWill(float diff) {
        Debug.Log("Change will by -" + diff);
        willLevel -= diff;
        // TODO: check if in range for meter
        willMeter.value = willLevel;
    }

    public void TakeAction (Action action) {
        // Check if sufficient will available for action:
        if (willLevel - action.WILL_MODIFIER < 0) {
            Debug.Log("Insufficient will left for action: " + action.actionName);
            return;
        }

        // Update meters:
        Debug.Log("Taking action: " + action.actionName);
        UpdateMetersWithEffects(action);

        // Decrease remaining turn count and increment active effects
        remainingTurns--;
        Debug.Log("Turns remaining: " + remainingTurns);
        remainingTurnsText.text = "Turns remaining: " + remainingTurns;
        AddNewEffects(action.effect, action.turnsOfEffect);

        // Move current customer if needed:
        MoveCustomer(action.movement);

        // Check end shift state for turns:
        if (remainingTurns == 0) EndShift();
    }

    // Add any new effects from current action and increment tracked effects
    private void AddNewEffects(ActionEffect effect, int turns) {
        // check if current action has no effect
        if (effect == null) return;
        // Check for effects that aren't displayed:
        if (effect.type == EffectType.ADD_TURNS) {
            remainingTurns += turns;
        }
        // add to active/displayed buffs/debuffs
        if (activeEffects.ContainsKey(effect.type)) {
            Debug.Log("Effect already active, add to stack");
            UIEffectController currUIEffect = activeEffects[effect.type].GetComponent<UIEffectController>();
            currUIEffect.UpdateTurns(turns);
            activeEffects[effect.type].GetComponent<MouseOverDescription>().UpdateDescription(effect.effectDescription + "Turns: " + currUIEffect.FetchTurns());
        } else {
            // TODO: clean up this system this is ugly
            Debug.Log("Add new effect");
            GameObject effectMarker = Instantiate(currentEffectPrefab, currentEffectsPanel);
            activeEffects.Add(effect.type, effectMarker);
            effectMarker.GetComponent<UIEffectController>().AddEffect(effect, turns);
            effectMarker.GetComponent<MouseOverDescription>().UpdateDescription(effect.effectDescription + "Turns: " + turns);
        }
    }

    private void UpdateMetersWithEffects(Action action) {
        // TODO: these will need editing for ones that stack, don't decay, etc
        // TODO: add all other effect types
        // TODO: add customer specific effects checks
        float performaceModifier = action.PERFORMANCE_MODIFIER;
        float frustrationModifier = action.FRUSTRATION_MODIFIER;
        float willModifier = action.WILL_MODIFIER;

        List<EffectType> effectsToRemove = new List<EffectType>();
        foreach(var (type, effect) in activeEffects) {
            switch (type) {
                case EffectType.ATTENTION:
                    Debug.Log("Multiplying performance change by 20% due to attention");
                    performaceModifier *= 1.2f;
                    break;
                case EffectType.HUSTLING:
                    // performance gains remove attention
                    if (performaceModifier > 0) {
                        if (activeEffects.ContainsKey(EffectType.ATTENTION)) {
                            Debug.Log("Removing attention due to hustling");
                            activeEffects[EffectType.ATTENTION].GetComponent<UIEffectController>().UpdateTurns(-1);
                        }
                    }
                    break;
            }
            // Reduce stacked turns, TODO: check if effect decays with turns
            effect.GetComponent<UIEffectController>().UpdateTurns(-1);
            if (effect.GetComponent<UIEffectController>().FetchTurns() == 0) {
                effectsToRemove.Add(type);
            }
        }
        foreach (var e in effectsToRemove) {
            Destroy(activeEffects[e]);
            activeEffects.Remove(e);
        }
        // Update meters with after effects values
        UpdatePerformance(performaceModifier);
        UpdateWill(willModifier);
        currCustomer.updateFrustration(frustrationModifier);
    }


    // Update front customer location in line if needed
    private void MoveCustomer(Action.ActionMovement movement) {
        switch (movement) {
            case Action.ActionMovement.FRONT:
                Debug.Log("Customer remains in front");
                 break;
            case Action.ActionMovement.AWAY:
                Debug.Log("Customer is removed from queue");
                currCustomer.SendAway(true, offScreenPoint); // TODO: remove green accepted true thing
                NextCustomer();
                break;
            case Action.ActionMovement.BACK:     
                Debug.Log("Customer is moved to back of line"); 
                currCustomer.SendToBack(spawnPoint);
                // TODO: Need like a delay here if it is the only customer left in line if we want them to move back first?
                customersInLine.Enqueue(currCustomer);
                NextCustomer();
                break;
        } 
    }
}
