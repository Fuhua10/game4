using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation2D : MonoBehaviour
{
    [SerializeField] private PlayerMovement2D movement;

    private Animator animator;
    private bool hasState;
    private bool wasMoving;
    private PlayerMovement2D.Direction lastDirection;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        hasState = false;
    }

    private void LateUpdate()
    {
        if (hasState && wasMoving == movement.IsMoving && lastDirection == movement.Facing)
            return;

        wasMoving = movement.IsMoving;
        lastDirection = movement.Facing;
        hasState = true;
        string action = wasMoving ? "Walk_" : "Idle_";
        int state = Animator.StringToHash("Base Layer." + action + lastDirection);
        animator.Play(state, 0, 0f);
    }
}
