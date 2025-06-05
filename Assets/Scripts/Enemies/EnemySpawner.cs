using UnityEngine;
using System.Collections.Generic;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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
            Customer customer = Instantiate(customerPrefab, spawnPoint).GetComponent<Customer>();
            customer.AddEnemyData(enemyTypes[0]); // just base one for testing rn
            customersInLine.Enqueue(customer);
            customerIconQueue.Enqueue(Instantiate(customerIconPrefab, customerQueuePanel)); // temp, this only works while there are less customers than the size of the panel
        }
        // TODO: add random selection based on available enemy types
    }
}
