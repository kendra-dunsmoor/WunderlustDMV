using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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
    private InventoryManager inventoryManager;

    [Header("------------- Prefabs -------------")]
    // Prefabs
    [SerializeField] private GameObject combatRewardsScreen;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject customer; // temp will need to have data for diff enemy types
    [SerializeField] private GameObject paperwork; // temp object placeholder
    [SerializeField] private GameObject actionButtonPrefab;
    [SerializeField] private GameObject currentEffectPrefab;
    [SerializeField] private GameObject customerIconPrefab;
    [SerializeField] private ActionEffect attentionEffect;

    [Header("------------- UI Meters -------------")]
    [SerializeField] private Slider performanceMeter;
    [SerializeField] private Slider willMeter;
    [SerializeField] private TextMeshProUGUI customerGoalText;
    [SerializeField] private TextMeshProUGUI remainingTurnsText;

    [Header("------------- Spawn Points -------------")]
    [SerializeField] private Transform currentEffectsPanel;
    [SerializeField] private Transform customerQueuePanel;
    [SerializeField] private Transform actionButtonGrid;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform offScreenPoint;
    [SerializeField] private Transform frontOfLinePoint;

    [Header("------------- Customer Queues -------------")]
    private Queue<Customer> customersInLine = new Queue<Customer>();
    private Queue<GameObject> customerIconQueue = new Queue<GameObject>();
    private Customer currCustomer;
    [Header("------------- Combat Values -------------")]
    [SerializeField, Tooltip("Customer goal set at beginning of combat scene")] private int CUSTOMER_GOAL;
    [SerializeField, Tooltip("Current performance level, UI might not update if this is changed until action taken")] private float performanceLevel; // temp range 0 to 50
    [SerializeField, Tooltip("Current will level, UI might not update if this is changed until action taken")] private float willLevel; // temp value
    [SerializeField, Tooltip("Remaining turns in combat, UI might not update if this is changed until action taken")] private int remainingTurns; // temp value
    private Dictionary<EffectType, GameObject> activeEffects = new Dictionary<EffectType, GameObject>();
    private List<Action> actionLoadout;
    [SerializeField, Tooltip("If loadout not customized from apartment use base")] private List<Action> STARTER_LOADOUT;
    public class EffectResult
    {
        public float FrustrationModifier { get; set; }
        public float PerformanceModifier { get; set; }
        public float WillModifier { get; set; }
        public bool shouldRemoveEffect { get; set; }
    }

    private float MAX_PERFORMANCE;
    public bool actionsDisabled;
    [SerializeField] private GameObject buttonsParent;

    void Start()
    {
        audioManager = 	FindFirstObjectByType<AudioManager>();
        if (audioManager != null) audioManager.PlayMusic(audioManager.combatMusic);

        // temp initialization for quick testing when game manager is null:
        if (performanceLevel == 0) performanceLevel = 50f;
        if (willLevel == 0) willLevel = 50f;
        if (CUSTOMER_GOAL == 0) CUSTOMER_GOAL = 10;
        MAX_PERFORMANCE = performanceMeter.maxValue;

        gameManager = FindFirstObjectByType<GameManager>();
        inventoryManager = FindFirstObjectByType<InventoryManager>();
        if (gameManager != null) {
            if (remainingTurns == 0) remainingTurns = CUSTOMER_GOAL + gameManager.FetchCurrentCalendarDay() - 1; // temp
            performanceLevel = gameManager.FetchPerformance();
            willLevel = gameManager.FetchWill();
        }
        performanceMeter.value = performanceLevel;
        willMeter.value = willLevel;
        willMeter.GetComponentInParent<MouseOverDescription>().UpdateDescription(willLevel + "/" + willMeter.maxValue);
        performanceMeter.GetComponentInParent<MouseOverDescription>().UpdateDescription(performanceLevel + "/" + performanceMeter.maxValue);
        remainingTurnsText.text = "Turns remaining: " + remainingTurns;
        AddActionLoadout();
        InitializeCustomerQueue();
    }

    /* Add Action Loadout: 
    * ~~~~~~~~~~~~~~~
    * Fetch player action loadout to customize class actions
    */
    private void AddActionLoadout() {
        if (gameManager != null) actionLoadout = gameManager.FetchActions();
        foreach (Action action in actionLoadout) {
            GameObject button = Instantiate(actionButtonPrefab, actionButtonGrid);
            button.GetComponent<Button>().onClick.AddListener(() => TakeAction(action));
            button.GetComponent<OnHoverChangeImage>().UpdateImages(action.baseButtonImage, action.hoverButtonImage);
            button.GetComponent<MouseOverDescription>().UpdateDescription(action.GetDescription());
        }
    }

    /* End Shift: 
    * ~~~~~~~~~~~~~~~
    * Shift completed by either running out of turns or customers
    * Add any end of shift effects and add combat rewards
    */
    private void EndShift()
    {
        DisableActions();
        // Check for game over state first:
        if (performanceLevel <=0 || performanceLevel >= performanceMeter.maxValue) return;
        // End of shift artifact effects
        inventoryManager.EndShiftArtifacts();
        // Pop up end screen
        gameManager.ShiftCompleted(performanceLevel, willLevel);
        // Get combat rewards
        if (audioManager != null) audioManager.PlaySFX(audioManager.shiftOver_Success);
        Instantiate(combatRewardsScreen, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
    }

    /* Game Over: 
    * ~~~~~~~~~~~~~~~
    * Player reincarnated or fired due to performance
    * Instantiates game over screen
    */
    private void GameOver()
    {
        DisableActions();
        // Pop up end screen
        Instantiate(gameOverMenu, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
    }

    /* Next Customer: 
    * ~~~~~~~~~~~~~~~
    * Customer removed, update customer counters, current customer, and move new customer to front
    */
    private void NextCustomer()
    {
        customerGoalText.text = CUSTOMER_GOAL - customersInLine.Count + "/" + CUSTOMER_GOAL;
        if (customersInLine.Count == 0) EndShift();
        else {
            currCustomer = customersInLine.Dequeue();
            Debug.Log("Customer dequeued");
            currCustomer.SendToFront(frontOfLinePoint);
            Destroy(customerIconQueue.Dequeue());
        }
    }
    
    /* Spawn Paperwork: 
    * ~~~~~~~~~~~~~~~
    * Customer reaches front of queue and their paperwork should be instantiated in
    * This determines whether customer should be accepted or rejected
    */
    public void SpawnPaperwork()
    {
        // TODO: Paperwork will be unique/randomized and instantiated in, just for looks rn
        paperwork.SetActive(true);
    }

    /* Initialize Customer Queue: 
    * ~~~~~~~~~~~~~~~~~~~~~~~~~~~
    * Instantiate all customers off screen at start of combat
    * Add correct images to customer queue and move first customer to front of line
    */
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
        customerGoalText.text = CUSTOMER_GOAL - customersInLine.Count + "/" + CUSTOMER_GOAL;
    }

    /* Update Performance: 
    * ~~~~~~~~~~~~~~~~~~~~~~~~~~~
    * Update performance meter after player action
    * Check for game over state
    */
    public void UpdatePerformance(float diff) {
        performanceLevel += diff;
        performanceMeter.value = performanceLevel;
        Debug.Log("Performance level updated to: " + performanceLevel);
        performanceMeter.GetComponentInParent<MouseOverDescription>().UpdateDescription(performanceLevel + "/" + performanceMeter.maxValue);
        if (performanceLevel <= 0) {
            gameManager.UpdateRunStatus(GameState.RunStatus.FIRED);
            GameOver(); // Fired
            Debug.Log("Game Over: Fired for bad performance!");
        }
        if (performanceLevel >= MAX_PERFORMANCE) {
            gameManager.UpdateRunStatus(GameState.RunStatus.REINCARNATED);
            GameOver(); // Reincarnated
            Debug.Log("Game Over: Reincarnated for good performance!");
        }
    }
    
    /* Update Will: 
    * ~~~~~~~~~~~~~
    * Update will meter after player action
    */
    public void UpdateWill(float diff) {
        willLevel += diff;
        willMeter.value = willLevel;
        Debug.Log("Will updated to: " + willLevel);
        willMeter.GetComponentInParent<MouseOverDescription>().UpdateDescription(willLevel + "/" + willMeter.maxValue);
    }

    /* Update Frustration: 
    * ~~~~~~~~~~~~~~~~~~~~
    * Update current customer frustration meter after player action
    */
    public void UpdateFrustration(float diff) {
        Debug.Log("Change curr customer frustration by " + diff);
        currCustomer.UpdateFrustration(diff);
    }

    /* Take Action:
    * ~~~~~~~~~~~~~~
    * Player selects action button
    * Check action effects and cost
    * Update performance, will, frustration, and effects based on action
    * Iterate turn counter and any active effects
    */
    public void TakeAction (Action action) {
        PlayActionSound(action);

        // Check if sufficient will available for action:
        if (willLevel - action.WILL_MODIFIER < 0) {
            Debug.Log("Insufficient will left for action: " + action.actionName);
            if (audioManager != null) audioManager.PlaySFX(audioManager.noEnergy);
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
        inventoryManager.IncrementArtifacts();
        foreach (ActionEffectStacks effectStacks in action.effects) {
            if (effectStacks.stacks < 0) RemoveEffectStacks(effectStacks.stacks, effectStacks.effect.type);
            else AddNewEffect(effectStacks.effect, effectStacks.stacks);
        }

        // Move current customer if needed:
        MoveCustomer(action.movement);

        // Check end shift state for turns:
        if (remainingTurns == 0) EndShift();
    }


    /* Add New Effects:
    * ~~~~~~~~~~~~~~
    * Add any new effects from current action to UI
    * If already active in UI increase turns
    */
    public void AddNewEffect(ActionEffect effect, int stacks) {
        // check if current action has no effect
        if (effect == null) {
            Debug.Log("Effect is null");
            return;
        }
        // Check for effects that aren't displayed:
        if (effect.type == EffectType.ADD_TURNS) {
            remainingTurns += stacks;
            return;
        }
        // add to active/displayed buffs/debuffs
        if (activeEffects.ContainsKey(effect.type)) {
            UIEffectController currUIEffect = activeEffects[effect.type].GetComponent<UIEffectController>();
            if (effect.shouldStack) {
                Debug.Log("Effect already active, add to stack");
                currUIEffect.UpdateTurns(stacks);
                activeEffects[effect.type].GetComponent<MouseOverDescription>().UpdateDescription(effect.effectDescription + "Turns: " + currUIEffect.FetchTurns());
            } else {
                Debug.Log("Effect does not stack and is already active, ignoring");
            }
        } else {
            Debug.Log("Add new effect");
            GameObject effectMarker = Instantiate(currentEffectPrefab, currentEffectsPanel);
            activeEffects.Add(effect.type, effectMarker);
            effectMarker.GetComponent<UIEffectController>().AddEffect(effect, stacks);
            effectMarker.GetComponent<MouseOverDescription>().UpdateDescription(effect.effectDescription + "Turns: " + stacks);
        }
    }

    /* Add New Effects:
    * ~~~~~~~~~~~~~~
    * Check active player and customer effects
    * Add any modifiers from effects
    */
    private void UpdateMetersWithEffects(Action action) {
        float performaceModifier = action.PERFORMANCE_MODIFIER;
        float frustrationModifier = action.FRUSTRATION_MODIFIER;
        float willModifier = action.WILL_MODIFIER;        

        List<EffectType> effectsToRemove = new List<EffectType>();
        // Check player effects:
        foreach(var (type, effectUI) in activeEffects) {
            ApplyEffectModifiers(type, effectUI.GetComponent<UIEffectController>(), performaceModifier, willModifier, frustrationModifier);
        }
        // Check curr customer effects:
        foreach(var (type, effectUI) in currCustomer.GetActiveEffects()) {
            ApplyEffectModifiers(type, effectUI.GetComponent<UIEffectController>(), performaceModifier, willModifier, frustrationModifier);
            // Temp: Annoying special case
            if (type == EffectType.INCOHERENT) {
                // Removes 5 "Attention" when Escalated
                if (action.actionName == "Escalate") {
                    RemoveEffectStacks(5, EffectType.ATTENTION);
                }
            }
        }
        // Clean up any expired effects from UI:
        foreach (var e in effectsToRemove) {
            Debug.Log("Remove effect");
            Destroy(activeEffects[e]);
            activeEffects.Remove(e);
        }
        
        // Update meters with after effects and artifacts values
        UpdatePerformance(performaceModifier);
        UpdateWill(willModifier);
        UpdateFrustration(frustrationModifier);
    }


    /* Apply Effect Modifiers:
    * ~~~~~~~~~~~~~~~~~~~~~~~~~
    * For each individual effect, update modifiers
    */
    private EffectResult ApplyEffectModifiers(EffectType effectType, UIEffectController effectController,
        float performaceModifier, float willModifier, float frustrationModifier) {

        ActionEffect effect = effectController.effect;

        // General modifiers:
        if (effect.isPercent) {
            frustrationModifier *= effect.FRUSTRATION_MODIFIER;
            performaceModifier *= effect.PERFORMANCE_MODIFIER;
            willModifier *= effect.WILL_MODIFIER;
        } else {
            frustrationModifier += effect.FRUSTRATION_MODIFIER;
            performaceModifier += effect.PERFORMANCE_MODIFIER;
            willModifier += effect.WILL_MODIFIER;               
        }

        // Any special cases that need to be hard coded for now:
        switch (effectType) {
            case EffectType.IRATE:
                AddNewEffect(attentionEffect, 2);
                break;
            case EffectType.ATTENTION:
                Debug.Log("Multiplying performance change due to attention");
                for (int i = 0; i < effectController.FetchTurns(); i++) {
                    performaceModifier *= effect.PERFORMANCE_MODIFIER;
                }
                break;
            case EffectType.HUSTLING:
                // performance gains remove attention
                if (performaceModifier > 0) {
                    Debug.Log("Positive performance modifier while hustling effect active");
                    RemoveEffectStacks(1, EffectType.ATTENTION);
                }
                break;
            case EffectType.CAFFIENATED:
                // Check if player has thermos artifact which doubles caffienated modifier
                if (gameManager.ContainsItem("A_006"))
                    willModifier += effect.WILL_MODIFIER;
                break;
        }
        EffectResult effectResult = new EffectResult { 
            FrustrationModifier = frustrationModifier, 
            PerformanceModifier = performaceModifier, 
            WillModifier = willModifier, 
            shouldRemoveEffect = false 
        };
        if (effect.shouldDecay) effectController.UpdateTurns(-1);
        if (effectController.FetchTurns() == 0) effectResult.shouldRemoveEffect = true;
        return effectResult;
    }


    /* Move Customer:
    * ~~~~~~~~~~~~~~~~
    * Update location of customer sprite
    */
    private void MoveCustomer(Action.ActionMovement movement) {
        switch (movement) {
            case Action.ActionMovement.FRONT:
                Debug.Log("Customer remains in front");
                break;
            case Action.ActionMovement.AWAY:
                Debug.Log("Customer is removed from queue");
                currCustomer.SendAway(true, offScreenPoint); // TODO: remove green accepted true thing
                // Disable actions while customer transitions
                DisableActions();
                NextCustomer();
                break;
            case Action.ActionMovement.BACK:     
                Debug.Log("Customer is moved to back of line"); 
                currCustomer.SendToBack(spawnPoint);
                // Disable actions while customer transitions
                DisableActions();
                customersInLine.Enqueue(currCustomer);
                customerIconQueue.Enqueue(Instantiate(customerIconPrefab, customerQueuePanel)); // temp, this only works while there are less customers than the size of the panel
                NextCustomer();
                break;
        } 
    }

    /* Remove Effect Stacks:
    * ~~~~~~~~~~~~~~~~~~~~~~~~~
    * Remove stacks from effect if active
    */
    public void RemoveEffectStacks(int amount, EffectType effectType) {
        if (activeEffects.ContainsKey(effectType)) {
            Debug.Log("Removing attention");
            UIEffectController effectUI = activeEffects[effectType].GetComponent<UIEffectController>();
            effectUI.UpdateTurns(-amount);
            activeEffects[effectType].GetComponent<MouseOverDescription>().UpdateDescription(
            effectUI.effect.effectDescription + "Turns: " + effectUI.FetchTurns());
            if (effectUI.FetchTurns() == 0) {
                Debug.Log("Remove effect");
                Destroy(activeEffects[effectType]);
                activeEffects.Remove(effectType);
            }
        }
    }

    /* Clear Player Conditions:
    * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    * Clear all active player conditions
    */    
    public void ClearPlayerConditions() {
        foreach(var (type, effect) in activeEffects) {
            Destroy(effect);
        }
        activeEffects = new Dictionary<EffectType, GameObject>();
    }

    public void DisableActions() {
        actionsDisabled = true;
        Button[] buttons = buttonsParent.GetComponentsInChildren<Button>();
        foreach (Button button in buttons) {
            button.interactable = false;
        }
    }

    public void EnableActions() {
        actionsDisabled = false;
        Button[] buttons = buttonsParent.GetComponentsInChildren<Button>();
        foreach (Button button in buttons) {
            button.interactable = true;
        }
    }

    private void PlayActionSound(Action action) {
        if (audioManager != null) {
            if (action.actionName == "Accept") audioManager.PlaySFX(audioManager.acceptButton);
            else if (action.actionName == "Reject") audioManager.PlaySFX(audioManager.rejectButton);
            else audioManager.PlaySFX(audioManager.specialActionButton);
        }
    }
}
