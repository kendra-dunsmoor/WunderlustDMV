using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class ActionUpgradeDB: ScriptableObject
{
	[SerializeField] ActionUpgrade[] upgrades;

	public ActionUpgrade[] GetRandomUpgrades()
	{
		// TODO: temp just return
		return upgrades;
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
		if (folders == null || folders.Length == 0) {
			guids = AssetDatabase.FindAssets("t:" + type);
		} else {
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
}
