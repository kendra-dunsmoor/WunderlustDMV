using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class ItemDB : ScriptableObject
{
	[SerializeField] Item[] items;

	private GameManager gameManager;


	public Item GetItemReference(string itemID)
	{
		foreach (Item item in items)
		{
			if (item.ID == itemID)
			{
				return item;
			}
		}
		return null;
	}

	public Item GetItemCopy(string itemID)
	{
		Item item = GetItemReference(itemID);
		return item != null ? item.GetCopy() : null;
	}

	public Item GetItemFromIndex(int index)
	{
		Item item = items[index];
		return item != null ? item.GetCopy() : null;
	}

	public List<Item> GetRandomItems(int numArtifacts, bool shouldBeArtifact)
	{
		// for each item (can do this only while small amount of items like this)
		// check if artifact or consumable
		// check rarity and assign that a spawnRate
		List<Item> itemsToReturn = new List<Item>();
		List<Item> itemsToCheck = new List<Item>();
		foreach (Item item in items)
		{
			if (shouldBeArtifact && item is ArtifactItem && item.itemRarity != Item.Rarity.STARTER)
				itemsToCheck.Add(item);
			else if (!shouldBeArtifact && item is not ArtifactItem && item.itemRarity != Item.Rarity.STARTER)
				itemsToCheck.Add(item);
		}

		for (int i = 0; i < numArtifacts; i++)
		{
			itemsToReturn.Add(GetRandomItemFromList(itemsToCheck));
		}
		return itemsToReturn;
	}

#if UNITY_EDITOR
	private void OnValidate()
	{
		LoadItems();
	}

	private void LoadItems()
	{
		items = FindAssetsByType<Item>("Assets/Scripts/Items/ItemObjects");
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

	private float GetItemSpawnRate(Item.Rarity rarity)
	{

        gameManager = FindFirstObjectByType<GameManager>();
		List<Certificate> playerCerts = gameManager.FetchCertificates();

		
		if (playerCerts.Any(c => c.type == SUPPLY_SOURCING))
		{
			return rarity switch
			{
				Item.Rarity.COMMON => 0.3f,
				Item.Rarity.UNCOMMON => 0.4f,
				Item.Rarity.RARE => 0.3f,
				_ => 1f
			};
		}
		
		return rarity switch
		{
			Item.Rarity.COMMON => 0.5f,
			Item.Rarity.UNCOMMON => 0.35f,
			Item.Rarity.RARE => 0.15f,
			_ => 1f
		};
	}

	private Item GetRandomItemFromList(List<Item> itemsToCheck)
	{
		// Get total drop chance
		float totalChance = 0f;
		for (int i = 0; i < itemsToCheck.Count; i++)
		{
			totalChance += GetItemSpawnRate(itemsToCheck[i].itemRarity);
		}
		float rand = Random.Range(0f, totalChance);
		float cumulativeChance = 0f;
		for (int i = 0; i < itemsToCheck.Count; i++)
		{
			cumulativeChance += GetItemSpawnRate(itemsToCheck[i].itemRarity);
			if (rand <= cumulativeChance)
			{
				return GetItemCopy(itemsToCheck[i].ID);
			}
		}
		return GetItemCopy(itemsToCheck[0].ID);
    }
}
