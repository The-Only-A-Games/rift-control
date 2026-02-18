using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerTopDownLocomotion : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 10;

    [Header("Dash")]
    public float dashSpeed = 25f;
    public float dashDuration = 0.15f;
    public int maxDashCharges = 2;
    public float dashRefillDelay = 0.4f;

    private int currentDashCharges;
    private float dashRefillTimer;

    private bool isDashing;
    private bool canMove = true;

    private CharacterController controller;
    private Camera camera;

    private Vector3 velocity;

    void Start()
    {
        camera = Camera.main;
        controller = GetComponent<CharacterController>();
        currentDashCharges = maxDashCharges;
    }

    void Update()
    {
        if (controller == null) return;

        RotateToMouse();

        if (canMove)
            Move();

        HandleDash();
        HandleDashRefill();
    }

    void HandleDash()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame &&
            currentDashCharges > 0 &&
            !isDashing)
        {
            currentDashCharges--;
            dashRefillTimer = 0f;
            StartCoroutine(Dash());
        }
    }

    void HandleDashRefill()
    {
        if (currentDashCharges >= maxDashCharges) return;

        dashRefillTimer += Time.deltaTime;

        if (dashRefillTimer >= dashRefillDelay)
        {
            currentDashCharges++;
            dashRefillTimer = 0f;
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        canMove = false;

        float timer = 0f;

        while (timer < dashDuration)
        {
            controller.Move(transform.forward * dashSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        canMove = true;
        isDashing = false;
    }

    void RotateToMouse()
    {
        Vector3 mousePosition = MousePosition(Mouse.current.position.ReadValue());

        if (mousePosition != Vector3.zero)
        {
            mousePosition.y = transform.position.y;
            transform.LookAt(mousePosition);
        }
    }

    void Move()
    {
        Vector3 direction = MoveDirection().normalized;

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += Physics.gravity.y * Time.deltaTime;

        Vector3 finalMove = direction * moveSpeed + velocity;

        controller.Move(finalMove * Time.deltaTime);
    }

    Vector3 MousePosition(Vector3 target)
    {
        Ray ray = camera.ScreenPointToRay(target);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000))
            return hit.point;

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