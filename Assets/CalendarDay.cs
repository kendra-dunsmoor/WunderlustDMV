using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
* Calendar Day
* ~~~~~~~~~~~~
* Data object for calendar UI weekday
*/
[System.Serializable]
public class CalendarDay
{
    public TextMeshProUGUI optionA;
    public TextMeshProUGUI optionB;
    public TextMeshProUGUI orText;
    public GameObject shiftCompleteMarker;
}
