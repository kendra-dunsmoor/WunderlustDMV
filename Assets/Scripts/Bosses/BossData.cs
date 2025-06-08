using UnityEngine;

[CreateAssetMenu(fileName = "BossData", menuName = "Scriptable Objects/BossData")]
public class BossData : ScriptableObject
{
    [Header("------------- General -------------")]
    public string BossName;
    public string BossDescription;

    [Header("------------- Will -------------")]
    public float startingWill;
    public float willIncreasePerTurn; // unused for now
    public float maxWill;
    public float willThreshold; // for vulnerabilities to trigger

    [Header("------------- Actions -------------")]
    public BossAction[] turnActions; // total list of actions boss is capable of
    public BossAction[] passiveActions; // total list of passives boss is capable of

    [Header("------------- Dialogue -------------")]
    public string[] openingDialogueLines;
    public string[] encounterDialogueLines; // lines needed for throughout the encounter

    public enum LineType
    {
        OPENING,
        ENCOUNTER,
    }

    [Header("------------- UI -------------")]
    public Sprite[] bossSprites; // some bosses will use more than one sprite
    [Header("------------- New Player Actions -------------")]
    public Action[] newPlayerActions; // player gets new actions that replace [Accept, Reject, Escalte]
}
