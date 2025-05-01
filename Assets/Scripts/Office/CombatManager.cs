using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    // Manager queue of customers and goals/progress
    
    // Prefabs
    [SerializeField] private GameObject shiftOverMenu;
    
    private int CUSTOMER_GOAL = 3;
    private int customersRemaining = 3;
    [SerializeField] private Customer[] queue;
    [SerializeField] private GameObject paperwork;
    private int currCustomer = 0;
    
    
    // Temporary, just sticking meter management here for now
    [SerializeField] private Slider efficiencyMeter;
    [SerializeField] private Slider willMeter;
    [SerializeField] private TextMeshProUGUI customerGoalText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        efficiencyMeter.value = 0.5f;
        willMeter.value = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // Button Interaction Functions
    public void Accept()
    {
        efficiencyMeter.value += 0.2f;
        willMeter.value -= 0.2f;
        NextCustomer(true);
    }
    
    public void Reject()
    {
        efficiencyMeter.value -= 0.2f;
        willMeter.value -= 0.2f;
        NextCustomer(false);
    }
    
    public void Escalate()
    {
        efficiencyMeter.value -= 0.2f;
        willMeter.value -= 0.2f;
        NextCustomer(false);
    }

    private void EndShift()
    {
        // Pop up end screen
        Instantiate(shiftOverMenu, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
    }

    private void NextCustomer(bool accepted)
    {
        customersRemaining -= 1;
        customerGoalText.text = "Customers remaining: " + customersRemaining;
        queue[currCustomer].SendAway(accepted);
        currCustomer++;
        if (currCustomer != 3) queue[currCustomer].SendToFront();
        // temp ugly hard code
        if (currCustomer == 1) queue[currCustomer + 1].SendToNext();
        if (customersRemaining == 0)
        {
            EndShift();
        }
    }
    
    public void SpawnPaperwork()
    {
        // TODO: Paperwork will be unique/randomized and instantiated in
        paperwork.SetActive(true);
    }
}
