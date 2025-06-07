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

    // TODO: add value for conditions and for how many turns the condition lasts

    // TODO: add a function that accepts unique actions
    
    public string GetDescription()
    {
        string description = generalDescription;
        if (WILL_MODIFIER > 0) description += "Gain " + WILL_MODIFIER + "Will";
        if (WILL_MODIFIER < 0) description += "Lose " + WILL_MODIFIER + "Will";
        if (FRUSTRATION_MODIFIER > 0) description += "Gain " + FRUSTRATION_MODIFIER + "Frustration";
        if (FRUSTRATION_MODIFIER < 0) description += "Lose " + FRUSTRATION_MODIFIER + "Frustration";
        if (PERFORMANCE_MODIFIER > 0) description += "Gain " + PERFORMANCE_MODIFIER + "Performance";
        if (PERFORMANCE_MODIFIER < 0) description += "Lose " + PERFORMANCE_MODIFIER + "Performance";
        if (ATTENTION_MODIFIER > 0) description += "Gain " + ATTENTION_MODIFIER + "% Attention";
        if (ATTENTION_MODIFIER < 0) description += "Lose " + ATTENTION_MODIFIER + "% Attention";
        // foreach (ActionEffectStacks effectStacks in effects)
        // {
        //     if (effectStacks != null) description += " Adds effect " + effectStacks.effect.type + " for " + effectStacks.stacks + " turns.";
        // }
        return description;
    }
}