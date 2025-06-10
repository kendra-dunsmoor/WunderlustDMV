using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

/* Enemy Spawner
* ~~~~~~~~~~~~~~~~
* Responsible for determining what enemy types/queue composition should look like in combat
*/
public class EnemySpawner : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField, Tooltip("All available enemy types to select from")] private EnemyData[] enemyTypes;
    [SerializeField, Tooltip("Customer GameObject to plug enemy type data into")] private GameObject customerPrefab;
    [SerializeField, Tooltip("Customer icon for queue visibility")] private GameObject customerIconPrefab;
    [SerializeField, Tooltip("Location to spawn enemies")] private Transform spawnPoint;
    [SerializeField, Tooltip("Location for customer queue icons")] private Transform customerQueuePanel;
    private System.Random randomGen = new System.Random();

    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    /* Initialize Customer Queue: 
    * ~~~~~~~~~~~~~~~~~~~~~~~~~~~
    * Spawn enemies and add to queue for combat tracking
    */
    public void SpawnEnemies(int numCustomers, out Queue<Customer> customersInLine, out Queue<GameObject> customerIconQueue)
    {
        customersInLine = new Queue<Customer>();
        customerIconQueue = new Queue<GameObject>();
        Debug.Log("Initialize customer queue: " + numCustomers);
        for (int i = 0; i < numCustomers; i++)
        {
            EnemyData spawnedEnemy = GetRandomEnemy();
            Customer customer = Instantiate(customerPrefab, spawnPoint).GetComponent<Customer>();
            if (gameManager.InTutorial()) customer.AddEnemyData(enemyTypes[0]); // just base one for testing rn
            else customer.AddEnemyData(spawnedEnemy);
            customersInLine.Enqueue(customer);
            GameObject customerIcon = Instantiate(customerIconPrefab, customerQueuePanel);
                    // Get all components of type T in children
            customerIcon.GetComponentInChildren<Image>().sprite = spawnedEnemy.iconSprite;
            customerIconQueue.Enqueue(customerIcon); // temp, this only works while there are less customers than the size of the panel
        }
    }
    
    private EnemyData GetRandomEnemy()
	{
		// Get total drop chance
        float totalChance = 0f;
        for (int i = 0; i < enemyTypes.Length; i++)
        {
            totalChance += GetEnemySpawnRate(enemyTypes[i].enemyType);
        }
        float rand = (float) randomGen.NextDouble() * totalChance;
		float cumulativeChance = 0f;
		for (int i = 0; i < enemyTypes.Length; i++)
		{
			cumulativeChance += GetEnemySpawnRate(enemyTypes[i].enemyType);
			if (rand <= cumulativeChance)
			{
                return enemyTypes[i];
			}
		}
        return enemyTypes[0];
    }

    private float GetEnemySpawnRate(EnemyData.EnemyType enemyType)
    {
        return enemyType switch
        {
            EnemyData.EnemyType.BASIC => 0.5f,
            EnemyData.EnemyType.SPECIAL1 => 0.15f,            
            EnemyData.EnemyType.SPECIAL2 => 0.1f,
            EnemyData.EnemyType.SPECIAL3 => 0.1f,
            EnemyData.EnemyType.ELITE => 0.15f,
            _ => 1f
        };
    }
}
