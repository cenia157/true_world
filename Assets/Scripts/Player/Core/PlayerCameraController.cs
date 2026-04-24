using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform target;

    [Header("Sensitive")]
    [SerializeField] private float mouseSensitivity = 3f;

    [Header("Distance")]
    [SerializeField] private Vector3 targetOffset = new Vector3(0f, 3f, 0f);
    [SerializeField] private float distance = 10f;
    [SerializeField] private float minPitch = -40f;
    [SerializeField] private float maxPitch = 60f;

    [Header("collision")]
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private float collisionRadius = 0.3f;
    [SerializeField] private float collisionOffset = 0.2f;

    [Header("Cursor")]
    [SerializeField] private KeyCode cursorToggleKey = KeyCode.LeftAlt;

    private float yaw;
    private float pitch;
    private bool isCursorFree = false;

    public Transform Target => target;
    public bool IsCursorFree => isCursorFree;

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        SetCursorFree(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(cursorToggleKey))
        {
            SetCursorFree(!isCursorFree);
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        if (!isCursorFree)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            yaw += mouseX * mouseSensitivity;
            pitch -= mouseY * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 targetPosition = target.position + targetOffset;
        Vector3 desiredPosition = targetPosition + (rotation * new Vector3(0f, 0f, -distance));

        Vector3 direction = (desiredPosition - targetPosition).normalized;
        float maxDistance = distance;

        if (Physics.SphereCast(
            targetPosition,
            collisionRadius,
            direction,
            out RaycastHit hit,
            maxDistance,
            collisionMask
        ))
        {
            float hitDistance = hit.distance - collisionOffset;
            hitDistance = Mathf.Clamp(hitDistance, 0.5f, distance);

            transform.position = targetPosition + direction * hitDistance;
        }
        else
        {
            transform.position = desiredPosition;
        }

        transform.rotation = rotation;
    }

    private void SetCursorFree(bool free)
    {
        isCursorFree = free;

        if (isCursorFree)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public Vector3 GetCameraForwardOnPlane()
    {
        Vector3 forward = transform.forward;
        forward.y = 0f;
        return forward.normalized;
    }

    public Vector3 GetCameraRightOnPlane()
    {
        Vector3 right = transform.right;
        right.y = 0f;
        return right.normalized;
    }
}