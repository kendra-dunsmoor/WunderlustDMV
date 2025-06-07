using System;
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
public class BossCombatManager : MonoBehaviour
{
    [SerializeField] Dialogue combatTutorialDialogue;
    private AudioManager audioManager;
    private GameManager gameManager;
    private InventoryManager inventoryManager;

    [Header("------------- Prefabs -------------")]
    [SerializeField] private GameObject combatRewardsScreen;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject actionButtonPrefab;
    [SerializeField] private GameObject currentEffectPrefab;
    [SerializeField] private GameObject customerIconPrefab;

    [Header("------------- UI Meters -------------")]
    [SerializeField] private Slider performanceMeter;
    [SerializeField] private Slider willMeter;

    [Header("------------- Spawn Points -------------")]
    [SerializeField] private Transform currentEffectsPanel;
    [SerializeField] private Transform actionButtonGrid;
    [SerializeField] private Transform spawnPoint;

    //?? don't think we need this?
    // [Header("------------- Customer Queues -------------")]
    // private Queue<Customer> customersInLine = new Queue<Customer>();
    // private Queue<GameObject> customerIconQueue = new Queue<GameObject>();
    // private Customer currCustomer;

    [Header("------------- Combat Values -------------")]
    //!! needs new logic
    [SerializeField] Action acceptAction;
    //!! needs new logic
    [SerializeField] Action rejectAction;
    //!! needs new logic
    [SerializeField] Action escalateAction;
    [SerializeField, Tooltip("Current performance level, UI might not update if this is changed until action taken")] private float performanceLevel; // temp range 0 to 50
    [SerializeField, Tooltip("Current will level, UI might not update if this is changed until action taken")] private float willLevel; // temp value
    //!! [SerializeField, Tooltip("Current boss will level, UI might not update if this is changed until action taken")] private float bossWillLevel; // temp value
    private Dictionary<EffectType, GameObject> activeEffects = new Dictionary<EffectType, GameObject>();
    private List<Action> actionLoadout;
    [SerializeField, Tooltip("If loadout not customized from apartment use base")] private List<Action> STARTER_LOADOUT;

    // New Boss State Management
    public enum BossState { Neutral, Angry, Pacified, FinalPhase }
    private BossState currentBossState;
    private BossState[] usableStates = { BossState.Neutral, BossState.Angry, BossState.Pacified };
    //?? not sure how to add a list of possible actions
    // public enum BossAction { FabricateError, ...  };
    private bool isBossVulnerable = false;
    private int bossStateCounter;
    private int turnCounter = 0;
    public class EffectResult
    {
        public float PerformanceModifier { get; set; }
        public float WillModifier { get; set; }
        public float BossWillModifier { get; set; }
    }

    private float MAX_PERFORMANCE;
    public bool actionsDisabled;
    [SerializeField] private GameObject buttonsParent;

    void Awake()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
        gameManager = FindFirstObjectByType<GameManager>();
        inventoryManager = FindFirstObjectByType<InventoryManager>();
    }

    void Start()
    {
        if (audioManager != null) audioManager.PlayMusic(audioManager.combatMusic);

        // temp initialization for quick testing when game manager is null:
        if (performanceLevel == 0) performanceLevel = 50f;
        if (willLevel == 0) willLevel = 50f;
        MAX_PERFORMANCE = performanceMeter.maxValue;

        if (gameManager != null)
        {
            performanceLevel = gameManager.FetchPerformance();
            willLevel = gameManager.FetchWill();
        }
        performanceMeter.value = performanceLevel;
        willMeter.value = willLevel;
        willMeter.GetComponentInParent<MouseOverDescription>().UpdateDescription(willLevel + "/" + willMeter.maxValue, "FreeWill");
        performanceMeter.GetComponentInParent<MouseOverDescription>().UpdateDescription(performanceLevel + "/" + performanceMeter.maxValue, "Performance");
        AddActionLoadout();
        StartBossEncounter();
    }

    /* Start Boss Encounter: 
    * ~~~~~~~~~~~~~~~~~~~~~
    * Begins the encounter with a dialogue from the boss.
    */
    private void StartBossEncounter()
    {
        // FindObjectOfType<DialogueManager>().StartDialogue(bossIntroDialogue, OnDialogueChoice);

        // OnDialogueChoice(0); // Example for Angry
        // OnDialogueChoice(1); // Example for Pacified
        // OnDialogueChoice(2); // Example for Neutral
    }

    /* On Dialogue Choice:
    * ~~~~~~~~~~~~~~~~~~~
    * Sets the initial state of the boss based on the player's dialogue choice.
    */
    public void OnDialogueChoice(int choiceIndex)
    {
        switch (choiceIndex)
        {
            case 0:
                SetBossState(BossState.Angry);
                break;
            case 1:
                SetBossState(BossState.Pacified);
                break;
            case 2:
                SetBossState(BossState.Neutral);
                break;
        }
    }

    /* Set Boss State:
    * ~~~~~~~~~~~~~~~
    * Transitions the boss to a new state and applies any state-specific logic.
    */
    private void SetBossState(BossState newState)
    {
        currentBossState = newState;
        Debug.Log("Boss has entered state: " + newState);

        switch (newState)
        {
            case BossState.Angry:
                OnEnterAngryState();
                break;
            case BossState.Pacified:
                OnEnterPacifiedState();
                break;
            case BossState.Neutral:
                OnEnterNeutralState();
                break;
            case BossState.FinalPhase:
                OnEnterFinalPhase();
                break;
        }
    }

    // --- State-Specific Logic ---
    private void OnEnterAngryState()
    {
        // Boss says angry text
        // boss sprite(face) changes
        // boss gains affect of angry

    }

    private void OnEnterPacifiedState()
    {
        // ADD PACIFIED STATE LOGIC HERE
    }

    private void OnEnterNeutralState()
    {
        // ADD NEUTRAL STATE LOGIC HERE
    }

    private void OnEnterFinalPhase()
    {
        // Randomly choose one of the three states for the final phase
        int randomState = UnityEngine.Random.Range(0, 3);
        switch (randomState)
        {
            case 0:
                SetBossState(BossState.Angry);
                break;
            case 1:
                SetBossState(BossState.Pacified);
                break;
            case 2:
                SetBossState(BossState.Neutral);
                break;
        }
    }

    private void getBossStatePassiveEffect(BossState state)
    {
        //gets affect from state that 
    }

    /* Add Action Loadout: 
    * ~~~~~~~~~~~~~~~
    * Fetch player action loadout to customize class actions
    */
    private void AddActionLoadout()
    {
        if (gameManager != null) actionLoadout = gameManager.FetchActions();
        foreach (Action action in actionLoadout)
        {
            GameObject button = Instantiate(actionButtonPrefab, actionButtonGrid);
            button.GetComponent<Button>().onClick.AddListener(() => TakeAction(action));
            button.GetComponent<OnHoverChangeImage>().UpdateImages(action.baseButtonImage, action.hoverButtonImage);
            button.GetComponent<MouseOverDescription>().UpdateDescription(action.GetDescription(), action.actionName);
        }
        //!! needs updating
        GameObject.FindGameObjectWithTag("AcceptButton").GetComponent<MouseOverDescription>()
            .UpdateDescription(acceptAction.GetDescription(), acceptAction.actionName);
        GameObject.FindGameObjectWithTag("RejectButton").GetComponent<MouseOverDescription>()
            .UpdateDescription(rejectAction.GetDescription(), rejectAction.actionName);
        GameObject.FindGameObjectWithTag("EscalateButton").GetComponent<MouseOverDescription>()
            .UpdateDescription(escalateAction.GetDescription(), escalateAction.actionName);
    }

    /* End Shift: 
    // * ~~~~~~~~~~~~~~~
    // * Shift completed by either running out of turns or customers
    // * Add any end of shift effects and add combat rewards
    // */
    // private void EndShift(bool completedQueue)
    // {
    //     DisableActions();
    //     // Check if customers ran out
    //     if (completedQueue) UpdatePerformance(EARLY_FINISH_PENALTY);
    //     // Check for game over state first:
    //     if (performanceLevel <= 0 || performanceLevel >= performanceMeter.maxValue) return;
    //     // End of shift artifact effects
    //     inventoryManager.EndShiftArtifacts();
    //     UpdateAttention(END_SHIFT_ATTENTION_MODIFIER);
    //     // Pop up end screen
    //     gameManager.ShiftCompleted(performanceLevel, willLevel, attentionLevel);
    //     // Get combat rewards
    //     if (audioManager != null) audioManager.PlaySFX(audioManager.shiftOver_Success);
    //     GameObject screen = Instantiate(combatRewardsScreen, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
    //     screen.GetComponent<CombatRewardsController>().markEarlyFinish(completedQueue);
    // }

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

    /* Update Performance: 
    * ~~~~~~~~~~~~~~~~~~~~~~~~~~~
    * Update performance meter after player action
    * Check for game over state
    */
    public void UpdatePerformance(float diff)
    {
        performanceLevel += (float)Math.Round(diff);
        performanceMeter.GetComponent<SliderCounter>().UpdateBar(performanceLevel);
        Debug.Log("Performance level updated to: " + performanceLevel);
        if (performanceLevel <= 0)
        {
            gameManager.UpdateRunStatus(GameState.RunStatus.FIRED);
            GameOver(); // Fired
            Debug.Log("Game Over: Fired for bad performance!");
        }
        if (performanceLevel >= MAX_PERFORMANCE)
        {
            gameManager.UpdateRunStatus(GameState.RunStatus.REINCARNATED);
            GameOver(); // Reincarnated
            Debug.Log("Game Over: Reincarnated for good performance!");
        }
    }

    /* Update Will: 
    * ~~~~~~~~~~~~~
    * Update will meter after player action
    */
    public void UpdateWill(float diff)
    {
        willLevel += diff;
        if (willLevel > gameManager.FetchMaxWill()) willLevel = gameManager.FetchMaxWill();
        if (willLevel < 0) willLevel = 0;
        willMeter.GetComponent<SliderCounter>().UpdateBar(willLevel);
        Debug.Log("Will updated to: " + willLevel);
    }

    /* Update Boss Will: 
    * ~~~~~~~~~~~~~~~~~~~~
    * Update current customer frustration meter after player action
    */
    public void UpdateBossWill(float diff)
    {
        Debug.Log("Change boss will by " + diff);
        bossWillLevel += diff;
        if (bossWillLevel > gameManager.FetchMaxWill()) bossWillLevel = gameManager.FetchMaxWill();
        if (bossWillLevel < 0) bossWillLevel = 0;
        willMeter.GetComponent<SliderCounter>().UpdateBar(bossWillLevel);
        Debug.Log("Will updated to: " + bossWillLevel);
    }

    /* Take Action:
    * ~~~~~~~~~~~~~~
    * Player selects action button
    * Check action effects and cost
    * Update performance, will, frustration, and effects based on action
    * Iterate turn counter and any active effects
    */
    public void TakeAction(Action action)
    {
        PlayActionSound(action);

        // Check if sufficient will available for action:
        if (willLevel - action.WILL_MODIFIER < 0)
        {
            Debug.Log("Insufficient will left for action: " + action.actionName);
            if (audioManager != null) audioManager.PlaySFX(audioManager.noEnergy);
            return;
        }

        // Update meters:
        Debug.Log("Taking action: " + action.actionName);
        UpdateMetersWithEffects(action);

        // Apply new effects for next turn
        foreach (ActionEffectStacks effectStacks in action.effects)
        {
            if (effectStacks.stacks < 0) RemoveEffectStacks(effectStacks.stacks, effectStacks.effect.type);
            else AddNewEffect(effectStacks.effect, effectStacks.stacks);
        }

        IncrementTurns();
    }

    /* Increment Turns
    * Increment combat turns, artifact turn counters, and active effect counters
    */
    private void IncrementTurns()
    {
        // Update turn counter for artifacts and apply any effects:
        inventoryManager.IncrementArtifacts();

        // Update turn counter for active effects:
        foreach (var (type, effectUI) in activeEffects)
        {
            ActionEffect effect = effectUI.GetComponent<UIEffectController>().effect;
            if (effect.shouldDecay) RemoveEffectStacks(1, effect.type);
        }
        boss.IncrementActiveEffects();

        // Check to trigger tutorial
        // if (gameManager.InTutorial() && remainingTurns == 7)
        //     FindFirstObjectByType<DialogueManager>().StartDialogue(combatTutorialDialogue);
    }


    /* Add New Effects:
    * ~~~~~~~~~~~~~~
    * Add any new effects from current action to UI
    * If already active in UI increase turns
    */
    public void AddNewEffect(ActionEffect effect, int stacks)
    {
        // check if current action has no effect
        if (effect == null)
        {
            Debug.Log("Effect is null");
            return;
        }

        // Check if should apply to customer or player
        if (effect.target == TargetType.ENEMY) boss.AddNewEnemyEffect(effect, stacks);
        else AddNewPlayerEffect(effect, stacks);
    }

    /* Add New Player Effect
    * ~~~~~~~~~~~~~~
    * Add new effect to player panel
    */
    public void AddNewPlayerEffect(ActionEffect effect, int stacks)
    {
        // Add to active/displayed buffs/debuffs
        if (activeEffects.ContainsKey(effect.type))
        {
            UIEffectController currUIEffect = activeEffects[effect.type].GetComponent<UIEffectController>();
            if (effect.shouldStack)
            {
                Debug.Log("Effect already active, add to stack");
                currUIEffect.UpdateTurns(stacks);
            }
            else
            {
                Debug.Log("Effect does not stack and is already active, ignoring");
            }
        }
        else
        {
            Debug.Log("Add new effect");
            GameObject effectMarker = Instantiate(currentEffectPrefab, currentEffectsPanel);
            activeEffects.Add(effect.type, effectMarker);
            effectMarker.GetComponent<UIEffectController>().AddEffect(effect, stacks);
        }
    }

    /* Update Meters With Effects
    * ~~~~~~~~~~~~~~
    * Check active player and customer effects
    * Add any modifiers from effects
    */
    private void UpdateMetersWithEffects(Action action)
    {
        float performaceModifier = action.PERFORMANCE_MODIFIER;
        float willModifier = action.WILL_MODIFIER;

        // Check player effects:
        foreach (var (type, effectUI) in activeEffects)
        {
            EffectResult effectResult = ApplyEffectModifiers(type, effectUI.GetComponent<UIEffectController>(), performaceModifier, willModifier);
            performaceModifier += effectResult.PerformanceModifier;
            willModifier += effectResult.WillModifier;
        }
        // Check boss effects:
        foreach (var (type, effectUI) in boss.GetActiveEffects())
        {
            EffectResult effectResult = ApplyEffectModifiers(type, effectUI.GetComponent<UIEffectController>(), performaceModifier, willModifier);
            performaceModifier += effectResult.PerformanceModifier;
            willModifier += effectResult.WillModifier;
            // Temp: Annoying special case
        }

        // Update meters with after effects and artifacts values
        UpdatePerformance(performaceModifier);
        UpdateWill(willModifier);
    }

    /* Apply Effect Modifiers:
    * ~~~~~~~~~~~~~~~~~~~~~~~~~
    * For each individual effect, update modifiers
    */
    private EffectResult ApplyEffectModifiers(EffectType effectType, UIEffectController effectController,
        float performaceModifier, float willModifier)
    {

        ActionEffect effect = effectController.effect;

        // General modifiers:
        if (effect.isPercent)
        {
            performaceModifier *= effect.PERFORMANCE_MODIFIER;
            willModifier *= effect.WILL_MODIFIER;
        }
        else
        {
            performaceModifier += effect.PERFORMANCE_MODIFIER;
            willModifier += effect.WILL_MODIFIER;
        }

        // Any special cases that need to be hard coded for now:
        switch (effectType)
        {
            case EffectType.CAFFIENATED:
                // Check if player has thermos artifact which doubles caffienated modifier
                if (gameManager.ContainsItem("A_006"))
                    willModifier += effect.WILL_MODIFIER;
                break;
        }
        EffectResult effectResult = new EffectResult
        {
            PerformanceModifier = performaceModifier,
            WillModifier = willModifier,
        };
        return effectResult;
    }

    /* Remove Effect Stacks:
    * ~~~~~~~~~~~~~~~~~~~~~~~~~
    * Remove stacks from effect if active
    */
    public void RemoveEffectStacks(int amount, EffectType effectType)
    {
        if (activeEffects.ContainsKey(effectType))
        {
            Debug.Log("Decreasing effectType: " + effectType + " by " + amount);
            UIEffectController effectUI = activeEffects[effectType].GetComponent<UIEffectController>();
            effectUI.UpdateTurns(-amount);
            activeEffects[effectType].GetComponent<MouseOverDescription>().UpdateDescription(
            effectUI.effect.effectDescription + "\nTurns: " + effectUI.FetchTurns(), effectUI.effect.effectName);
            if (effectUI.FetchTurns() == 0)
            {
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
    public void ClearPlayerConditions()
    {
        foreach (var (type, effect) in activeEffects)
        {
            Destroy(effect);
        }
        activeEffects = new Dictionary<EffectType, GameObject>();
    }

    public void DisableActions()
    {
        actionsDisabled = true;
        Button[] buttons = buttonsParent.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }
    }

    public void EnableActions()
    {
        actionsDisabled = false;
        Button[] buttons = buttonsParent.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            button.interactable = true;
        }
    }

    private void PlayActionSound(Action action)
    {
        if (audioManager != null)
        //TODO: needs to be updated with sounds for nepobaby
        {
        }
    }
}
