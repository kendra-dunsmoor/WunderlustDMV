using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
* Combat Manager
* ~~~~~~~~~~~~~~~
* Initialize and manager queue of customers, meters, and goals/progress
*/
public class CombatManager : MonoBehaviour
{
    private AudioManager audioManager;
    private GameManager gameManager;

    // Prefabs
    [SerializeField] private GameObject shiftOverMenu;
    [SerializeField] private GameObject customer; // temp will need to have data for diff enemy types
    [SerializeField] private GameObject paperwork; // temp object placeholder

    // Temporary, just sticking meter management here for now
    [SerializeField] private Slider efficiencyMeter;
    [SerializeField] private Slider willMeter;
    [SerializeField] private TextMeshProUGUI customerGoalText;
    [SerializeField] private GameObject[] customerIconQueue;

    // Spawn Points
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform offScreenPoint;
    [SerializeField] private Transform frontOfLinePoint;

    // Combat trackers
    private List<Customer> customersInLine = new List<Customer>();
    private int CUSTOMER_GOAL = 3;
    private int customersRemaining = 3;
    private int currCustomer = 0;

    void Start()
    {
        currCustomer = 0;
        efficiencyMeter.value = 0.5f;
        willMeter.value = 1f;
        InitializeCustomerQueue();
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void Update()
    {
        
    }
    
    // Button Interaction Functions
    public void Accept()
    {
        efficiencyMeter.value += 0.2f;
        willMeter.value -= 0.2f;
        customersInLine[currCustomer].SendAway(true, offScreenPoint);
        NextCustomer();
    }
    
    public void Reject()
    {
        efficiencyMeter.value -= 0.2f;
        willMeter.value -= 0.2f;
        customersInLine[currCustomer].SendToBack(spawnPoint);
        NextCustomer();
    }
    
    public void Escalate()
    {
        efficiencyMeter.value -= 0.2f;
        willMeter.value -= 0.2f;
        customersInLine[currCustomer].SendAway(false, offScreenPoint);
        NextCustomer();
    }

    public void Delay()
    {
        efficiencyMeter.value -= 0.2f;
        willMeter.value -= 0.2f;
        customersInLine[currCustomer].SendAway(false, offScreenPoint);
        NextCustomer();
    }

    private void EndShift()
    {
        // Pop up end screen
        gameManager.ShiftCompleted();
        Instantiate(shiftOverMenu, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
    }

    private void NextCustomer()
    {
        Debug.Log("Next Customer");
        customersRemaining -= 1;
        Debug.Log("Customers remaining: " + customersRemaining);
        customerGoalText.text = "Customers remaining: " + customersRemaining;
        currCustomer++;
        if (customersRemaining == 0) EndShift();
        else {
            customersInLine[currCustomer].SendToFront(frontOfLinePoint);
            customerIconQueue[customersRemaining - 1].SetActive(false); // this won't work if more customers than queue spots
        }
    }
    
    public void SpawnPaperwork()
    {
        // TODO: Paperwork will be unique/randomized and instantiated in, just for looks rn
        paperwork.SetActive(true);
    }

    // TODO: add logic to randomize/select queue of customers for the shift
    private void InitializeCustomerQueue() {
        // Temp: Spawn all customers off screen and add to in game and icon queues
        Debug.Log("initialize queue");
        for (int i = 0; i < CUSTOMER_GOAL; i++) {
            Debug.Log("spawning");
            customersInLine.Add(Instantiate(customer, spawnPoint).GetComponent<Customer>());
            customerIconQueue[i].SetActive(true);
        }
        customerIconQueue[CUSTOMER_GOAL - 1].SetActive(false);
        Debug.Log("Sending to front+ " + frontOfLinePoint);
        customersInLine[0].SendToFront(frontOfLinePoint);
    }
}
