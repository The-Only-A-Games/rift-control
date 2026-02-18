using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTopDownLocomotion : MonoBehaviour
{
    [Header("Attributes")]
    public float moveSpeed = 10;
    // public float rotationSpeed = 3f;

    [Header("GameObjects and Components")]
    private CharacterController controller;
    private Camera camera;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camera = Camera.main;

        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Prevet running the code if components are not found
        if (
            controller == null
        ) return;

        // ----- PLAYER ROTATION -----\\
        Vector3 mousePosition = MousePosition(Mouse.current.position.ReadValue());
        mousePosition.y = 0; // To make sure we dont rotate on the y-axis

        if (mousePosition != Vector3.zero)
        {
            transform.LookAt(mousePosition);
        }

        // ----- PLAYER MOVEMENT ----- \\
        Vector3 direction = MoveDirection();

        controller.Move(direction * moveSpeed * Time.deltaTime);


    }

    Vector3 MousePosition(Vector3 target)
    {
        int maxDistance = 1000;
        // Allows us to fire raycast from the center of the screen
        // Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        // Allows us to create a raycast from screen point
        Ray ray = camera.ScreenPointToRay(target);

        // Captures cast information
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            Debug.Log(hit.point);
            return hit.point;
        }

        return Vector3.zero;

    }

    Vector3 MoveDirection()
    {
        float h = 0;
        float v = 0;

        if (Keyboard.current.aKey.isPressed) h = -1;
        if (Keyboard.current.dKey.isPressed) h = 1;

        if (Keyboard.current.sKey.isPressed) v = -1;
        if (Keyboard.current.wKey.isPressed) v = 1;

        return new Vector3(h, 0, v);
    }
}
