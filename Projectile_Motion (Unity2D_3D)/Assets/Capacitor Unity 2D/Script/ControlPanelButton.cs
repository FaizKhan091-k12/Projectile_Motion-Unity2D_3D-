using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ControlPanelButton : MonoBehaviour
{
    
    Camera mainCamera;
    private RectTransform controlPanel_RectTransform;
    
    
    [Header("ControlPanel Settings")] 
    [SerializeField] private Button controlPanel_Button;
    [SerializeField] private float transitionDuration = 0.4f;
    [SerializeField] private bool isPanelOpen;
    [SerializeField] private float min_Pivot, max_Pivot;

    [Header("Fade Targets")]
    [SerializeField] private Image[] imagesToFade;
    [SerializeField] private TextMeshProUGUI textToFade;

    public bool wantToAnimateCameraViewport;
    public float cameraOffsetValue;
    void Start()
    {
        mainCamera = Camera.main;
        if (!controlPanel_RectTransform)
            controlPanel_RectTransform = GetComponent<RectTransform>();

        controlPanel_Button.onClick.AddListener(ClickControlPanelButton);
    }

    public void ClickControlPanelButton()
    {
        // Animate pivot
        Vector2 targetPivot = isPanelOpen ? new Vector2(min_Pivot, controlPanel_RectTransform.pivot.y)
            : new Vector2(max_Pivot, controlPanel_RectTransform.pivot.y);
        controlPanel_RectTransform.DOPivot(targetPivot, transitionDuration).SetEase(Ease.InOutSine);

        // Fade UI elements
        float targetAlpha = isPanelOpen ? .8f : 0f;
        foreach (var img in imagesToFade)
        {
            img.DOFade(targetAlpha, transitionDuration);
            img.raycastTarget = isPanelOpen;
        }

        if (textToFade != null)
        {
            textToFade.DOFade(targetAlpha, transitionDuration);
        }

        // Animate camera.rect.x
        if (wantToAnimateCameraViewport)
        {
            float targetX = isPanelOpen ? 0f : cameraOffsetValue;
            DOTween.To(() => mainCamera.rect.x, 
                x => {
                    Rect r = mainCamera.rect;
                    r.x = x;
                    mainCamera.rect = r;
                },
                targetX, 
                transitionDuration
            ).SetEase(Ease.InOutSine);
        }
      

        isPanelOpen = !isPanelOpen;
    }

}