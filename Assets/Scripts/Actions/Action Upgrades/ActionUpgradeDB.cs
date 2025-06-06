using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class ActionUpgradeDB : ScriptableObject
{
	[SerializeField] ActionUpgrade[] upgrades;

	public List<ActionUpgrade> GetRandomUpgrades(int numUpgrades)
	{
		List<ActionUpgrade> toReturn = new List<ActionUpgrade>();
		for (int i = 0; i < numUpgrades; i++)
		{
			toReturn.Add(GetRandomUpgrade());
		}
		return toReturn;
	}

	private ActionUpgrade GetRandomUpgrade() {
				// Get total drop chance
		float totalChance = 0f;
		for (int i = 0; i < upgrades.Length; i++)
		{
			totalChance += GetUpgradeSpawnRate(upgrades[i].rarity);
		}
		float rand = Random.Range(0f, totalChance);
		float cumulativeChance = 0f;
		for (int i = 0; i < upgrades.Length; i++)
		{
			cumulativeChance += GetUpgradeSpawnRate(upgrades[i].rarity);
			if (rand <= cumulativeChance)
			{
				return upgrades[i];
			}
		}
		return upgrades[0];
	}

#if UNITY_EDITOR
	private void OnValidate()
	{
		LoadItems();
	}

	private void LoadItems()
	{
		upgrades = FindAssetsByType<ActionUpgrade>("Assets/Scripts/Actions/Action Upgrades");
	}
	public static T[] FindAssetsByType<T>(params string[] folders) where T : Object
	{
		string type = typeof(T).Name;

		string[] guids;
		if (folders == null || folders.Length == 0)
		{
			guids = AssetDatabase.FindAssets("t:" + type);
		}
		else
		{
			guids = AssetDatabase.FindAssets("t:" + type, folders);
		}

		T[] assets = new T[guids.Length];

		for (int i = 0; i < guids.Length; i++)
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
			assets[i] = AssetDatabase.LoadAssetAtPath<T>(assetPath);
		}
		return assets;
	}
	#endif
	
	private float GetUpgradeSpawnRate(Item.Rarity rarity)
	{
		return rarity switch
		{
			Item.Rarity.COMMON => 0.5f,
			Item.Rarity.UNCOMMON => 0.35f,
			Item.Rarity.RARE => 0.15f,
			_ => 1f
		};
	}
}
