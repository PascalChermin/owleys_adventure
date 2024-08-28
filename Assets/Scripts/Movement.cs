using AssemblyCSharp.Assets.Scripts;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] internal float runSpeed = 1f;
    [SerializeField] internal float jumpSpeed = 1f;
    [SerializeField] internal float climbSpeed = 1f;
    internal Animator movementAnimator;
    internal Rigidbody2D movementCharacter;
    internal CapsuleCollider2D bodyCharacter;
    internal BoxCollider2D flooredCharacter;
    internal bool isOnFloor;
    internal bool isClimbing;

    internal void Idle()
    {
        foreach (var item in movementAnimator.parameters)
        {
            if (item.type == AnimatorControllerParameterType.Bool)
                movementAnimator.SetBool(item.name, false);
        }
    }

    internal void MoveLeft()
    {
        Move(-1f);
    }

    internal void MoveRight()
    {
        Move(1f);
    }

    internal void MoveStop()
    {
        if (movementCharacter == null) return;
        movementCharacter.velocity = new Vector2(0, 0);
    }

    private void Move(float direction)
    {
        if (movementCharacter == null) return;

        if (!CharacterNextToLadder() || isOnFloor) { isClimbing = false; }
        movementCharacter.transform.localScale = new Vector3(direction, 1);
        movementCharacter.velocity = new Vector2(
            direction * (isClimbing ? climbSpeed : runSpeed),
            movementCharacter.gravityScale * movementCharacter.velocity.y);
        movementAnimator.speed = isClimbing ? 0.75f : 1f;
    }

    internal void Jump()
    {
        if (movementCharacter == null) return;

        if (isOnFloor)
        {
            movementCharacter.velocity += new Vector2(0f, jumpSpeed);
            isOnFloor = false;
        }
    }

    internal void ClimbLadder(float direction)
    {
        if (movementCharacter == null) return;

        if (CharacterNextToLadder())
        {
            isClimbing = true;
            movementAnimator.SetBool(Animations.Climbing, true);
            movementAnimator.speed = 0.75f;
            movementCharacter.velocity = new Vector2(0, direction * climbSpeed);
        }
        else
        {
            isClimbing = false;
        }
    }

    internal bool CharacterNextToLadder()
    {
        return movementCharacter.IsTouchingLayers(LayerMask.GetMask(Layers.Ladder));
    }
}
