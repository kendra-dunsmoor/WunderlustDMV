using UnityEngine;

public class Artifact: ScriptableObject
{
    public string artifactName;
    public Sprite sprite;
    public int index;
    public int price;
    // how often should ability trigger
    public int turnClock;
    // counter for how many turns since last trigger 
    public int currentTurnCounter;
    public enum Rarity {
        COMMON,
        UNCOMMON,
        RARE,
        STARTER,
        QUEST
    }
    public Rarity rarity;
    public string description;
    // is there a way to track what their effects are besides manually hard-coding each? They all effect different meters
}
