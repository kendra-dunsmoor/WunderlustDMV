using UnityEngine;
using System.Collections.Generic;
using static ActionEffect;
using static EnemyData;
using TMPro;
using static BossCombatManager;

// Manage Frustration Bar, effects, and movement/reactions
public class Boss : MonoBehaviour
{
    [SerializeField] private BossData bossData;
    [SerializeField] private BossCombatManager bossCombatManager;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [Header("------------- Enemy Actions -------------")]
    [SerializeField] private GameObject actionTelegraph;
    private BossAction preppedAction;

    [Header("------------- Effects -------------")]
    [SerializeField] private GameObject currentEffectPrefab;
    [SerializeField] private Transform currentEffectsPanel;
    [SerializeField] private Dictionary<EffectType, GameObject> activeEffects = new Dictionary<EffectType, GameObject>();

    void Start()
    {
        dialogueBox.SetActive(false);
        actionTelegraph.SetActive(false);
    }

    public Dictionary<EffectType, GameObject> GetActiveEffects()
    {
        return activeEffects;
    }

    public void TakeTurn(BossState state)
    {
        Debug.Log("Boss Turn Starting");
        // Take prepped action
        if (preppedAction != null) TakeBossAction(preppedAction);

        // Telegraph next action
        SetNewPreppedAction(state);

        Debug.Log("Boss Turn Completed");
    }

    public void SayDialogueLine(LineType lineType)
    {
        var dialogueChoices = lineType switch
        {
            LineType.OPENING => bossData.openingDialogueLines,
            LineType.NEUTRAL => bossData.neutralDialogueLines,
            LineType.NEGATIVE => bossData.negativeDialogueLines,
            LineType.POSITIVE => bossData.positiveDialogueLines,
            _ => bossData.neutralDialogueLines
        };
        if (dialogueChoices.Length > 0)
        {
            dialogueBox.SetActive(true);
            dialogueText.text = dialogueChoices[Random.Range(0, dialogueChoices.Length)];
            // TODO: Make this type out like normal dialogue
        }
    }

    public void AddEnemyData(BossData data)
    {
        bossData = data;
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
    private void RemoveEffectStacks(int amount, EffectType effectType)
    {
        if (activeEffects.ContainsKey(effectType))
        {
            Debug.Log("Incrementing customer effect");
            UIEffectController effectUI = activeEffects[effectType].GetComponent<UIEffectController>();
            effectUI.UpdateTurns(-amount);
            activeEffects[effectType].GetComponent<MouseOverDescription>().UpdateDescription(
            effectUI.effect.effectDescription + "\nTurns: " + effectUI.FetchTurns(), effectUI.effect.effectName);
            if (effectUI.FetchTurns() == 0)
            {
                Debug.Log("Remove effect from UI");
                Destroy(activeEffects[effectType]);
                activeEffects.Remove(effectType);
            }
        }
    }

    public void SetNewPreppedAction(BossState currentBossState)
    {
        // Set new action based on enemy state
        if (currentBossState == BossState.Neutral) preppedAction = bossData.neutralPhaseActions[0];
        if (currentBossState == BossState.Angry) preppedAction = bossData.angryPhaseActions[0];
        if (currentBossState == BossState.Pacified) preppedAction = bossData.pacifiedPhaseActions[0];
        if (currentBossState == BossState.Transition) preppedAction = null;

        if (preppedAction != null)
        {
            actionTelegraph.SetActive(true);
            // TODO: should it be different sprite for diff action type?
            actionTelegraph.GetComponent<MouseOverDescription>().UpdateDescription("Next turn enemy will use " + preppedAction.BossActionName);
        }
    }

    private void TakeBossAction(BossAction action)
    {
        // Apply will cost for boss
        bossCombatManager.UpdateBossWill(action.BOSS_WILL_MODIFIER);

        Debug.Log("Taking Enemy Action: " + action.BossActionName);
        bossCombatManager.UpdatePerformance(action.PERFORMANCE_MODIFIER);
        bossCombatManager.UpdateWill(action.WILL_MODIFIER);

        // TODO: show somehow in dialogue box action was taken?
        actionTelegraph.SetActive(false);
    }

    public float GetStartingWill()
    {
        return bossData.startingWill;
    }

    public float GetMaxWill()
    {
        return bossData.maxWill;
    }
}
