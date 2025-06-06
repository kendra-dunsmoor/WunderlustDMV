using UnityEngine;
using System.Collections.Generic;
using static ActionEffect;
using static EnemyData;
using TMPro;

// Manage Frustration Bar, effects, and movement/reactions
public class Customer : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private FloatingHealthBar frustrationMeter;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("------------- Effects -------------")]
    [SerializeField] private GameObject currentEffectPrefab;
    [SerializeField] private Transform currentEffectsPanel;
    [SerializeField] private Dictionary<EffectType, GameObject> activeEffects = new Dictionary<EffectType, GameObject>();
    [SerializeField] private ActionEffect irateEffect;

    private float frustrationLevel;

    // temp:
    private bool movingToFront;
    private bool movingAway;
    private bool movingBack;
    private Transform goalPoint;

    CombatManager combatManager;

    void Start()
    {
        frustrationLevel = 0;
        UpdateFrustration(enemyData.startingFrustration);
        combatManager = GameObject.FindGameObjectWithTag("CombatManager").GetComponent<CombatManager>();
        dialogueBox.SetActive(false);
    }

    void Update()
    {
        if (movingToFront)
        {
            transform.position = Vector2.MoveTowards(transform.position, goalPoint.position,
                enemyData.moveSpeed * Time.deltaTime);
            if (transform.position.x >= goalPoint.position.x)
            {
                movingToFront = false;
                SayDialogueLine(LineType.OPENING);
                combatManager.EnableActions();
                combatManager.SpawnPaperwork();
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

    public void SendAway(bool accepted, Transform point)
    {
        if (gameObject == null) return;

        Debug.Log("Sending customer away");
        SayDialogueLine(LineType.POSITIVE);
        movingAway = true;
        goalPoint = point;
        // toogle paperwork visibility false
        GameObject paperwork = GameObject.FindGameObjectWithTag("Paperwork");
        if (paperwork != null) paperwork.SetActive(false);
        if (accepted)
        {
            if (enemyData.acceptedSprite != null)
                gameObject.GetComponent<SpriteRenderer>().sprite = enemyData.acceptedSprite;
        }
        else
        {
            if (enemyData.acceptedSprite != null)
                gameObject.GetComponent<SpriteRenderer>().sprite = enemyData.rejectedSprite;
        }
    }

    public void SendToBack(Transform point)
    {
        if (gameObject == null) return;
        Debug.Log("Sending customer to back");
        SayDialogueLine(LineType.NEGATIVE);
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
            AddNewEnemyEffect(irateEffect, 1);
        }
    }

    public Dictionary<EffectType, GameObject> GetActiveEffects()
    {
        return activeEffects;
    }

    public void TakeTurn()
    {
        Debug.Log("Customer Turn Starting");
        // Apply turn frustration
        Debug.Log("Adding frustration for waiting");
        UpdateFrustration(enemyData.frustrationIncreasePerTurn);
        // Take action

        // Add dialogue
        SayDialogueLine(LineType.NEUTRAL);
        Debug.Log("Customer Turn Completed");
        combatManager.EnableActions();
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
            dialogueText.text = dialogueChoices[Random.Range(0, dialogueChoices.Length)];
        }
    }

    public void AddEnemyData(EnemyData data)
    {
        enemyData = data;
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
                activeEffects[effect.type].GetComponent<MouseOverDescription>().UpdateDescription(effect.effectDescription + "\nTurns: " + currUIEffect.FetchTurns(), effect.effectName);
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
            effectMarker.GetComponent<MouseOverDescription>().UpdateDescription(effect.effectDescription + "\nTurns: " + stacks, effect.effectName);
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
    private void RemoveEffectStacks(int amount, EffectType effectType) {
        if (activeEffects.ContainsKey(effectType)) {
            Debug.Log("Incrementing customer effect");
            UIEffectController effectUI = activeEffects[effectType].GetComponent<UIEffectController>();
            effectUI.UpdateTurns(-amount);
            activeEffects[effectType].GetComponent<MouseOverDescription>().UpdateDescription(
            effectUI.effect.effectDescription + "\nTurns: " + effectUI.FetchTurns(), effectUI.effect.effectName);
            if (effectUI.FetchTurns() == 0) {
                Debug.Log("Remove effect from UI");
                Destroy(activeEffects[effectType]);
                activeEffects.Remove(effectType);
            }
        }
    }
}
