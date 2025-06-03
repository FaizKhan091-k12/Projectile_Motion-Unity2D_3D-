using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.Serialization;

public class SliderController : MonoBehaviour 
{
    private Slider slider;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI slider_Value;
    [SerializeField] private Button incrementButton;
    [SerializeField] private Button decrementButton;

    [Header("Step Settings")]
    [Tooltip("How much to increase on each step")]
    [SerializeField] private float increaseValue = 1f;
    [Tooltip("How much to decrease on each step")]
    [SerializeField] private float decreaseValue = 1f;

    [Header("Optional Custom SI Unit")]
    [SerializeField] private string customSIUnit = "";

    [Tooltip("How Much Decimal Length 'Like You Want '1.0' Type '0.0' and if '1.00' Type '0.00'")]
    [SerializeField] private string howManyNumberAfterPoint;

    [Header("Hold Settings")]
    [Tooltip("Delay between repeated steps (on hold)")]
    [SerializeField] private float holdRepeatDelay = 0.1f;

    private Coroutine holdCoroutine;

    private void Start()
    {
        if (string.IsNullOrWhiteSpace(howManyNumberAfterPoint))
        {
            howManyNumberAfterPoint = "0.0";
        }

        slider = GetComponent<Slider>();
            
       

      
        slider.onValueChanged.AddListener(delegate { SliderValueChanged(); });
        if (incrementButton != null && decrementButton != null)
        {

            incrementButton.onClick.AddListener(IncrementSlider);
            decrementButton.onClick.AddListener(DecrementSlider);

            AddHoldEvents(incrementButton, IncrementSlider);
            AddHoldEvents(decrementButton, DecrementSlider);
        }

        SliderValueChanged(); 
    }

   

    void SliderValueChanged()
    {
        string unit = string.IsNullOrWhiteSpace(customSIUnit) ? "" : "" + customSIUnit;
        slider_Value.text = slider.value.ToString(howManyNumberAfterPoint) + unit;

        if (incrementButton != null && decrementButton != null)
        {
            incrementButton.interactable = slider.value < slider.maxValue;
            decrementButton.interactable = slider.value > slider.minValue;

        }
    }

    void IncrementSlider()
    {
        slider.value = Mathf.Clamp(slider.value + increaseValue, slider.minValue, slider.maxValue);
    }

    void DecrementSlider()
    {
        slider.value = Mathf.Clamp(slider.value - decreaseValue, slider.minValue, slider.maxValue);
    }

    void AddHoldEvents(Button button, System.Action action)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (!trigger)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        // Pointer Down
        EventTrigger.Entry pointerDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        pointerDown.callback.AddListener((e) => { holdCoroutine = StartCoroutine(RepeatHold(action)); });
        trigger.triggers.Add(pointerDown);

        // Pointer Up
        EventTrigger.Entry pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        pointerUp.callback.AddListener((e) => { if (holdCoroutine != null) StopCoroutine(holdCoroutine); });
        trigger.triggers.Add(pointerUp);

        // Pointer Exit (in case user drags out)
        EventTrigger.Entry pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExit.callback.AddListener((e) => { if (holdCoroutine != null) StopCoroutine(holdCoroutine); });
        trigger.triggers.Add(pointerExit);
    }

    IEnumerator RepeatHold(System.Action action)
    {
        yield return new WaitForSeconds(0.3f); // initial hold delay
        while (true)
        {
            action.Invoke();
            yield return new WaitForSeconds(holdRepeatDelay);
        }
    }
}
