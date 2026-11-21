using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput))]
public class PlayerMovement2D : MonoBehaviour
{
    public enum Direction { Down, Left, Right, Up }

    [SerializeField, Min(0f)] private float moveSpeed = 3f;

    public Direction Facing { get; private set; } = Direction.Down;
    public bool IsMoving { get; private set; }

    private Rigidbody2D body;
    private InputAction move;
    private Vector2 movement;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        move = GetComponent<PlayerInput>().actions.FindAction("Player/Move", throwIfNotFound: true);
    }

    private void Update()
    {
        movement = Vector2.ClampMagnitude(move.ReadValue<Vector2>(), 1f);
        IsMoving = movement.sqrMagnitude > 0.0001f;
        if (!IsMoving)
        {
            movement = Vector2.zero;
            return; // 松开按键时保留上次朝向。
        }

        float horizontal = Mathf.Abs(movement.x);
        float vertical = Mathf.Abs(movement.y);
        bool wasHorizontal = Facing == Direction.Left || Facing == Direction.Right;
        // 沿对角线走时，沿用上一次的朝向轴，避免动画来回跳。
        bool faceHorizontal = Mathf.Approximately(horizontal, vertical) ? wasHorizontal : horizontal > vertical;
        if (faceHorizontal)
            Facing = movement.x > 0f ? Direction.Right : Direction.Left;
        else
            Facing = movement.y > 0f ? Direction.Up : Direction.Down;
    }

    private void FixedUpdate()
    {
        body.velocity = movement * moveSpeed;
    }

    private void OnDisable()
    {
        movement = Vector2.zero;
        IsMoving = false;
        if (body != null)
            body.velocity = Vector2.zero;
    }
}
