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
        ELITE,
        BOSS
    }
    public EnemyType enemyType;
    public float moveSpeed;

    [Header("------------- Frustration -------------")]
    public float startingFrustration;
    public float frustrationIncreasePerTurn;
    public float maxFrustration;

    [Header("------------- Actions -------------")]

    public EnemyAction[] availableActions;

    [Header("------------- Dialogue -------------")]
    public string[] openingDialogueLines;
    public string[] neutralDialogueLines; // mostly just use when they have a turn
    public string[] positiveDialogueLines;
    public string[] negativeDialogueLines;

    public enum LineType
    {
        OPENING,
        NEUTRAL,
        POSITIVE,
        NEGATIVE
    }

    [Header("------------- UI -------------")]    
    public Sprite acceptedSprite;
    public Sprite rejectedSprite;
    public Sprite iconSprite;
}
