using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("------------- General -------------")]
    public string enemyName;
    public string enemyDescription;
    public enum EnemyType
    {
        BASIC,
        SPECIAL,
        MSPECIAL,
        ELITE,
        BOSS
    }
    public EnemyType enemyType;
    public float moveSpeed;
    public float correctPaperworkOdds;

    [Header("------------- Frustration -------------")]
    public float startingFrustration;
    public float frustrationIncreasePerTurn;
    public float maxFrustration;

    [Header("------------- Dialogue -------------")]
    public string[] openingDialogueLines;
    public string[] neutralDialogueLines;
    public string[] positiveDialogueLines;
    public string[] negativeDialogueLines;

    public enum LineType
    {
        OPENING,
        NEUTRAL,
        POSITIVE,
        NEGATIVE,
        TRANSITION,
        ACTION
    }

    [Header("------------- Actions -------------")]
    public EnemyAction positiveAction;
    public EnemyAction neutralAction;
    public EnemyAction negativeAction;
    public EnemyAction passiveAction;

    [Header("------------- UI -------------")]    
    public Sprite acceptedSprite;
    public Sprite rejectedSprite;
    public Sprite iconSprite;
}
