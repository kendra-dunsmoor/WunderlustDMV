using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using static ActionEffect;
using static EnemyData;
using TMPro;
using System;

// Manage Frustration Bar, effects, and movement/reactions
public class Customer : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private FloatingHealthBar frustrationMeter;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [Header("------------- Sound Clips -------------")]
    [SerializeField] private AudioClip turnSound;
    [SerializeField] private AudioClip angrySound;
    [SerializeField] private AudioClip happySound;
    [SerializeField] private AudioClip openingSound;

    [Header("------------- Enemy Actions -------------")]
    [SerializeField] private GameObject actionTelegraph;
    private EnemyAction preppedAction;

    [Header("------------- Effects -------------")]
    [SerializeField] private GameObject currentEffectPrefab;
    [SerializeField] private Transform currentEffectsPanel;
    [SerializeField] private Dictionary<EffectType, GameObject> activeEffects = new Dictionary<EffectType, GameObject>();
    [SerializeField] private ActionEffect irateEffect;
    [SerializeField] private ActionEffect elatedEffect;
    private float frustrationLevel;

    private float SHAKE_DURATION = 1f;
    private float DIALOGUE_DURATION = 1.5f;

    // temp:
    private bool movingToFront;
    private bool movingAway;
    private bool movingBack;
    private Transform goalPoint;

    AudioManager audioManager;
    CombatManager combatManager;

    void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        combatManager = GameObject.FindGameObjectWithTag("CombatManager").GetComponent<CombatManager>();
    }
    void Start()
    {
        frustrationLevel = 0;
        UpdateFrustration(enemyData.startingFrustration);
        dialogueBox.SetActive(false);
        actionTelegraph.SetActive(false);
    }

    void Update()
    {
        if (movingToFront)
        {
            transform.position = Vector2.MoveTowards(transform.position, goalPoint.position,
                enemyData.moveSpeed * Time.deltaTime);
            if (transform.position.x >= goalPoint.position.x)
            {
                // Customer hits front of line
                if (enemyData.passiveAction != null) TakeEnemyAction(enemyData.passiveAction);
                movingToFront = false;
                SayDialogueLine(LineType.OPENING);
                audioManager.PlayDialogue(openingSound);
                SetNewPreppedAction();
                combatManager.SpawnPaperwork();
                combatManager.EnableActions();
            }
        }
        if (movingAway)
        {
            transform.position = Vector2.MoveTowards(transform.position, goalPoint.position,
                enemyData.moveSpeed * Time.deltaTime);
            if (transform.position.x >= goalPoint.position.x)
            {
                movingAway = false;
                Destroy(gameObject);
            }
        }
        if (movingBack)
        {
            transform.position = Vector2.MoveTowards(transform.position, goalPoint.position,
                enemyData.moveSpeed * Time.deltaTime);
            if (transform.position.x <= goalPoint.position.x)
            {
                movingBack = false;
                dialogueBox.SetActive(false);
            }
        }
    }

    public void SendToFront(Transform point)
    {
        if (gameObject == null) return;
        Debug.Log("Sending customer to front");
        movingToFront = true;
        goalPoint = point;
    }
    public void Interupt(string action)
    {
        if (enemyData.passiveAction != null && enemyData.passiveAction.enemyActionName == action)
            TakeEnemyAction(enemyData.passiveAction);
    }

    public void SendAway(bool accepted, Transform point)
    {
        if (gameObject == null) return;

        if (accepted) audioManager.PlayDialogue(happySound);
        else audioManager.PlayDialogue(angrySound);

        Debug.Log("Sending customer away");
        movingAway = true;
        goalPoint = point;

        // No difference in sprites currently, can uncomment if that changes
        // if (accepted)
        // {
        //     if (enemyData.acceptedSprite != null)
        //         gameObject.GetComponent<SpriteRenderer>().sprite = enemyData.acceptedSprite;
        // }
        // else
        // {
        //     if (enemyData.rejectedSprite != null)
        //         gameObject.GetComponent<SpriteRenderer>().sprite = enemyData.rejectedSprite;
        // }
    }

    public void SendToBack(Transform point)
    {
        if (gameObject == null) return;
        Debug.Log("Sending customer to back");
        SayDialogueLine(LineType.NEGATIVE);
        audioManager.PlayDialogue(angrySound);
        movingBack = true;
        goalPoint = point;
        // toogle paperwork visibility false
        GameObject.FindGameObjectWithTag("Paperwork").SetActive(false);
    }

    public void UpdateFrustration(float change)
    {
        if (gameObject == null) return;

        frustrationLevel += change;
        Debug.Log("Customer frustration level updated to: " + frustrationLevel);
        if (frustrationMeter != null) frustrationMeter.UpdateBar(frustrationLevel, enemyData.maxFrustration);
        if (frustrationLevel >= enemyData.maxFrustration)
        {
            audioManager.PlayDialogue(angrySound);
            AddNewEnemyEffect(irateEffect, 1);
        }
        if (frustrationLevel <= 0)
        {
            audioManager.PlayDialogue(happySound);
            AddNewEnemyEffect(elatedEffect, 1);
        }
    }

    public Dictionary<EffectType, GameObject> GetActiveEffects()
    {
        return activeEffects;
    }

    public void TakeTurn()
    {
        StartCoroutine(TakeTurnWithUI());
    }

    // take prepped action, wait, update new prepped action
    private IEnumerator TakeTurnWithUI()
    {
        yield return new WaitForSeconds(SHAKE_DURATION); // wait for player action results and SFX
        // Apply turn frustration
        Debug.Log("Adding frustration for waiting");
        UpdateFrustration(enemyData.frustrationIncreasePerTurn);

        Debug.Log("Customer Turn Starting");
        // Take prepped action
        if (preppedAction != null) StartCoroutine(TakeActionWithUI(preppedAction));
        yield return new WaitForSeconds(SHAKE_DURATION); // Shake duration
        yield return new WaitForSeconds(DIALOGUE_DURATION); // dialogue duration
        yield return new WaitForSeconds(DIALOGUE_DURATION); // action text duration

        // Telegraph next action
        SetNewPreppedAction();

        Debug.Log("Customer Turn Completed");
        combatManager.EnableActions();
    }

    // shake enemy, wait, show dialogue, wait, apply action effects
    private IEnumerator TakeActionWithUI(EnemyAction preppedAction)
    {
        // Enemy taking action shake
        dialogueBox.SetActive(false);
        actionTelegraph.GetComponent<UIShake>().StartShake();
        yield return new WaitForSeconds(SHAKE_DURATION); // Shake duration

        // Add dialogue
        actionTelegraph.SetActive(false);
        dialogueBox.SetActive(true);

        SayDialogueLine(LineType.NEUTRAL);
        yield return new WaitForSeconds(DIALOGUE_DURATION); // dialogue duration

        // Add action result text
        StartCoroutine(TypeLine(preppedAction.GetDescription()));
        yield return new WaitForSeconds(DIALOGUE_DURATION); // dialogue duration

        // Apply effect results
        TakeEnemyAction(preppedAction);
    }

    IEnumerator TypeLine(string text) {
        dialogueText.text = "";
        foreach (char letter in text) {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void SayDialogueLine(LineType lineType)
    {
        var dialogueChoices = lineType switch
        {
            LineType.OPENING => enemyData.openingDialogueLines,
            LineType.NEUTRAL => enemyData.neutralDialogueLines,
            LineType.NEGATIVE => enemyData.negativeDialogueLines,
            LineType.POSITIVE => enemyData.positiveDialogueLines,
            _ => enemyData.neutralDialogueLines
        };
        if (dialogueChoices.Length > 0)
        {
            dialogueBox.SetActive(true);
            StartCoroutine(TypeLine(dialogueChoices[UnityEngine.Random.Range(0, dialogueChoices.Length)]));
        }
    }

    public void AddEnemyData(EnemyData data)
    {
        enemyData = data;
        gameObject.GetComponent<SpriteRenderer>().sprite = enemyData.acceptedSprite;
    }

    public float GetPaperworkOdds()
    {
        return enemyData.correctPaperworkOdds;
    }

    /* Add New Enemy Effect
    * ~~~~~~~~~~~~~~
    * add to UI and activeEffects tracker
    */
    public void AddNewEnemyEffect(ActionEffect effect, int stacks)
    {
        // Add to active/displayed buffs/debuffs
        if (activeEffects.ContainsKey(effect.type))
        {
            UIEffectController currUIEffect = activeEffects[effect.type].GetComponent<UIEffectController>();
            if (effect.shouldStack)
            {
                Debug.Log("Effect already active, add to stack");
                currUIEffect.UpdateTurns(stacks);
                activeEffects[effect.type].GetComponent<CustomerEffect_MouseOverDescription>().UpdateDescription(effect.effectDescription + "\nTurns: " + currUIEffect.FetchTurns(), effect.effectName);
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
            effectMarker.GetComponent<CustomerEffect_MouseOverDescription>().UpdateDescription(effect.effectDescription + "\nTurns: " + stacks, effect.effectName);
        }
    }

    public void IncrementActiveEffects()
    {
        foreach (var (type, effectUI) in activeEffects)
        {
            ActionEffect effect = effectUI.GetComponent<UIEffectController>().effect;
            if (effect.shouldDecay) RemoveEffectStacks(1, effect.type);
        }
    }

    /* Remove Effect Stacks:
    * ~~~~~~~~~~~~~~~~~~~~~~~~~
    * Remove stacks from effect if active
    */
    private void RemoveEffectStacks(int amount, EffectType effectType)
    {
        if (activeEffects.ContainsKey(effectType))
        {
            Debug.Log("Incrementing customer effect");
            UIEffectController effectUI = activeEffects[effectType].GetComponent<UIEffectController>();
            effectUI.UpdateTurns(-amount);
            activeEffects[effectType].GetComponent<CustomerEffect_MouseOverDescription>().UpdateDescription(
            effectUI.effect.effectDescription + "\nTurns: " + effectUI.FetchTurns(), effectUI.effect.effectName);
            if (effectUI.FetchTurns() == 0)
            {
                Debug.Log("Remove effect from UI");
                Destroy(activeEffects[effectType]);
                activeEffects.Remove(effectType);
            }
        }
    }

    private void SetNewPreppedAction()
    {
        // Set new action based on enemy state
        if (frustrationLevel < 10) preppedAction = enemyData.positiveAction;
        else if (frustrationLevel > 90) preppedAction = enemyData.negativeAction;
        else preppedAction = enemyData.neutralAction;
        if (preppedAction != null)
        {
            actionTelegraph.SetActive(true);
            // TODO: should it be different sprite for diff action type?
            actionTelegraph.GetComponent<MouseOverDescription>().UpdateDescription("Next turn enemy will use " + preppedAction.enemyActionName);
        }
    }

    private void TakeEnemyAction(EnemyAction action)
    {
        Debug.Log("Taking Enemy Action: " + action.enemyActionName);
        combatManager.UpdatePerformance(action.PERFORMANCE_MODIFIER);
        combatManager.UpdateWill(action.WILL_MODIFIER);
        combatManager.UpdateAttention(action.ATTENTION_MODIFIER);

        // Apply new effects for next turn
        List<EffectType> cleanupEffects = new List<EffectType>();
        foreach (ActionEffectStacks effectStacks in action.effects)
        {
            // If marked as negative, remove those stacks
            if (effectStacks.stacks < 0)
            {
                bool shouldCleanup = combatManager.RemoveEffectStacks(-effectStacks.stacks, effectStacks.effect.type);
                if (shouldCleanup) cleanupEffects.Add(effectStacks.effect.type);
            }
            // Else add new stacks
            else 
            {
                combatManager.AddNewEffect(effectStacks.effect, effectStacks.stacks);
            }
        }
        foreach (EffectType effect in cleanupEffects) combatManager.DeleteEffect(effect);

        // TODO: show somehow in dialogue box action was taken?
        actionTelegraph.SetActive(false);
    }
}
