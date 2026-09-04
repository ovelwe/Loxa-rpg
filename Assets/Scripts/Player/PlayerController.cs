using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController2D : MonoBehaviour
{
    public float moveSpeed = 5f;
    
    private CharacterController controller;
    private Camera cam;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main;
    }

    void Update()
    {
        // Движение по X и Y
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 move = new Vector3(horizontal, vertical, 0f).normalized;

        controller.Move(move * moveSpeed * Time.deltaTime);

        // Поворот в 2D (направление правым боком / осью X)
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z; // Выравниваем Z-координату

        Vector3 direction = (mousePos - transform.position).normalized;
        if (direction.sqrMagnitude > 0.001f)
        {
            transform.right = direction;
        }
    }
}