using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAction", menuName = "Scriptable Objects/EnemyAction")]
public class EnemyAction : ScriptableObject
{
    public string enemyActionName;
    public string generalDescription;

    [Header("------------- Action Effects -------------")]
    public float FRUSTRATION_MODIFIER;
    public float WILL_MODIFIER;
    public float PERFORMANCE_MODIFIER;
    public float ATTENTION_MODIFIER;
    public enum EnemyActionType
    {
        PASSIVE,
        NEGATIVE,
        POSTIVE,
        NEUTRAL
    }

    // TODO: add accuracy check:
    public float ACTION_CHANCE = 1.0f; // The chance of the action happening, 1.0f = 100%

    public List<ActionEffectStacks> effects;


    // TODO: add a function that accepts unique actions
    
    public string GetDescription()
    {
        string description = "Enemy used " + enemyActionName;
        if (generalDescription != "" || generalDescription != null) description += "\n" + generalDescription;
        if (WILL_MODIFIER > 0) description += "\nGained " + WILL_MODIFIER + " Will";
        if (WILL_MODIFIER < 0) description += "\nLost " + -WILL_MODIFIER + " Will";
        if (FRUSTRATION_MODIFIER > 0) description += "\nGained " + FRUSTRATION_MODIFIER + " Frustration";
        if (FRUSTRATION_MODIFIER < 0) description += "\nLost " + -FRUSTRATION_MODIFIER + " Frustration";
        if (PERFORMANCE_MODIFIER > 0) description += "\nGained " + PERFORMANCE_MODIFIER + " Performance";
        if (PERFORMANCE_MODIFIER < 0) description += "\nLost " + -PERFORMANCE_MODIFIER + " Performance";
        if (ATTENTION_MODIFIER > 0) description += "\nGained " + ATTENTION_MODIFIER + "% Attention";
        if (ATTENTION_MODIFIER < 0) description += "\nLost " + -ATTENTION_MODIFIER + "% Attention";
        foreach (ActionEffectStacks effectStacks in effects)
        {
            // TODO: Clearer descriptions for particular actions
            if (effectStacks != null)
            {
                if (effectStacks.effect.type == ActionEffect.EffectType.ADD_TURNS) description += "\nSkips a turn.";
                else if (effectStacks.effect.type == ActionEffect.EffectType.MADE_MISTAKE) description += "\nEach Mistake draws more Attention.";
                else description += "\nAdded effect \"" + effectStacks.effect.effectName + "\" for " + effectStacks.stacks + " turns.";
            }
        }
        return description;
    }
}