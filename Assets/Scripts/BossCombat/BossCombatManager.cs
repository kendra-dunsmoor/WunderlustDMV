using System;
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
public class BossCombatManager : MonoBehaviour
{
    private AudioManager audioManager;
    private GameManager gameManager;
    private InventoryManager inventoryManager;

    [Header("------------- Prefabs -------------")]
    [SerializeField] private GameObject combatRewardsScreen;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject actionButtonPrefab;
    [SerializeField] private GameObject currentEffectPrefab;

    [Header("------------- UI Meters -------------")]
    [SerializeField] private Slider performanceMeter;
    [SerializeField] private Slider willMeter;
    [SerializeField] private Slider bossWillMeter;
    [SerializeField] private Boss boss;

    [Header("------------- Spawn Points -------------")]
    [SerializeField] private Transform currentEffectsPanel;
    [SerializeField] private Transform actionButtonGrid;
    [Header("------------- Sound Clips -------------")]
    [SerializeField] private AudioClip neutralSound;
    [SerializeField] private AudioClip angrySound;
    [SerializeField] private AudioClip happySound;
    [SerializeField] private AudioClip openingSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip noWillSound;

    [Header("------------- Combat Values -------------")]
    [SerializeField] Action praiseAction;
    [SerializeField] Action counterPointAction;
    [SerializeField] Action factsAction;
    [SerializeField, Tooltip("Current performance level")] private float performanceLevel; // temp range 0 to 50
    [SerializeField, Tooltip("Current will level")] private float willLevel; // temp value
    [SerializeField, Tooltip("Current boss will level")] private float bossWillLevel; // temp value
    private Dictionary<EffectType, GameObject> activeEffects = new Dictionary<EffectType, GameObject>();
    private List<Action> actionLoadout;

    // New Boss State Management
    public enum BossState { Neutral, Angry, Pacified, Opening, Transition }
    private BossState currentBossState;
    private List<BossState> usableStates = new List<BossState> { BossState.Neutral, BossState.Angry, BossState.Pacified }; // states that the boss hasn't used
    private bool isBossVulnerable = false;
    private int bossStateCounter = 0; // how many turns remaining in state
    public class EffectResult
    {
        public float PerformanceModifier { get; set; }
        public float WillModifier { get; set; }
        public float BossWillModifier { get; set; }
    }

    private float MAX_PERFORMANCE;
    [SerializeField] private GameObject baseButtonsParent;
    [SerializeField] private GameObject specialButtonsParent;


    void Awake()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
        gameManager = FindFirstObjectByType<GameManager>();
        inventoryManager = FindFirstObjectByType<InventoryManager>();
    }

    void Start()
    {
        DisableActions(baseButtonsParent);
        DisableActions(specialButtonsParent);
        if (audioManager != null) audioManager.PlayMusic(audioManager.combatMusic);

        MAX_PERFORMANCE = performanceMeter.maxValue;

        if (gameManager != null)
        {
            performanceLevel = gameManager.FetchPerformance();
            willLevel = gameManager.FetchWill();
        }
        performanceMeter.value = performanceLevel;
        willMeter.value = willLevel;
        willMeter.GetComponentInParent<MouseOverDescription>().UpdateDescription(willLevel + "/" + willMeter.maxValue, "Free Will");
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
        UpdateBossWill(boss.GetStartingWill());
        SetBossState(BossState.Opening);
    }

    /* On Dialogue Choice:
    * ~~~~~~~~~~~~~~~~~~~
    * Sets the initial state of the boss based on the player's dialogue choice.
    */
    public void OpeningActionChoice(Action action)
    {
        switch (action.actionName)
        {
            case "Praise":
                SetBossState(BossState.Pacified);
                break;
            case "CounterPoint":
                SetBossState(BossState.Angry);
                break;
            case "Facts":
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
            case BossState.Opening:
                if (audioManager != null) audioManager.PlayDialogue(openingSound);
                boss.SayDialogueLine(EnemyData.LineType.OPENING);
                DisableActions(specialButtonsParent);
                EnableActions(baseButtonsParent);
                break;
            case BossState.Angry:
                OnEnterAngryState();
                break;
            case BossState.Pacified:
                OnEnterPacifiedState();
                break;
            case BossState.Neutral:
                OnEnterNeutralState();
                break;
            case BossState.Transition:
                if (audioManager != null) audioManager.PlayDialogue(openingSound);
                // TODO: update to transition dialogue prompt
                Debug.Log("Entering transition state");
                boss.SayDialogueLine(EnemyData.LineType.NEUTRAL);
                boss.SetNewPreppedAction(BossState.Transition);
                DisableActions(specialButtonsParent);
                EnableActions(baseButtonsParent);
                break;
        }
    }

    // --- State-Specific Logic ---
    private void OnEnterAngryState()
    {
        if (audioManager != null) audioManager.PlayDialogue(angrySound);
        // remove angry from usable states
        usableStates.Remove(BossState.Angry);

        // set turn timer for boss state
        bossStateCounter = 4;

        // set active boss sprite(face)

        // increase player's WILL by 5
        UpdateWill(10);
        boss.SayDialogueLine(EnemyData.LineType.NEGATIVE);

        boss.SetNewPreppedAction(BossState.Angry);

        // Allow player moves
        DisableActions(baseButtonsParent);
        EnableActions(specialButtonsParent);

        // set player condition to caffeinated
        // ?? how to set decay value?
        // ApplyEffectModifiers(EffectType.CAFFIENATED);

        // THIS WILL ACT AS THE PASSIVE
        // if PERFORMANCE_MODIFIER is negative
        // player loses 2 performanceat at end of turn

        // TODO: set boss dialogue
        // TODO: there's some dialogue for each state, but I'd leave that for polish

        // set boss available turn action to Fabricate Error

    }

    private void OnEnterPacifiedState()
    {
        if (audioManager != null) audioManager.PlayDialogue(happySound);
        // remove pacified from usable states
        usableStates.Remove(BossState.Pacified);

        // set turn timer for boss state
        bossStateCounter = 3;

        // set active boss sprite(face)

        // increase player's WILL by 5
        UpdateWill(5);

        // set player condition to Drained
        // ?? how to set decay value?
        // ApplyEffectModifiers(EffectType.DRAINED);

        // THIS WILL ACT AS THE PASSIVE
        // if PERFORMANCE_MODIFIER is positive
        // player gains 2 performance at the end of turn

        // TODO: set boss dialogue
        // TODO: there's some dialogue for each state, but I'd leave that for polish
        boss.SayDialogueLine(EnemyData.LineType.POSITIVE);
        boss.SetNewPreppedAction(BossState.Pacified);

        // set boss available turn action to Gloat

        DisableActions(baseButtonsParent);
        EnableActions(specialButtonsParent);
    }

    private void OnEnterNeutralState()
    {
        if (audioManager != null) audioManager.PlayDialogue(neutralSound);

        // remove neutral from usable states
        usableStates.Remove(BossState.Neutral);

        // set turn timer for boss state
        bossStateCounter = 2;

        // set active boss sprite(face)

        // player's WILL does not change

        // no condition applied to player

        // no need to set passive
        // one of two actions will happen at random

        // TODO: set boss dialogue
        // TODO: there's some dialogue for each state, but I'd leave that for polish
        boss.SayDialogueLine(EnemyData.LineType.NEUTRAL);
        boss.SetNewPreppedAction(BossState.Neutral);
        // set boss available turn action to [Hyper-Crit, Credit Steal] at random

        DisableActions(baseButtonsParent);
        EnableActions(specialButtonsParent);
    }

    private void OnEnterFinalPhase()
    {
        EndShift();

        // // Randomly choose one of the three states for the final phase
        // int randomState = UnityEngine.Random.Range(0, 3);
        // switch (randomState)
        // {
        //     case 0:
        //         SetBossState(BossState.Angry);
        //         break;
        //     case 1:
        //         SetBossState(BossState.Pacified);
        //         break;
        //     case 2:
        //         SetBossState(BossState.Neutral);
        //         break;
        // }
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
            button.GetComponent<MouseOverDescription>().UpdateDescription(action.GetDescription(true), action.actionName);
        }
        GameObject.FindGameObjectWithTag("AcceptButton").GetComponent<MouseOverDescription>()
            .UpdateDescription(praiseAction.GetDescription(), praiseAction.actionName);
        GameObject.FindGameObjectWithTag("RejectButton").GetComponent<MouseOverDescription>()
            .UpdateDescription(counterPointAction.GetDescription(), counterPointAction.actionName);
        GameObject.FindGameObjectWithTag("EscalateButton").GetComponent<MouseOverDescription>()
            .UpdateDescription(factsAction.GetDescription(), factsAction.actionName);
    }

    /* End Shift: 
    * ~~~~~~~~~~~~~~~
    * Shift completed by either running out of turns or customers
    * Add any end of shift effects and add combat rewards
    */
    private void EndShift()
    {
        DisableActions(baseButtonsParent);
        DisableActions(specialButtonsParent);

        // Check for game over state first:
        if (performanceLevel <= 0 || performanceLevel >= performanceMeter.maxValue) return;
        // TODO: game manager un complete function
        gameManager.RunWon();
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
        if (audioManager != null) audioManager.PlayDialogue(gameOverSound);
        DisableActions(specialButtonsParent);
        DisableActions(baseButtonsParent);
        // Pop up end screen
        // TODO update description for boss
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
    * Update boss will meter after player action
    */
    public void UpdateBossWill(float diff)
    {
        Debug.Log("Change boss will by " + diff);
        bossWillLevel += diff;
        if (bossWillLevel > boss.GetMaxWill()) bossWillLevel = boss.GetMaxWill();
        if (bossWillLevel < 0) bossWillLevel = 0;
        bossWillMeter.GetComponent<SliderCounter>().UpdateBar(bossWillLevel);
        Debug.Log("Will updated to: " + bossWillLevel);

        // if boss will power empty set isBossVulnerable to true
        if (bossWillLevel == 0)
        {
            isBossVulnerable = true;
            if (audioManager != null) audioManager.PlayDialogue(noWillSound);
        }
        if (bossWillLevel > 0) isBossVulnerable = false;
    }

    /* Take Action:
    * ~~~~~~~~~~~~~~
    * Player selects action button
    * Check action effects and cost
    * Update performance, will, bossWill, and effects based on action
    * Iterate turn counter and any active effects
    */
    public void TakeAction(Action action)
    {
        PlayActionSound(action);

        // If base action send into boss state
        if (currentBossState == BossState.Opening || currentBossState == BossState.Transition)
        {
            OpeningActionChoice(action);
        }

        // Else modify for special actions
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
        List<EffectType> cleanupEffects = new List<EffectType>();
        foreach (ActionEffectStacks effectStacks in action.effects)
        {
            // If marked as negative, remove those stacks
            if (effectStacks.stacks < 0 && effectStacks.effect.type == EffectType.ADD_TURNS)  AddNewEffect(effectStacks.effect, effectStacks.stacks); // annoying special case where negative isn't removing
            else if (effectStacks.stacks < 0)
            {
                bool shouldCleanup = RemoveEffectStacks(-effectStacks.stacks, effectStacks.effect.type);
                if (shouldCleanup) cleanupEffects.Add(effectStacks.effect.type);
            }
            // Else add new stacks
            else AddNewEffect(effectStacks.effect, effectStacks.stacks);
        }
        foreach (EffectType effect in cleanupEffects) DeleteEffect(effect);

        IncrementTurns();
    }

    /* Increment Turns
    * Increment combat turns, artifact turn counters, and active effect counters
    */
    private void IncrementTurns()
    {
        // Update turn counter for artifacts and apply any effects:
        inventoryManager.IncrementArtifacts();

        // check if boss is vulnerable
        if (isBossVulnerable)
        {
            // vulnerability modifiers
            // Name: Exposed
            // Effect: Special Actions MODIFIERS are doubled

            // boss doesn't act one turn(also ok with causing non-special action to double positive affects if it's too complicated to cause boss to lose a turn
        }

        // Check if phase is over, check if final phase needs to happen, decrease turn counter for boss state and trigger any passive effects:
        if (bossStateCounter == 0 && usableStates.Count == 0) OnEnterFinalPhase();
        else if (bossStateCounter == 0) SetBossState(BossState.Transition);
        else
        {
            boss.TakeTurn(currentBossState);
            bossStateCounter--;
        }

        // TRIGGER BOSS PASSIVE EFFECTS HERE
        // foreach (var (type, effectUI) in activeEffects)
        // {
        //     // if boss is angry and performance modifier is negative, player loses 2 performance
        //     ActionEffect effect = effectUI.GetComponent<UIEffectController>().effect;
        //     if (BossState.Angry == currentBossState && effect.PERFORMANCE_MODIFIER < 0)
        //     {
        //         UpdatePerformance(-2);
        //     }
        //     else if (BossState.Pacified == currentBossState && effect.PERFORMANCE_MODIFIER > 0)
        //     {
        //         UpdatePerformance(2);
        //     }
        //     // if boss is neutral, one of two actions will happen at random
        //     // ?? is this a good place for this?
        //     else if (BossState.Neutral == currentBossState)
        //     {
        //         System.Random rand = new System.Random();
        //         int randomAction = rand.Next(0, 2);
        //         if (randomAction == 0)
        //         {
        //             // Hyper-Crit
        //             UpdatePerformance(-5);
        //         }
        //         else
        //         {
        //             // Credit Steal
        //             UpdateBossWill(5);
        //         }
        //     }
        // }


        // Decrement all active player effects
        List<EffectType> cleanupEffects = new List<EffectType>();
        foreach (var (type, effectUI) in activeEffects)
        {
            ActionEffect effect = effectUI.GetComponent<UIEffectController>().effect;
                if (effect.shouldDecay)
                {
                    bool shouldCleanup = RemoveEffectStacks(1, effect.type);
                    if (shouldCleanup) cleanupEffects.Add(effect.type);
                }
        }
        foreach (EffectType effect in cleanupEffects) DeleteEffect(effect);
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
        float bossWillModifier = action.BOSS_WILL_MODIFIER;

        // Check player effects:
        foreach (var (type, effectUI) in activeEffects)
        {
            EffectResult effectResult = ApplyEffectModifiers(type, effectUI.GetComponent<UIEffectController>(), performaceModifier, willModifier, bossWillModifier);
            performaceModifier += effectResult.PerformanceModifier;
            willModifier += effectResult.WillModifier;
            bossWillModifier += effectResult.BossWillModifier;
        }
        // Check boss effects:
        foreach (var (type, effectUI) in boss.GetActiveEffects())
        {
            EffectResult effectResult = ApplyEffectModifiers(type, effectUI.GetComponent<UIEffectController>(), performaceModifier, willModifier, bossWillModifier);
            performaceModifier += effectResult.PerformanceModifier;
            willModifier += effectResult.WillModifier;
            bossWillModifier += effectResult.BossWillModifier;
        }

        // Update meters with after effects and artifacts values
        UpdatePerformance(performaceModifier);
        UpdateWill(willModifier);
        UpdateBossWill(bossWillModifier);
    }

    /* Apply Effect Modifiers:
    * ~~~~~~~~~~~~~~~~~~~~~~~~~
    * For each individual effect, update modifiers
    */
    private EffectResult ApplyEffectModifiers(EffectType effectType, UIEffectController effectController,
        float performaceModifier, float willModifier, float bossWillModifier)
    {

        ActionEffect effect = effectController.effect;

        // General modifiers:
        if (effect.isPercent)
        {
            performaceModifier *= effect.PERFORMANCE_MODIFIER;
            willModifier *= effect.WILL_MODIFIER;
            bossWillModifier *= effect.BOSS_WILL_MODIFIER;
        }
        else
        {
            performaceModifier += effect.PERFORMANCE_MODIFIER;
            willModifier += effect.WILL_MODIFIER;
            bossWillModifier += effect.BOSS_WILL_MODIFIER;
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
            BossWillModifier = bossWillModifier
        };
        return effectResult;
    }

    /* Remove Effect Stacks:
    * ~~~~~~~~~~~~~~~~~~~~~~~~~
    * Remove stacks from effect if active
    */
    public bool RemoveEffectStacks(int amount, EffectType effectType) {
        if (activeEffects.ContainsKey(effectType))
        {
            Debug.Log("Decreasing effectType: " + effectType + " by " + amount);
            UIEffectController effectUI = activeEffects[effectType].GetComponent<UIEffectController>();
            effectUI.UpdateTurns(-amount);
            if (effectUI.FetchTurns() == 0) return true;
            else return false;
        } return false;
    }

    /* Delete Effect:
    * ~~~~~~~~~~~~~~~~~~~~~~~~~
    * If no stacks remaining delete efect, separated to avoid collection errors
    */
    public void DeleteEffect(EffectType effectType)
    {
        Debug.Log("Cleanup effect: " + effectType);
        Destroy(activeEffects[effectType]);
        activeEffects.Remove(effectType);
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

    public void DisableActions(GameObject buttonsParent)
    {
        Button[] buttons = buttonsParent.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }
    }

    public void EnableActions(GameObject buttonsParent)
    {
        Button[] buttons = buttonsParent.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            // Only enable buttons that haven't been used yet
            if (button.tag == "EscalateButton") button.interactable = usableStates.Contains(BossState.Neutral);
            else if (button.tag == "RejectButton") button.interactable = usableStates.Contains(BossState.Angry);
            else if (button.tag == "AcceptButton") button.interactable = usableStates.Contains(BossState.Pacified);
            else button.interactable = true;
        }
    }

    private void PlayActionSound(Action action)
    {
        if (audioManager != null)
        {
            if (action.actionName == "Praise") audioManager.PlaySFX(audioManager.acceptButton);
            else if (action.actionName == "CounterPoint") audioManager.PlaySFX(audioManager.rejectButton);
            else audioManager.PlaySFX(audioManager.specialActionButton);
        }
    }
}
