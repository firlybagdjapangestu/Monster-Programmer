using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private AnimationClip idleAnimation;
    [SerializeField] private AnimationClip walkFrontAnimation;
    [SerializeField] private AnimationClip walkBackAnimation;
    [SerializeField] private AnimationClip walkSideAnimation;

    private InputSystem_Actions playerInput;
    private Vector2 movementInput = Vector2.zero;
    private IInteractable currentInteractable;

    private void Awake()
    {
        playerInput = new InputSystem_Actions();
        playerInput.Player.Enable();

        playerInput.Player.Move.started += ctx => OnMove(ctx);
        playerInput.Player.Move.performed += ctx => OnMove(ctx);
        playerInput.Player.Move.canceled += ctx => OnMoveCanceled();

        playerInput.Player.Interact.performed += ctx => OnInteract(); // Interaksi saat tombol E ditekan
    }

    private void Update()
    {
        HandleMovement();
        HandleAnimation();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled()
    {
        movementInput = Vector2.zero;
    }

    private void OnInteract()
    {
        if (currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    private void HandleMovement()
    {
        if (movementInput != Vector2.zero)
        {
            transform.Translate(movementInput * speed * Time.deltaTime, Space.World);
        }
    }

    private void HandleAnimation()
    {
        if (movementInput == Vector2.zero)
        {
            animator.Play(idleAnimation.name);
            return;
        }

        // Horizontal movement (left/right)
        if (Mathf.Abs(movementInput.x) > Mathf.Abs(movementInput.y))
        {
            animator.Play(walkSideAnimation.name);
            spriteRenderer.flipX = movementInput.x < 0;
        }
        // Vertical movement (up/down)
        else if (movementInput.y > 0)
        {
            animator.Play(walkFrontAnimation.name);
        }
        else
        {
            animator.Play(walkBackAnimation.name);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Bisa interact");
        IInteractable interactable = collision.GetComponent<IInteractable>();
        if (interactable != null)
        {
            currentInteractable = interactable;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<IInteractable>() != null)
        {
            currentInteractable = null;
        }
    }
}
