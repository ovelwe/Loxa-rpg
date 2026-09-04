using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -10);
    
    [Tooltip("Примерное время в секундах, за которое камера догоняет цель")]
    public float smoothTime = 0.3f; // Чем больше число, тем сильнее задержка (попробуйте от 0.1 до 0.5)

    private Vector3 currentVelocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        
        // SmoothDamp делает задержку с плавным ускорением и торможением
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothTime);
    }
}