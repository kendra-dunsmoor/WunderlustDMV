using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class Character : ScriptableObject
{
    public string characterName;
    public Sprite characterImage;
    public float typingSpeed;
    public AudioClip[] sounds;
}
