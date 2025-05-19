using UnityEngine;
using UnityEngine.UI;

// Manage Frustration Bar, effects, and movement/reactions
public class Customer : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxFrustration;
    private float frustrationLevel;
    [SerializeField] private FloatingHealthBar frustrationMeter;
    [SerializeField] private Sprite acceptedSprite;
    [SerializeField] private Sprite rejectedSprite;
    [SerializeField] private Sprite iconSprite;
    // temp:
    private bool movingToFront;
    private bool movingAway;
    private bool movingBack;
    private Transform goalPoint;


    // TODO: add active effects tracking

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        frustrationLevel = 0;
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
                GameObject.FindGameObjectWithTag("CombatManager").GetComponent<CombatManager>().SpawnPaperwork();
            } 
        }
        if (movingAway)
        {
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
        Debug.Log("Sending customer to front");
        movingToFront = true;
        goalPoint = point;
    }

    public void SendAway(bool accepted, Transform point)
    {
        Debug.Log("Sending customer away");
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
        Debug.Log("Sending customer to back");
        movingBack = true;
        goalPoint = point;
        // toogle paperwork visibility false
        GameObject.FindGameObjectWithTag("Paperwork").SetActive(false);
    }

    public void updateFrustration(float change) {
        frustrationLevel += change;
        Debug.Log("Customer frustration level updated to: " + frustrationLevel);
        frustrationMeter.UpdateBar(frustrationLevel, maxFrustration);
    }
}
