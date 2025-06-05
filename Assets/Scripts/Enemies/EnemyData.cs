using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public enum EnemyType
    {
        BASIC,
        SPECIAL,
        BOSS
    }
    public EnemyType enemyType;
    public float moveSpeed;
    public float maxFrustration;
    public Sprite acceptedSprite;
    public Sprite rejectedSprite;
    public Sprite iconSprite;
}
