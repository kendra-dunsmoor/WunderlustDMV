using UnityEngine;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    // Manage Frustration Bar and movement/reactions

    private float frustrationLevel;
    [SerializeField] private GameObject frustrationMeter;
    [SerializeField] private Sprite acceptedSprite;
    [SerializeField] private Sprite rejectedSprite;
    private bool movingToPoint;
    private Transform goalPoint;
    [SerializeField] private float moveSpeed;
    
    // Temporary solution:
    [SerializeField] private Transform front;
    [SerializeField] private Transform next;
    [SerializeField] private Transform offScreen;
    [SerializeField] private Camera mainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        frustrationLevel = 0;
        moveSpeed = 5;
    }

    // Update is called once per frame
    void Update()
    {
        if (movingToPoint)
        {
            transform.position = Vector2.MoveTowards(transform.position, goalPoint.position,
                moveSpeed * Time.deltaTime);
            if (transform.position.x >= goalPoint.position.x)
            {
                movingToPoint = false;
                if (goalPoint == front)
                {
                    GameObject.FindGameObjectWithTag("CombatManager").GetComponent<CombatManager>().SpawnPaperwork();
                }
            }
            // Transform slider to be above
        }
    }

    public void SendToFront()
    {
        movingToPoint = true;
        goalPoint = front;
    }

    public void SendToNext()
    {
        movingToPoint = true;
        goalPoint = next;
    }

    public void SendAway(bool accepted)
    {
        movingToPoint = true;
        goalPoint = offScreen;
        // toogle paperwork visibility false
        GameObject.FindGameObjectWithTag("Paperwork").SetActive(false);
        if (accepted)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = acceptedSprite;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = rejectedSprite;
        }
    }
}
