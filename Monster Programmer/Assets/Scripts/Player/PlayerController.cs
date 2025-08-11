using Manager;
using System.Collections;
using UI.Controll;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Main;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private AnimationClip idleAnimation;
    [SerializeField] private AnimationClip walkFrontAnimation;
    [SerializeField] private AnimationClip walkBackAnimation;
    [SerializeField] private AnimationClip walkSideAnimation;

    [Header("Audio")]
    [SerializeField] private float delayPlay = 0.4f;
    [SerializeField] private AudioClip[] stepClips;

    private InputSystem_Actions playerInput;
    private Vector2 movementInput = Vector2.zero;
    private IInteractable currentInteractable;

    private void Awake()
    {
        Main = this;

        playerInput = new InputSystem_Actions();
        playerInput.Player.Enable();

        playerInput.Player.Move.started += ctx => OnMove(ctx);
        playerInput.Player.Move.performed += ctx => OnMove(ctx);
        playerInput.Player.Move.canceled += ctx => OnMoveCanceled();

        playerInput.Player.Interact.performed += ctx =>  //E button
        {
            OnInteract();
        };

        RefreshUiAction();
    }

    private void Update()
    {
        HandleMovement();
        HandleAnimation();
    }

    #region Static Controller

    public void OnPlayerMove(Vector2 _direction)
    {
        movementInput = _direction;
    }

    public void OnPlayerInteract()
    {
        OnInteract();
    }

    #endregion

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

        PlayFootStep();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Bisa interact");
        IInteractable interactable = collision.GetComponent<IInteractable>();
        if (interactable != null)
        {
            currentInteractable = interactable;
            RefreshUiAction();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<IInteractable>() != null)
        {
            currentInteractable = null;
            RefreshUiAction();
        }
    }

    #region Trigger Button

    UiController uiButtonAction;

    private void RefreshUiAction()
    {
        if (uiButtonAction == null)
        {
            UiController[] controls = FindObjectsByType<UiController>(FindObjectsSortMode.None);
            for (int i = 0; i < controls.Length; i++)
            {
                if (controls[i].IsName("ButtonAction"))
                {
                    uiButtonAction = controls[i];
                    break;
                }
            }
        }

        if (uiButtonAction == null)
            return;

        uiButtonAction.gameObject.SetActive(currentInteractable != null);
    }

    #endregion



    bool canPlay;
    private void PlayFootStep()
    {
        if (stepClips == null)
            return;

        if (canPlay)
            return;

        canPlay = true;

        StartCoroutine(PlaySound());

        IEnumerator PlaySound()
        {
            int rand = Random.Range(0, stepClips.Length);
            SoundManager.Instance.PlaySoundEffect(stepClips[rand]);

            yield return new WaitForSeconds(delayPlay);
            
            canPlay = false;
        }
    }
}

