using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class MovementController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigBody;
    [SerializeField] private FixedJoystick joystick;
    [SerializeField] private float moveSpeed;
    [SerializeField] [Range(0.1f, 2f)] private float verticalThreshold = 1.5f;
    
    private Animator animator;
    private Vector2 movementInput;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if(rigBody == null) rigBody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        GetInput();
        Move();
        UpdateAnimation();
    }

    private void GetInput()
    {
        movementInput = new Vector2(
            joystick.Horizontal,
            joystick.Vertical
        ).normalized;
    }

    private void Move()
    {
        rigBody.velocity = movementInput * moveSpeed;
    }

    private void UpdateAnimation()
    {
        bool isMoving = movementInput.magnitude > 0.1f;
        animator.SetBool("IsMoving", isMoving);

        if(!isMoving) return;

        // Определяем соотношение осей
        float absX = Mathf.Abs(movementInput.x);
        float absY = Mathf.Abs(movementInput.y);
        
        // Вертикальные анимации только если Y > X * threshold
        bool isVertical = absY > absX * verticalThreshold;

        if(isVertical)
        {
            animator.SetFloat("MoveY", Mathf.Sign(movementInput.y));
            animator.SetFloat("MoveX", 0);
        }
        else
        {
            animator.SetFloat("MoveX", Mathf.Sign(movementInput.x));
            animator.SetFloat("MoveY", 0);
        }
    }
}