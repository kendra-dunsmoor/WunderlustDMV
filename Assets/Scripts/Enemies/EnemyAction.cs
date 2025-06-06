using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAction", menuName = "Scriptable Objects/EnemyAction")]
public class EnemyAction : ScriptableObject
{
    public string enemyActionName;

    [Header("------------- Action Effects -------------")]
    public float FRUSTRATION_MODIFIER;
    public float WILL_MODIFIER;
    public float PERFORMANCE_MODIFIER;

    public float ATTENTION_MODIFIER;

    public bool PASSIVE = false;

    public float ACTION_CHANCE = 1.0f; // The chance of the action happening, 1.0f = 100%

    public float BOSS_WILL_MODIFIER;
    
    // TODO: add spawn/rarity chance for action?

    // TODO: add value for conditions and for how many turns the condition lasts

    // TODO: add a function that accepts unique actions
}
