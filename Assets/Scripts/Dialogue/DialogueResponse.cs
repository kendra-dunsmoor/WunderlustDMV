using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DialogueResponse
{
    public string responseText;
    public int nextNodeIndex;

    [Header("------------- Rewards -------------")]
    public bool containsReward;
    public int officeBucks;
    public int soulCredits;
    public int chaos;
    public List<Item> itemsRewards;
    public List<ActionEffectStacks> effectRewards;
    public float performanceBoost;
    public float willBoost;
    public string specialRewardMessage; // i.e. anything I don't feel like coding yet
}