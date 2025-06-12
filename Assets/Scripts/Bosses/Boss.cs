using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using static ActionEffect;
using static EnemyData;
using static BossCombatManager;
using System;

// Manage Frustration Bar, effects, and movement/reactions
public class Boss : MonoBehaviour
{
    [SerializeField] private BossData bossData;
    [SerializeField] private BossCombatManager bossCombatManager;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject specialButtonsParent;

    [Header("------------- Enemy Actions -------------")]
    [SerializeField] private GameObject actionTelegraph;
    private BossAction preppedAction;

    [Header("------------- Effects -------------")]
    [SerializeField] private GameObject currentEffectPrefab;
    [SerializeField] private Transform currentEffectsPanel;
    [SerializeField] private Dictionary<EffectType, GameObject> activeEffects = new Dictionary<EffectType, GameObject>();

    private float SHAKE_DURATION = 1f;
    private float DIALOGUE_DURATION = 3f;

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
        StartCoroutine(TakeTurnWithUI(state));
    }

    // take prepped action, wait, update new prepped action
    private IEnumerator TakeTurnWithUI(BossState state)
    {
        yield return new WaitForSeconds(SHAKE_DURATION); // wait for player action results and SFX

        Debug.Log("Boss Turn Starting");

        // Take prepped action
        if (preppedAction != null) StartCoroutine(TakeBossActionWithUI(preppedAction));
        yield return new WaitForSeconds(SHAKE_DURATION); // Shake duration
        yield return new WaitForSeconds(DIALOGUE_DURATION); // dialogue duration
        yield return new WaitForSeconds(DIALOGUE_DURATION); // action text duration

        // //
        // for (int i = 0; i < loops; i++)
        // {
        //     Debug.Log("Loop" + i);
        //     // Take prepped action
        //     if (preppedAction != null) TakeBossAction(preppedAction);
        //       SetNewPreppedAction(state); 

        // }

        // Telegraph next action
        SetNewPreppedAction(state); 

        Debug.Log("Boss Turn Completed");
    }

    // shake enemy, wait, show dialogue, wait, apply action effects
    private IEnumerator TakeBossActionWithUI(BossAction preppedAction)
    {
        // Enemy taking action shake
        dialogueBox.SetActive(false);
        actionTelegraph.GetComponent<UIShake>().StartShake();
        yield return new WaitForSeconds(SHAKE_DURATION); // Shake duration

        // Add dialogue
        actionTelegraph.SetActive(false);
        dialogueBox.SetActive(true);
        SayDialogueLine(SelectDialogueChoice(LineType.ACTION));
        yield return new WaitForSeconds(DIALOGUE_DURATION); // dialogue duration

        // Apply effect results
        TakeBossAction(preppedAction);

        // Add action result text
        dialogueBox.SetActive(true);
        if (bossCombatManager.earlyShiftPenalty() > 0)
        {
            StartCoroutine(TypeLine(preppedAction.GetDescription() + "\nMultiplied by " + bossCombatManager.earlyShiftPenalty() + " early shift penalties"));
        }
        else StartCoroutine(TypeLine(preppedAction.GetDescription()));
        yield return new WaitForSeconds(DIALOGUE_DURATION); // dialogue duration
        bossCombatManager.EnableActions(specialButtonsParent);
    }

    IEnumerator TypeLine(string text)
    {
        dialogueText.text = "";
        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.01f);
        }
    }

    public string SelectDialogueChoice(LineType lineType)
    {
        Debug.Log("Selecting dialogue line");
        var dialogueChoices = lineType switch
        {
            LineType.OPENING => bossData.openingDialogueLines,
            LineType.TRANSITION => bossData.transitionDialogueLines,
            LineType.ACTION => bossData.actionDialogueLines,
            _ => bossData.actionDialogueLines
        };
        if (dialogueChoices.Length > 0)
        {
            return dialogueChoices[UnityEngine.Random.Range(0, dialogueChoices.Length)];
        }
        return "";     
    }

    public string SelectStateDialogue(BossState state, bool isExiting)
    {
        Debug.Log("Selecting state transition dialogue line");
        var dialogueChoices = state switch
        {
            BossState.Neutral => bossData.neutralDialogueLines,
            BossState.Angry => bossData.negativeDialogueLines,
            BossState.Pacified => bossData.positiveDialogueLines,
            _ => bossData.neutralDialogueLines
        };
        if (isExiting) return dialogueChoices[1];
        else return dialogueChoices[0];
    }

    public void SayDialogueLine(string line)
    {
        dialogueBox.SetActive(true); 
        StartCoroutine(TypeLine(line));
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
        if (currentBossState == BossState.Transition) preppedAction = bossData.pacifiedPhaseActions[0];

        if (preppedAction != null)
        {
            actionTelegraph.SetActive(true);
            // TODO: should it be different sprite for diff action type?
            actionTelegraph.GetComponent<MouseOverDescription>().UpdateDescription("Next turn enemy will use " + preppedAction.BossActionName);
        }
    }

    private void TakeBossAction(BossAction action)
    {
        Debug.Log("Taking Enemy Action: " + action.BossActionName);
        int earlyShiftsPenalty = bossCombatManager.earlyShiftPenalty() + 1;
        // Apply will cost for boss
        bossCombatManager.UpdateBossWill(action.BOSS_WILL_MODIFIER * earlyShiftsPenalty);
        bossCombatManager.UpdatePerformance(action.PERFORMANCE_MODIFIER * earlyShiftsPenalty);
        bossCombatManager.UpdateWill(action.WILL_MODIFIER * earlyShiftsPenalty);
    }

    public float GetStartingWill()
    {
        return bossData.startingWill;
    }

    public float GetMaxWill()
    {
        return bossData.maxWill;
    }

    public void UpdateImage(BossState state)
    {
        gameObject.GetComponent<Image>().sprite = state switch
        {
            BossState.Angry => bossData.angrySprite,
            BossState.Pacified => bossData.happySprite,
            _ => bossData.neutralSprite
        };
    }
}
