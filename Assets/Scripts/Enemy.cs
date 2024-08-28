using AssemblyCSharp.Assets.Scripts;
using UnityEngine;

public class Enemy : Movement
{
    [SerializeField] bool isOnRope = false;
    MovementDirection movementDirection;
    bool isAlive;
    bool killedPlayer;

    // Start is called before the first frame update
    void Start()
    {
        this.movementCharacter = GetComponent<Rigidbody2D>();
        this.bodyCharacter = GetComponent<CapsuleCollider2D>();
        this.flooredCharacter = GetComponent<BoxCollider2D>();
        this.movementAnimator = GetComponent<Animator>();
        this.runSpeed = 1f;
        this.movementDirection = MovementDirection.Right;
        this.isAlive = true;
        this.killedPlayer = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!this.isAlive || this.killedPlayer) { return; }

        if (bodyCharacter.IsTouchingLayers(LayerMask.GetMask(Layers.Player)))
        {
            var player = FindObjectOfType<Player>();
            player.Die();
            this.killedPlayer = true;
            this.MoveStop();
            return;
        }

        switch (this.movementDirection)
        {
            case MovementDirection.Left:
                MoveLeft();
                break;
            case MovementDirection.Right:
                MoveRight();
                break;
            // Enemy unable to climb ladders or fly
            case MovementDirection.Up:
            case MovementDirection.Down:
            default:
                break;
        }
    }

    public void Die()
    {
        movementAnimator.SetTrigger(Animations.Death);
        movementCharacter.velocity = new Vector2(0, 0);
        isAlive = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isOnRope)
        {
            TurnAround();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isOnRope == false)
        {
            TurnAround();
        }
    }

    private void TurnAround()
    {
        transform.localScale = new Vector2(-(Mathf.Sign(this.movementCharacter.velocity.x)), 1f);
        this.movementDirection = transform.localScale.x > 0 ? MovementDirection.Right : MovementDirection.Left;
    }
}
