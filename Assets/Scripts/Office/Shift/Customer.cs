using UnityEngine;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    // Manage Frustration Bar and movement/reactions

    private float frustrationLevel;
    [SerializeField] private GameObject frustrationMeter;
    [SerializeField] private Sprite acceptedSprite;
    [SerializeField] private Sprite rejectedSprite;
    [SerializeField] private Sprite iconSprite;
    // temp:
    private bool movingToFront;
    private bool movingAway;
    private bool movingBack;
    private Transform goalPoint;
    [SerializeField] private float moveSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        frustrationLevel = 0;
        moveSpeed = 5;
    }

    // Update is called once per frame
    void Update()
    {
        if (movingToFront)
        {
            transform.position = Vector2.MoveTowards(transform.position, goalPoint.position,
                moveSpeed * Time.deltaTime);
            if (transform.position.x >= goalPoint.position.x)
            {
                movingToFront = false;
                Debug.Log("Made it to front!");
                GameObject.FindGameObjectWithTag("CombatManager").GetComponent<CombatManager>().SpawnPaperwork();
            } 
        }
        if (movingAway)
        {
            Debug.Log("Why we here");
            transform.position = Vector2.MoveTowards(transform.position, goalPoint.position,
                moveSpeed * Time.deltaTime);
            if (transform.position.x >= goalPoint.position.x)
            {
                movingAway = false;
                Destroy(gameObject);
            } 
        }
        if (movingBack)
        {
            Debug.Log("Why we here");
            transform.position = Vector2.MoveTowards(transform.position, goalPoint.position,
                moveSpeed * Time.deltaTime);
            if (transform.position.x <= goalPoint.position.x)
            {
                movingBack = false;
            } 
        }
    }

    public void SendToFront(Transform point)
    {
        movingToFront = true;
        goalPoint = point;
        Debug.Log("With customer");
    }

    public void SendAway(bool accepted, Transform point)
    {
        movingAway = true;
        goalPoint = point;
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

        public void SendToBack(Transform point)
    {
        movingBack = true;
        goalPoint = point;
        // toogle paperwork visibility false
        GameObject.FindGameObjectWithTag("Paperwork").SetActive(false);
    }
}
