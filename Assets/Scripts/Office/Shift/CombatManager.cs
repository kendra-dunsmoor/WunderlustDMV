using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using Unity.VisualScripting;
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

    // UI objects/meters
    [SerializeField] private Slider performanceMeter;
    [SerializeField] private Slider willMeter;
    [SerializeField] private TextMeshProUGUI customerGoalText;
    [SerializeField] private GameObject[] customerIconQueue;

    // Spawn Points
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform offScreenPoint;
    [SerializeField] private Transform frontOfLinePoint;

    // Combat trackers
    private Queue<Customer> customersInLine = new Queue<Customer>();
    private Customer currCustomer;
    private int CUSTOMER_GOAL = 3;
    private float performanceLevel = 25; // temp range 0 to 50
    private float willLevel = 50; // temp value

    void Start()
    {
        performanceMeter.value = 25f;
        willMeter.value = 50f;
        gameManager = FindFirstObjectByType<GameManager>();
        InitializeCustomerQueue();
    }

    void Update()
    {
        
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
        Debug.Log("Customers remaining: " + customersInLine.Count);
        customerGoalText.text = "Customers remaining: " + customersInLine.Count;
        if (customersInLine.Count == 0) EndShift();
        else {
            currCustomer = customersInLine.Dequeue();
            currCustomer.SendToFront(frontOfLinePoint);
            customerIconQueue[customersInLine.Count].SetActive(false); // this won't work if more customers than queue spots
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
        Debug.Log("Initialize customer queue: " + CUSTOMER_GOAL);
        for (int i = 0; i < CUSTOMER_GOAL; i++) {
            customersInLine.Enqueue(Instantiate(customer, spawnPoint).GetComponent<Customer>());
            customerIconQueue[i].SetActive(true);
        }
        currCustomer = customersInLine.Dequeue();
        customerIconQueue[CUSTOMER_GOAL - 1].SetActive(false);
        currCustomer.SendToFront(frontOfLinePoint);
    }

    private void UpdatePerformance(float diff) {
        Debug.Log("Change performance by " + diff);
        performanceLevel += diff;
        // TODO: check if in range for meter
        performanceMeter.value = performanceLevel;
        Debug.Log("Remaining performance" + performanceLevel);
    }
    private void UpdateWill(float diff) {
        Debug.Log("Change will by -" + diff);
        willLevel -= diff;
        // TODO: check if in range for meter
        willMeter.value = willLevel;
    }

    public void TakeAction (Action action) {
        // Check if sufficient will available for action:
        if (willLevel - action.WILL_MODIFIER < 0) {
            Debug.Log("Insufficient will left for action: " + action.actionName);
            return;
        }

        // Update meters:
        Debug.Log("Taking action: " + action.actionName);
        UpdatePerformance(action.PERFORMANCE_MODIFIER);
        UpdateWill(action.WILL_MODIFIER);
        currCustomer.updateFrustration(action.FRUSTRATION_MODIFIER);

        // Move current customer if needed:
        switch (action.movement) {
            case Action.ActionMovement.FRONT:
                Debug.Log("Customer remains in front");
                 break;
            case Action.ActionMovement.AWAY:
                Debug.Log("Customer is removed from queue");
                currCustomer.SendAway(true, offScreenPoint); // TODO: remove green accepted true thing
                NextCustomer();
                break;
            case Action.ActionMovement.BACK:     
                Debug.Log("Customer is moved to back of line"); 
                currCustomer.SendToBack(spawnPoint);
                // TODO: Need like a delay here if it is the only customer left in line if we want them to move back first?
                customersInLine.Enqueue(currCustomer);
                NextCustomer();
                break;
        } 
    }
}
