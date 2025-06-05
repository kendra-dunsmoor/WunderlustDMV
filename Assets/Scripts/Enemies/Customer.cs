using UnityEngine;
using System.Collections.Generic;
using static ActionEffect;
using static EnemyData;
using TMPro;

// Manage Frustration Bar, effects, and movement/reactions
public class Customer : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private FloatingHealthBar frustrationMeter;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("------------- Effects -------------")]
    [SerializeField] private GameObject currentEffectPrefab;
    [SerializeField] private Transform currentEffectsPanel;
    [SerializeField] private Dictionary<EffectType, GameObject> activeEffects = new Dictionary<EffectType, GameObject>();
    [SerializeField] private ActionEffect irateEffect;

    private float frustrationLevel;

    // temp:
    private bool movingToFront;
    private bool movingAway;
    private bool movingBack;
    private Transform goalPoint;

    CombatManager combatManager;

    void Start()
    {
        frustrationLevel = 0;
        UpdateFrustration(enemyData.startingFrustration);
        combatManager = GameObject.FindGameObjectWithTag("CombatManager").GetComponent<CombatManager>();
        dialogueBox.SetActive(false);
    }

    void Update()
    {
        if (movingToFront)
        {
            transform.position = Vector2.MoveTowards(transform.position, goalPoint.position,
                enemyData.moveSpeed * Time.deltaTime);
            if (transform.position.x >= goalPoint.position.x)
            {
                movingToFront = false;
                SayDialogueLine(LineType.OPENING);
                combatManager.EnableActions();
                combatManager.SpawnPaperwork();
            }
        }
        if (movingAway)
        {
            transform.position = Vector2.MoveTowards(transform.position, goalPoint.position,
                enemyData.moveSpeed * Time.deltaTime);
            if (transform.position.x >= goalPoint.position.x)
            {
                movingAway = false;
                Destroy(gameObject);
            }
        }
        if (movingBack)
        {
            transform.position = Vector2.MoveTowards(transform.position, goalPoint.position,
                enemyData.moveSpeed * Time.deltaTime);
            if (transform.position.x <= goalPoint.position.x)
            {
                movingBack = false;
                dialogueBox.SetActive(false);
            }
        }
    }

    public void SendToFront(Transform point)
    {
        if (gameObject == null) return;
        Debug.Log("Sending customer to front");
        movingToFront = true;
        goalPoint = point;
    }

    public void SendAway(bool accepted, Transform point)
    {
        if (gameObject == null) return;

        Debug.Log("Sending customer away");
        SayDialogueLine(LineType.POSITIVE);
        movingAway = true;
        goalPoint = point;
        // toogle paperwork visibility false
        GameObject paperwork = GameObject.FindGameObjectWithTag("Paperwork");
        if (paperwork != null) paperwork.SetActive(false);
        if (accepted)
        {
            if (enemyData.acceptedSprite != null)
                gameObject.GetComponent<SpriteRenderer>().sprite = enemyData.acceptedSprite;
        }
        else
        {
            if (enemyData.acceptedSprite != null)
                gameObject.GetComponent<SpriteRenderer>().sprite = enemyData.rejectedSprite;
        }
    }

    public void SendToBack(Transform point)
    {
        if (gameObject == null) return;
        Debug.Log("Sending customer to back");
        SayDialogueLine(LineType.NEGATIVE);
        movingBack = true;
        goalPoint = point;
        // toogle paperwork visibility false
        GameObject.FindGameObjectWithTag("Paperwork").SetActive(false);
    }

    public void UpdateFrustration(float change)
    {
        if (gameObject == null) return;

        frustrationLevel += change;
        Debug.Log("Customer frustration level updated to: " + frustrationLevel);
        if (frustrationMeter != null) frustrationMeter.UpdateBar(frustrationLevel, enemyData.maxFrustration);
        if (frustrationLevel >= enemyData.maxFrustration)
        {
            // Check if customer is already irate:
            if (activeEffects.ContainsKey(EffectType.IRATE)) return;
            // else add effect: 
            GameObject effectMarker = Instantiate(currentEffectPrefab, currentEffectsPanel);
            effectMarker.GetComponent<UIEffectController>().AddEffect(irateEffect, 1);
            effectMarker.GetComponent<MouseOverDescription>().UpdateDescription(irateEffect.effectDescription);
            activeEffects.Add(EffectType.IRATE, effectMarker);
        }
    }

    public Dictionary<EffectType, GameObject> GetActiveEffects()
    {
        return activeEffects;
    }

    public void TakeTurn()
    {
        Debug.Log("Customer Turn Starting");
        // Apply turn frustration
        Debug.Log("Adding frustration for waiting");
        UpdateFrustration(enemyData.frustrationIncreasePerTurn);
        // Take action

        // Add dialogue
        SayDialogueLine(LineType.NEUTRAL);
        Debug.Log("Customer Turn Completed");
        combatManager.EnableActions();
    }

    public void SayDialogueLine(LineType lineType)
    {
        var dialogueChoices = lineType switch
        {
            LineType.OPENING => enemyData.openingDialogueLines,
            LineType.NEUTRAL => enemyData.neutralDialogueLines,
            LineType.NEGATIVE => enemyData.negativeDialogueLines,
            LineType.POSITIVE => enemyData.positiveDialogueLines,
            _ => enemyData.neutralDialogueLines
        };
        dialogueBox.SetActive(true);
        dialogueText.text = dialogueChoices[Random.Range(0, dialogueChoices.Length)];
    }

    public void AddEnemyData(EnemyData data)
    {
        enemyData = data;
    }
}
