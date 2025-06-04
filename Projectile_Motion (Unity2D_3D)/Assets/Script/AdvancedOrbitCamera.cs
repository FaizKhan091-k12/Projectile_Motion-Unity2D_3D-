using UnityEngine;
using UnityEngine.EventSystems;

public class AdvancedOrbitCamera : MonoBehaviour
{
    public static AdvancedOrbitCamera instance;

    [Header("Camera Settings")]
    [SerializeField] private Camera orbitCamera;

    [Header("Initial View")]
    [SerializeField] private float defaultZoom = 10f;
    [SerializeField] private float defaultHorizontalRotation = 0f;
    [SerializeField] private float defaultVerticalRotation = 20f;

    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 cameraTargetOffset = Vector3.zero;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private bool invertY = false;

    [SerializeField] private bool restrictVerticalRotation = true;
    [SerializeField] private float minVerticalAngle = -20f;
    [SerializeField] private float maxVerticalAngle = 80f;

    [SerializeField] private bool restrictHorizontalRotation = false;
    [SerializeField] private float minHorizontalAngle = -360f;
    [SerializeField] private float maxHorizontalAngle = 360f;

    [SerializeField] private bool requireRightClick = false;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 20f;

    [Header("Smoothing")]
    [SerializeField] private float smoothTime = 0.15f;

    [Header("Target Height Mapping")]
    [SerializeField] private float minTargetY = 0f;
    [SerializeField] private float maxTargetY = 5f;
    [SerializeField] private bool useTargetYPosition = false;
    [SerializeField] private float minTargetX = 0f;
    [SerializeField] private float maxTargetX = 5f;
    [SerializeField] private bool useTargetXPosition = false;

    public bool canOrbit = true;

    private float targetX, targetY, currentX, currentY;
    private float rotationVelocityX, rotationVelocityY;
    private float targetDistance, currentDistance, zoomVelocity;

    private void Awake()
    {
        instance = this;

        if (orbitCamera == null)
            orbitCamera = Camera.main;
    }

    private void Start()
    {
        if (!target)
        {
            target = new GameObject("CameraTarget").transform;
        }

        ApplyInitialView();
    }

    private void ApplyInitialView()
    {
        defaultZoom = Mathf.Clamp(defaultZoom, minZoom, maxZoom);
        if (restrictVerticalRotation)
            defaultVerticalRotation = Mathf.Clamp(defaultVerticalRotation, minVerticalAngle, maxVerticalAngle);

        if (restrictHorizontalRotation)
            defaultHorizontalRotation = Mathf.Clamp(defaultHorizontalRotation, minHorizontalAngle, maxHorizontalAngle);

        targetDistance = currentDistance = defaultZoom;
        targetX = currentX = defaultHorizontalRotation;
        targetY = currentY = defaultVerticalRotation;
    }

    private void OnValidate()
    {
        if (!Application.isPlaying) return;
        ApplyInitialView();
    }

    public void OrbitControlsWorkingState(bool CanOrbit)
    {
        this.canOrbit = CanOrbit;
    }

    private void LateUpdate()
    {
        if (!canOrbit || orbitCamera == null) return;

        HandleInput();
        ApplySmoothCamera();

    #if UNITY_EDITOR
        if (Application.isPlaying)
        {
            defaultHorizontalRotation = currentX;
            defaultVerticalRotation = currentY;
            defaultZoom = currentDistance;
        }
    #endif
    }

    private void HandleInput()
    {
        bool isPointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

    #if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        bool isRotating = !requireRightClick || Input.GetMouseButton(1);

        if (!isPointerOverUI && isRotating && Input.GetMouseButton(0))
        {
            targetX += Input.GetAxis("Mouse X") * rotationSpeed;
            float yDelta = Input.GetAxis("Mouse Y") * rotationSpeed;
            targetY += invertY ? yDelta : -yDelta;
        }

        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetDistance -= scroll * zoomSpeed;
        }
    #endif

    #if UNITY_ANDROID || UNITY_IOS || UNITY_SIMULATOR
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 delta = Input.GetTouch(0).deltaPosition;
            targetX += delta.x * rotationSpeed * 0.02f;
            targetY += (invertY ? delta.y : -delta.y) * rotationSpeed * 0.02f;
        }
        else if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 prevPos0 = t0.position - t0.deltaPosition;
            Vector2 prevPos1 = t1.position - t1.deltaPosition;
            float prevMag = Vector2.Distance(prevPos0, prevPos1);
            float currMag = Vector2.Distance(t0.position, t1.position);
            float deltaMag = prevMag - currMag;

            targetDistance += deltaMag * zoomSpeed * 0.005f;
        }
    #endif

        targetDistance = Mathf.Clamp(targetDistance, minZoom, maxZoom);

        if (restrictVerticalRotation)
            targetY = Mathf.Clamp(targetY, minVerticalAngle, maxVerticalAngle);

        if (restrictHorizontalRotation)
            targetX = Mathf.Clamp(targetX, minHorizontalAngle, maxHorizontalAngle);
    }

    private void ApplySmoothCamera()
    {
        currentX = Mathf.SmoothDamp(currentX, targetX, ref rotationVelocityX, smoothTime);
        currentY = Mathf.SmoothDamp(currentY, targetY, ref rotationVelocityY, smoothTime);
        currentDistance = Mathf.SmoothDamp(currentDistance, targetDistance, ref zoomVelocity, smoothTime);

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 direction = rotation * Vector3.forward;

        float mappedY = useTargetYPosition ? Mathf.Lerp(minTargetY, maxTargetY, Mathf.InverseLerp(minZoom, maxZoom, currentDistance)) : target.position.y;
        float mappedX = useTargetXPosition ? Mathf.Lerp(minTargetX, maxTargetX, Mathf.InverseLerp(minZoom, maxZoom, currentDistance)) : target.position.x;

        Vector3 updatedTargetPos = new Vector3(mappedX, mappedY, target.position.z);
        target.position = updatedTargetPos;

        orbitCamera.transform.position = updatedTargetPos + cameraTargetOffset - direction * currentDistance;
        orbitCamera.transform.LookAt(updatedTargetPos + cameraTargetOffset);
    }

#if UNITY_EDITOR
    public float CurrentUpRotation => currentY;
    public float CurrentSideRotation => currentX;
    public float CurrentZoom => currentDistance;
#endif
}
