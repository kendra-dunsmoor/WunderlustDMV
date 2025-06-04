using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite Icon;
    public string ID;
    public int price;
    public enum Rarity {
        STARTER,
        COMMON,
        UNCOMMON,
        RARE
    }

    public Rarity itemRarity;
    public string flavorText;
    public string description;
    public List<ActionEffectStacks> effects;
    public int willModifier;
    public int performanceModifier;
    public int frustrationModifier;
    public virtual Item GetCopy()
	{
		return this;
	}
}
