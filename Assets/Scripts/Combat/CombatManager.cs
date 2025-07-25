using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static ActionEffect;

/*
* Combat Manager
* ~~~~~~~~~~~~~~~
* Initialize and manager queue of customers, meters, and goals/progress
*/
public class CombatManager : MonoBehaviour
{
    [SerializeField] EnemySpawner enemySpawner;
    [SerializeField] SceneFader sceneFader;
    private AudioManager audioManager;
    private GameManager gameManager;
    private InventoryManager inventoryManager;
    List<Certificate> playerCerts;

    [Header("------------- Tutorials -------------")]
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] TutorialManager tutorialManager;
    [SerializeField] Dialogue combatTutorialDialogue;
    [SerializeField] TutorialDialogue openingTutorial;
    [SerializeField] TutorialDialogue secondCombatTutorial;

    [Header("------------- Prefabs -------------")]
    [SerializeField] private GameObject combatRewardsScreen;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject paperwork; // temp object placeholder
    [SerializeField] private GameObject actionButtonPrefab;
    [SerializeField] private GameObject currentEffectPrefab;
    [SerializeField] private GameObject customerIconPrefab;
    //  [SerializeField] private ActionEffect attentionEffect;

    [Header("------------- UI Meters -------------")]
    [SerializeField] private Slider performanceMeter;
    [SerializeField] private Slider willMeter;
    [SerializeField] private TextMeshProUGUI customerGoalText;
    [SerializeField] private TextMeshProUGUI remainingTurnsText;
    [SerializeField] private TextMeshProUGUI attentionTracker;

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
    [SerializeField] Action acceptAction;
    [SerializeField] Action rejectAction;
    [SerializeField] Action escalateAction;
    [SerializeField, Tooltip("Remove this much attention at end of shift")] private int END_SHIFT_ATTENTION_MODIFIER = -30;
    [SerializeField, Tooltip("Modifier for going through all customers before shift end")] private int EARLY_FINISH_PENALTY;
    [SerializeField, Tooltip("Customer goal set at beginning of combat scene")] private int CUSTOMER_GOAL;
    [SerializeField, Tooltip("Current performance level, UI might not update if this is changed until action taken")] private float performanceLevel = 0;
    [SerializeField, Tooltip("Current attention level, UI might not update if this is changed until action taken")] private float attentionLevel = 0;
    [SerializeField, Tooltip("Current will level, UI might not update if this is changed until action taken")] private float willLevel = 0;
    [SerializeField, Tooltip("Remaining turns in combat, UI might not update if this is changed until action taken")] private int remainingTurns; // temp value
    private Dictionary<EffectType, GameObject> activeEffects = new Dictionary<EffectType, GameObject>();
    private List<Action> actionLoadout;
    [SerializeField, Tooltip("If loadout not customized from apartment use base")] private List<Action> STARTER_LOADOUT;
    public class EffectResult
    {
        public float FrustrationModifier { get; set; }
        public float PerformanceModifier { get; set; }
        public float WillModifier { get; set; }
        public float AttentionModifier { get; set; }
    }

    private float MAX_PERFORMANCE;
    public bool takenAction;
    public bool actionsDisabled;
    [SerializeField] private GameObject buttonsParent;

    void Awake()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
        gameManager = FindFirstObjectByType<GameManager>();
        inventoryManager = FindFirstObjectByType<InventoryManager>();
        sceneFader.gameObject.SetActive(true);
    }

    void Start()
    {
        if (audioManager != null) audioManager.PlayMusic(audioManager.combatMusic);
        if (gameManager != null)
        {
            if (gameManager.InTutorial()) remainingTurns = 5;
            else remainingTurns = 15;
            if (gameManager.InTutorial()) CUSTOMER_GOAL = 5;
            else CUSTOMER_GOAL = 10;
            // Regular week difficulty curve
            if (gameManager.FetchCurrentCalendarDay() > 1) remainingTurns = 20; 

            MAX_PERFORMANCE = performanceMeter.maxValue;
            UpdatePerformance(gameManager.FetchPerformance());
            UpdateAttention(gameManager.FetchAttention());
            UpdateWill(gameManager.FetchWill());
            playerCerts = gameManager.FetchCertificates();
        }

        // Check for tutorials:
        if (gameManager.InTutorial() && gameManager.FetchCurrentCalendarDay() == 0)
        {
            tutorialManager.StartTutorial(openingTutorial);
            GameObject.FindGameObjectWithTag("ClassPanel").SetActive(false);
        }
        // Hard code for 2nd day tutorial:
        if (gameManager.InTutorial() && gameManager.FetchCurrentCalendarDay() == 1)
        {
            remainingTurns = 15; //Curve up in difficulty
            CUSTOMER_GOAL = 10;
            tutorialManager.StartTutorial(secondCombatTutorial);
            gameManager.UpdateTutorialStatus(false);
        }

        takenAction = false;

        remainingTurnsText.text = remainingTurns.ToString();
        AddActionLoadout();
        InitializeCustomerQueue();
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
        GameObject.FindGameObjectWithTag("AcceptButton").GetComponent<MouseOverDescription>()
            .UpdateDescription(acceptAction.GetDescription(), acceptAction.actionName);
        GameObject.FindGameObjectWithTag("RejectButton").GetComponent<MouseOverDescription>()
            .UpdateDescription(rejectAction.GetDescription(), rejectAction.actionName);
        GameObject.FindGameObjectWithTag("EscalateButton").GetComponent<MouseOverDescription>()
            .UpdateDescription(escalateAction.GetDescription(), escalateAction.actionName);
    }

    /* End Shift: 
    * ~~~~~~~~~~~~~~~
    * Shift completed by either running out of turns or customers
    * Add any end of shift effects and add combat rewards
    */
    private void EndShift()
    {
        bool completedQueue = false;

        DisableActions();
        // Check for game over state first:
        if (performanceLevel <= 0 || performanceLevel >= performanceMeter.maxValue) return;

        // Check if customers ran out
        if (remainingTurns > 0)
        {
            completedQueue = true;
            UpdatePerformance(EARLY_FINISH_PENALTY);
            gameManager.EarlyShift();
        }
        // End of shift artifact effects
        inventoryManager.EndShiftArtifacts();
        UpdateAttention(END_SHIFT_ATTENTION_MODIFIER);

        // Pop up end screen
        gameManager.ShiftCompleted(performanceLevel, willLevel, attentionLevel);
        // Get combat rewards
        if (audioManager != null) audioManager.PlaySFX(audioManager.shiftOver_Success);
        GameObject screen = Instantiate(combatRewardsScreen, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
        screen.GetComponent<CombatRewardsController>().markEarlyFinish(completedQueue);
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
        if (customersInLine.Count > 0)
        {
            customerGoalText.text = CUSTOMER_GOAL - (customersInLine.Count-1) + "/" + CUSTOMER_GOAL;
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
        paperwork.GetComponent<Paperwork>()
            .CreatePaperwork(currCustomer.GetPaperworkOdds());
    }

    /* Initialize Customer Queue: 
    * ~~~~~~~~~~~~~~~~~~~~~~~~~~~
    * Instantiate all customers off screen at start of combat
    * Add correct images to customer queue and move first customer to front of line
    */
    private void InitializeCustomerQueue()
    {
        Debug.Log("Initializing queue");
        enemySpawner.SpawnEnemies(CUSTOMER_GOAL, out customersInLine, out customerIconQueue);
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
    public void UpdatePerformance(float diff)
    {
        Debug.Log("Performance: " + performanceLevel);
        performanceLevel += (float)Math.Round(diff);
        Debug.Log("Performance: " + performanceLevel);
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

    /* Update Attention: 
    * ~~~~~~~~~~~~~~~~~~~
    * Update attention after player action
    */
    public void UpdateAttention(float diff)
    {
        attentionLevel += diff;
        if (attentionLevel < 0) attentionLevel = 0;
        if (attentionLevel > 200) attentionLevel = 200;  //Testing 200% cap, 100% makes sense but was kinda boring
        attentionTracker.text = attentionLevel + "%";
        Debug.Log("Attention updated to: " + attentionLevel);
        attentionTracker.transform.parent.gameObject.GetComponent<MouseOverDescription>()
            .UpdateDescription("Increasing performance changes by " + attentionLevel + "%", "Attention");
    }

    /* Update Frustration: 
    * ~~~~~~~~~~~~~~~~~~~~
    * Update current customer frustration meter after player action
    */
    public void UpdateFrustration(float diff)
    {

        if (playerCerts.Any(c => c.type == Certificate.CertificateType.ANGER_MANAGE))
        {
            diff *= .8f;
        }

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
    public void TakeAction(Action action)
    {
        // Check if sufficient will available for action:
        if (willLevel + action.WILL_MODIFIER < 0)
        {
            Debug.Log("Insufficient will left for action: " + action.actionName);
            if (audioManager != null) audioManager.PlaySFX(audioManager.noEnergy);
            return;
        }
        
        takenAction = true;
        PlayActionSound(action);

        // Update meters:
        Debug.Log("Taking action: " + action.actionName);
        UpdateMetersWithEffects(action);

        // Apply new effects for next turn
        List<EffectType> cleanupEffects = new List<EffectType>();
        foreach (ActionEffectStacks effectStacks in action.effects)
        {
            // If marked as negative, remove those stacks
            if (effectStacks.stacks < 0 && effectStacks.effect.type == EffectType.ADD_TURNS) AddNewEffect(effectStacks.effect, effectStacks.stacks); // annoying special case where negative isn't removing
            else if (effectStacks.stacks < 0)
            {
                bool shouldCleanup = RemoveEffectStacks(-effectStacks.stacks, effectStacks.effect.type);
                if (shouldCleanup) cleanupEffects.Add(effectStacks.effect.type);
            }
            // Else add new stacks
            else AddNewEffect(effectStacks.effect, effectStacks.stacks);
        }
        foreach (EffectType effect in cleanupEffects) DeleteEffect(effect);

        // Move current customer if needed:
       /* BUGGED if (gameManager.ContainsItem("A_012"))
        {
            if (action.actionName == "Reject") MoveCustomer(Action.ActionMovement.FRONT, action.actionName);
        }
        else*/
        MoveCustomer(action.movement, action.actionName);

        IncrementTurns();

        //Had to make last customer condition, was ending shift if you delayed last customer
        if (remainingTurns <= 0 || (customersInLine.Count <= 0 && action.movement != Action.ActionMovement.FRONT)) EndShift();
    }

    /* Increment Turns
    * Increment combat turns, artifact turn counters, and active effect counters
    */
    private void IncrementTurns()
    {
        // Decrease remaining turn count and increment active effects
        remainingTurns--;
        Debug.Log("Turns remaining: " + remainingTurns);
        remainingTurnsText.text = remainingTurns.ToString();

        // Update turn counter for artifacts and apply any effects:
        inventoryManager.IncrementArtifacts();

        List<EffectType> cleanupEffects = new List<EffectType>();
        // Update turn counter for active effects:
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
        currCustomer.IncrementActiveEffects();

        // Check to trigger astaroth tutorial
        if (gameManager.InTutorial() && remainingTurns == 3)
            dialogueManager.StartDialogue(combatTutorialDialogue);
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
        // Check for effects that aren't displayed:
        if (effect.type == EffectType.ADD_TURNS)
        {
            remainingTurns += stacks;
            remainingTurnsText.text = remainingTurns.ToString();
            Debug.Log("Skipping turns: " + stacks);
            return;
        }
        // Check if should apply to customer or player

        if (effect.target == TargetType.ENEMY) currCustomer.AddNewEnemyEffect(effect, stacks);
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
        float frustrationModifier = action.FRUSTRATION_MODIFIER;
        float willModifier = action.WILL_MODIFIER;
        float attentionModifier = action.ATTENTION_MODIFIER;

        // Check for correct paperwork choice if accept/reject
        if (action.INCORRECT_CHOICE_ATTENTION_MODIFIER != 0 &&
            !MadeCorrectPaperworkChoice(action.actionName))
        {
            Debug.Log("Incorrect choice for paperwork, apply negative performance and attention");
            attentionModifier = action.INCORRECT_CHOICE_ATTENTION_MODIFIER;
            performaceModifier = -action.PERFORMANCE_MODIFIER;
        }

        // Check player effects:
        foreach (var (type, effectUI) in activeEffects)
        {
            EffectResult effectResult = ApplyEffectModifiers(type, effectUI.GetComponent<UIEffectController>());
            performaceModifier += effectResult.PerformanceModifier;
            willModifier += effectResult.WillModifier;
            frustrationModifier += effectResult.FrustrationModifier;
            attentionModifier += effectResult.AttentionModifier;

            if (type == EffectType.MADE_MISTAKE)
            {
                // Check for additional attention penalty
                if (action.actionName == "Make Mistake")
                {
                    if (playerCerts.Any(c => c.type == Certificate.CertificateType.DATA_ENTRY)) attentionModifier += 10;
                    else  attentionModifier += 15;
                }
            }
            if (type == EffectType.HUSTLING)
            {
                // performance gains remove attention
                if (performaceModifier > 0)
                {
                    Debug.Log("Positive performance modifier while hustling effect active");
                    UpdateAttention(-10);
                }
            }
        }
        // Check curr customer effects:
        foreach (var (type, effectUI) in currCustomer.GetActiveEffects())
        {
            EffectResult effectResult = ApplyEffectModifiers(type, effectUI.GetComponent<UIEffectController>());
            performaceModifier += effectResult.PerformanceModifier;
            willModifier += effectResult.WillModifier;
            frustrationModifier += effectResult.FrustrationModifier;
            attentionModifier += effectResult.AttentionModifier;


            // Temp: Annoying special case
            if (type == EffectType.INCOHERENT)
            {
                // Set attention to 0 when escalated
                if (action.actionName == "Escalate")
                    attentionModifier = -attentionLevel;
            }
            // Temp: Annoying special case
            else if (type == EffectType.SHORTFUSE)
            {

                if (action.actionName == "Escalate" || action.actionName == "Reject")
                    attentionModifier += 20;
            }
            // Temp: Annoying special case
            else if (type == EffectType.MELLOW)
            {
                if (action.actionName == "Hustle") frustrationModifier += 30;
                else frustrationModifier -= 5;
            }
        }

        // Update meters with after effects and artifacts values
        UpdatePerformance(performaceModifier * (1 + attentionLevel / 100));
        UpdateAttention(attentionModifier);
        UpdateWill(willModifier);
        UpdateFrustration(frustrationModifier);
    }

    /* Apply Effect Modifiers:
    * ~~~~~~~~~~~~~~~~~~~~~~~~~
    * For each individual effect, update modifiers
    */
    private EffectResult ApplyEffectModifiers(EffectType effectType, UIEffectController effectController)
    {

        ActionEffect effect = effectController.effect;

        // General modifiers:
        float frustrationModifier = effect.FRUSTRATION_MODIFIER;
        float performaceModifier = effect.PERFORMANCE_MODIFIER;
        float willModifier = effect.WILL_MODIFIER;
        float attentionModifier = effect.ATTENTION_MODIFIER;

        // Any special cases that need to be hard coded for now:
        switch (effectType)
        {
            case EffectType.IRATE:
                UpdateAttention(20);
                break;
            case EffectType.CAFFIENATED:
                // Check if player has thermos artifact which doubles caffienated modifier
                if (gameManager.ContainsItem("A_006")) willModifier += effect.WILL_MODIFIER*2;
                break;
        }
        EffectResult effectResult = new EffectResult
        {
            FrustrationModifier = frustrationModifier,
            PerformanceModifier = performaceModifier,
            WillModifier = willModifier,
            AttentionModifier = attentionModifier
        };
        return effectResult;
    }

    /* Move Customer:
    * ~~~~~~~~~~~~~~~~
    * Update location of customer sprite
    */
    private void MoveCustomer(Action.ActionMovement movement, string actionName)
    {

        switch (movement)
        {
            case Action.ActionMovement.FRONT:
                Debug.Log("Customer remains in front");
                // Disable actions while customer takes their turn
                DisableActions();
                currCustomer.TakeTurn();
                break;
            case Action.ActionMovement.AWAY:
                Debug.Log("Customer is removed from queue");
                DisableActions();
                StartCoroutine(CustomerLeavingVisuals(actionName));
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
    * Return if effect type should be removed from list, separated to avoid collection errors
    */
    public bool RemoveEffectStacks(int amount, EffectType effectType)
    {
        if (activeEffects.ContainsKey(effectType))
        {
            Debug.Log("Decreasing effectType: " + effectType + " by " + amount);
            UIEffectController effectUI = activeEffects[effectType].GetComponent<UIEffectController>();
            effectUI.UpdateTurns(-amount);
            // activeEffects[effectType].GetComponent<MouseOverDescription>().UpdateDescription(effectUI.effect.effectDescription + "\nTurns: " + effectUI.FetchTurns(), effectUI.effect.effectName);
            if (effectUI.FetchTurns() == 0) return true;
            else return false;
        }
        return false;
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

    private bool MadeCorrectPaperworkChoice(string actionName)
    {
        bool wasAcceptable = paperwork.GetComponent<Paperwork>().isAcceptable;
        if (actionName == "Accept") return wasAcceptable;
        else return !wasAcceptable;
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
        {
            if (action.actionName == "Accept") audioManager.PlaySFX(audioManager.acceptButton);
            else if (action.actionName == "Reject") audioManager.PlaySFX(audioManager.rejectButton);
            else audioManager.PlaySFX(audioManager.specialActionButton);
        }
    }

    // Collect all relevant info for UI visible changes
    // Run through them all in coroutine
    // Only re-enable actions once completed
    private IEnumerator CustomerLeavingVisuals(string actionName)
    {

        // Info needed:

        // if accept or reject shake paperwork and play corresponding sound
        // give time for enemy to say relevant line before enemy leaves

        // Add UI shake to paperwork:
        yield return new WaitForSeconds(0.25f);
        paperwork.GetComponent<UIShake>().StartShake();
        yield return new WaitForSeconds(0.4f); // shake duration
        if (MadeCorrectPaperworkChoice(actionName)) audioManager.PlaySFX(audioManager.correct);
        else audioManager.PlaySFX(audioManager.incorrect);

        // Customer dialogue
        if (actionName == "Accept") currCustomer.SayDialogueLine(EnemyData.LineType.POSITIVE);
        else currCustomer.SayDialogueLine(EnemyData.LineType.NEGATIVE);
        yield return new WaitForSeconds(1f); // dialogue duration

        currCustomer.SendAway(actionName == "Accept", offScreenPoint);
        // toogle paperwork visibility for next customer:
        paperwork.SetActive(false);
        NextCustomer();
    }
}
