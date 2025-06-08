using UnityEngine;

[CreateAssetMenu(fileName = "Furniture", menuName = "Scriptable Objects/Furniture")]
public class Furniture : ScriptableObject
{
    public enum FurnitureType {
        CHAIR,
        TABLE,
        DESK,
        DESK2,
        CHAIR2,
        BED
    };
    public FurnitureType type;
    [TextArea(3,10)]
    public string description;
    public string title;
    public int price;
}
