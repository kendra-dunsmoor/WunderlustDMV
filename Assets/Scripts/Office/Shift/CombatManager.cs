using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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

    [Header("------------- Prefabs -------------")]
    // Prefabs
    [SerializeField] private GameObject combatRewardsScreen;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject customer; // temp will need to have data for diff enemy types
    [SerializeField] private GameObject paperwork; // temp object placeholder
    [SerializeField] private GameObject currentEffectPrefab;
    [SerializeField] private GameObject customerIconPrefab;
    [SerializeField] private ActionEffect attentionEffect;

    [Header("------------- UI Meters -------------")]
    [SerializeField] private Slider performanceMeter;
    [SerializeField] private Slider willMeter;
    [SerializeField] private TextMeshProUGUI customerGoalText;
    [SerializeField] private TextMeshProUGUI remainingTurnsText;
    [SerializeField] private Transform currentEffectsPanel;
    [SerializeField] private Transform customerQueuePanel;

    [Header("------------- Spawn Points -------------")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform offScreenPoint;
    [SerializeField] private Transform frontOfLinePoint;

    [Header("------------- Spawn Points -------------")]
    private Queue<Customer> customersInLine = new Queue<Customer>();
    private Queue<GameObject> customerIconQueue = new Queue<GameObject>();
    private Customer currCustomer;
    [Header("------------- Combat Values -------------")]
    [SerializeField, Tooltip("Customer goal set at beginning of combat scene")] private int CUSTOMER_GOAL;
    [SerializeField, Tooltip("Current performance level, UI might not update if this is changed until action taken")] private float performanceLevel; // temp range 0 to 50
    [SerializeField, Tooltip("Current will level, UI might not update if this is changed until action taken")] private float willLevel; // temp value
    [SerializeField, Tooltip("Remaining turns in combat, UI might not update if this is changed until action taken")] private int remainingTurns; // temp value
    private Dictionary<EffectType, GameObject> activeEffects = new Dictionary<EffectType, GameObject>();

    void Start()
    {
        // temp initialization for quick testing when game manager is null:
        if (performanceLevel == 0) performanceLevel = 50f;
        if (willLevel == 0) willLevel = 50f;
        if (CUSTOMER_GOAL == 0) CUSTOMER_GOAL = 10;
        gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null) {
            if (remainingTurns == 0) remainingTurns = CUSTOMER_GOAL + gameManager.FetchCurrentCalendarDay() - 1; // temp
            performanceLevel = gameManager.FetchPerformance();
            willLevel = gameManager.FetchWill();
        }
        performanceMeter.value = performanceLevel;
        willMeter.value = willLevel;
        remainingTurnsText.text = "Turns remaining: " + remainingTurns;
        InitializeCustomerQueue();
    }

    private void EndShift()
    {
        // Pop up end screen
        gameManager.ShiftCompleted(performanceLevel, willLevel);
        // Get combat rewards
        Instantiate(combatRewardsScreen, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
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
            Debug.Log("Customer dequeued");
            currCustomer.SendToFront(frontOfLinePoint);
            Destroy(customerIconQueue.Dequeue());
            Debug.Log("Destroy queue icon");
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
            customerIconQueue.Enqueue(Instantiate(customerIconPrefab, customerQueuePanel)); // temp, this only works while there are less customers than the size of the panel
        }
        currCustomer = customersInLine.Dequeue();
        Destroy(customerIconQueue.Dequeue());
        currCustomer.SendToFront(frontOfLinePoint);
        customerGoalText.text = "Customers remaining: " + customersInLine.Count;
    }

    private void UpdatePerformance(float diff) {
        Debug.Log("Change performance by " + diff);
        performanceLevel += diff;
        // TODO: check if in range for meter
        performanceMeter.value = performanceLevel;
        Debug.Log("Performance" + performanceLevel);
        if (performanceLevel <= 0) {
            GameOver(); // Fired
            Debug.Log("Game Over: Fired for bad performance!");
        }
        if (performanceLevel >= 100) {
            GameOver(); // Reincarnated
            Debug.Log("Game Over: Reincarnated for good performance!");
        }
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
        Debug.Log("Decrement turns");
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
        if (effect == null) {
            Debug.Log("Effect is null");
            return;
        }
        // Check for effects that aren't displayed:
        if (effect.type == EffectType.ADD_TURNS) {
            Debug.Log("Effect type: " + effect.type);
            Debug.Log("Effect: " + effect);
            Debug.Log("Effect type: " + effect.type);
            remainingTurns += turns;
        }
        // add to active/displayed buffs/debuffs
        if (activeEffects.ContainsKey(effect.type)) {
            UIEffectController currUIEffect = activeEffects[effect.type].GetComponent<UIEffectController>();
            if (effect.shouldStack) {
                Debug.Log("Effect already active, add to stack");
                currUIEffect.UpdateTurns(turns);
                activeEffects[effect.type].GetComponent<MouseOverDescription>().UpdateDescription(effect.effectDescription + "Turns: " + currUIEffect.FetchTurns());
            } else {
                Debug.Log("Effect does not stack and is already active, ignoring");
            }
        } else {
            // TODO: clean up this system this is ugly
            Debug.Log("Add new effect");
            GameObject effectMarker = Instantiate(currentEffectPrefab, currentEffectsPanel);
            activeEffects.Add(effect.type, effectMarker);
            effectMarker.GetComponent<UIEffectController>().AddEffect(effect, turns);
            effectMarker.GetComponent<MouseOverDescription>().UpdateDescription(effect.effectDescription + "Turns: " + turns);
        }
    }

    // TODO: this is a nasty large function that will be cleaned up
    private void UpdateMetersWithEffects(Action action) {
        float performaceModifier = action.PERFORMANCE_MODIFIER;
        float frustrationModifier = action.FRUSTRATION_MODIFIER;
        float willModifier = action.WILL_MODIFIER;        

        // Check active effects:
        List<EffectType> effectsToRemove = new List<EffectType>();
        // Check player effects:
        foreach(var (type, effect) in activeEffects) {
            switch (type) {
                case EffectType.ATTENTION:
                    Debug.Log("Multiplying performance change by 20% due to attention");
                    performaceModifier *= 1.2f;
                    break;
                case EffectType.HUSTLING:
                    // performance gains remove attention
                    if (performaceModifier > 0) {
                        Debug.Log("Positive performance modifier while hustling effect active");
                        RemoveAttention(1);
                    }
                    break;
                case EffectType.DRAINED:
                    // lose 1 will
                    willModifier -= 1;
                    break;
                case EffectType.CAFFIENATED:
                    // gain 2 will
                    willModifier += 2;
                    break;
            }
            // Reduce stacked turns, TODO: check if effect decays with turns
            UIEffectController effectController = effect.GetComponent<UIEffectController>();
            if (effectController.effect.shouldDecay) effectController.UpdateTurns(-1);
            if (effectController.FetchTurns() == 0) {
                effectsToRemove.Add(type);
            }
        }
        // Check curr customer effects:
        Dictionary<EffectType, GameObject> customerEffects = currCustomer.GetActiveEffects();
        foreach(var (type, effect) in customerEffects) {
            switch (type) {
                case EffectType.CALMED:
                    frustrationModifier -= 5;
                    break;
                case EffectType.CONFUSED:
                    frustrationModifier += 5;
                    break;
                case EffectType.IRATE:
                    AddNewEffects(attentionEffect, 2);
                    break;
                case EffectType.INCOHERENT:
                    // Removes 5 "Attention" when Escalated
                    if (action.actionName == "Escalate") {
                        RemoveAttention(5);
                    }
                    break;
                case EffectType.SHORTFUSE:
                    frustrationModifier *= 2;
                    break;
                case EffectType.MELLOW:
                    frustrationModifier /= 2;
                    break;
                case EffectType.ELATED:
                    performaceModifier += 5;
                    break;
            }
                    // Reduce stacked turns, TODO: check if effect decays with turns
            UIEffectController effectController = effect.GetComponent<UIEffectController>();
            if (effectController.effect.shouldDecay) effectController.UpdateTurns(-1);
            if (effectController.FetchTurns() == 0) {
                effectsToRemove.Add(type);
            }
        }
        foreach (var e in effectsToRemove) {
            Debug.Log("Remove effect");
            Destroy(activeEffects[e]);
            activeEffects.Remove(e);
        }
        
        // Update meters with after effects and artifacts values
        UpdatePerformance(performaceModifier);
        UpdateWill(willModifier);
        currCustomer.UpdateFrustration(frustrationModifier);
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
                customerIconQueue.Enqueue(Instantiate(customerIconPrefab, customerQueuePanel)); // temp, this only works while there are less customers than the size of the panel
                NextCustomer();
                break;
        } 
    }

    private void RemoveAttention(int amount) {
        if (activeEffects.ContainsKey(EffectType.ATTENTION)) {
            Debug.Log("Removing attention");
            UIEffectController attentionEffect = activeEffects[EffectType.ATTENTION].GetComponent<UIEffectController>();
            attentionEffect.UpdateTurns(-amount);
            activeEffects[EffectType.ATTENTION].GetComponent<MouseOverDescription>().UpdateDescription(
            attentionEffect.effect.effectDescription + "Turns: " + attentionEffect.FetchTurns());
            if (attentionEffect.FetchTurns() == 0) {
                Debug.Log("Remove effect");
                Destroy(activeEffects[EffectType.ATTENTION]);
                activeEffects.Remove(EffectType.ATTENTION);
            }
        }
    }
}
