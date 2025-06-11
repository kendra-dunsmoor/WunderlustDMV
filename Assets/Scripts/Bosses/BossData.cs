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
    public BossAction[] neutralPhaseActions; // total list of actions boss is capable of
    public BossAction[] pacifiedPhaseActions; // total list of actions boss is capable of
    public BossAction[] angryPhaseActions; // total list of actions boss is capable of
    public BossAction[] passiveActions; // total list of passives boss is capable of

    [Header("------------- Dialogue -------------")]
    public string[] openingDialogueLines;
    public string[] actionDialogueLines;
    public string[] neutralDialogueLines;
    public string[] positiveDialogueLines;
    public string[] negativeDialogueLines;
    public string[] transitionDialogueLines;

    [Header("------------- UI -------------")]
    public Sprite neutralSprite;
    public Sprite angrySprite;
    public Sprite happySprite;

}
