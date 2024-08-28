using AssemblyCSharp.Assets.Scripts;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Movement
{
    [SerializeField] float PlayerDeathDelay = 3f;
    [SerializeField] AudioClip audioClipPlayerJump;
    [SerializeField] AudioClip audioClipPlayerHit;
    [SerializeField] AudioClip audioClipGameOver;
    [SerializeField] internal bool PlayerCanGlide = false;
    [SerializeField] internal bool PlayerCanFly = false;
    private ButtonMapping buttonMapping;
    private GameSession gameSession;
    internal bool alreadyDoubleJumped;
    internal bool isAlive;
    internal bool playerEnteringDoor;

    // Start is called before the first frame update
    void Start()
    {
        this.movementCharacter = GetComponent<Rigidbody2D>();
        this.bodyCharacter = GetComponent<CapsuleCollider2D>();
        this.flooredCharacter = GetComponent<BoxCollider2D>();
        this.movementAnimator = GetComponent<Animator>();
        this.runSpeed = 5f;
        this.jumpSpeed = 20f;
        this.climbSpeed = 2.5f;
        this.isOnFloor = true;
        this.isClimbing = false;
        this.playerEnteringDoor = false;

        this.buttonMapping = ButtonMapping.GetInstance();
        this.gameSession = FindObjectOfType<GameSession>();
        this.alreadyDoubleJumped = false;
        this.isAlive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!this.isAlive) { return; }

        // Check if player is able to double jump again
        isOnFloor = CheckFloorCollision(Layers.Ground);
        if (isOnFloor) movementCharacter.gravityScale = 1f;

        CheckAnimationStates();
        movementCharacter.gravityScale = isClimbing ? 0f : 1f;

        // Check if player is idle
        movementAnimator.speed = isClimbing ? 0f : 1f;
        if (Input.anyKey == false) { CheckIdle(); return; }

        // Check if player is performing actions
        if (Input.GetKey(buttonMapping.KeyboardLeft) || Input.GetAxis(Axis.Horizontal) < 0) MoveLeft();
        if (Input.GetKey(buttonMapping.KeyboardRight) || Input.GetAxis(Axis.Horizontal) > 0) MoveRight();
        if (Input.GetKey(buttonMapping.KeyboardUp) || Input.GetAxis(Axis.Vertical) > 0) { if (PlayerAtDoor() && this.playerEnteringDoor == false) { this.playerEnteringDoor = true; EnterDoor(); } else { ClimbLadder(1f); } }
        if (Input.GetKey(buttonMapping.KeyboardDown) || Input.GetAxis(Axis.Vertical) < 0) ClimbLadder(-1f);
        if (Input.GetKeyDown(buttonMapping.KeyboardJump) || Input.GetButtonDown(buttonMapping.ControllerJump)) PlayerJump();
        if (Input.GetKey(buttonMapping.KeyboardJump) || Input.GetButton(buttonMapping.ControllerJump)) PlayerGlide();
    }

    private void CheckAnimationStates()
    {
        // Running if there is horitontal movement and on the floor
        movementAnimator.SetBool(Animations.Running, movementCharacter.velocity.x != 0 && isOnFloor);
        // Jumping if player is not on the floor
        movementAnimator.SetBool(Animations.Jumping, !isOnFloor && !isClimbing);
        // Gliding if player is gliding
        movementAnimator.SetBool(Animations.Gliding, movementCharacter.gravityScale < 1);
        // Climbing if player is not on the floor AND next to a ladder
        movementAnimator.SetBool(Animations.Climbing, isClimbing);
    }

    private void CheckIdle()
    {
        if (isClimbing)
        {
            movementCharacter.velocity = new Vector2(0, 0);
        }
        else if (movementCharacter.velocity.x != 0)
        {
            movementCharacter.velocity = new Vector2(0, movementCharacter.velocity.y);
        }
    }

    internal void PlayerJump()
    {
        if (movementCharacter == null) return;

        // First jump?
        if (isOnFloor)
        {
            var camera = FindObjectOfType<Camera>();
            movementCharacter.velocity += new Vector2(0f, jumpSpeed);
            isOnFloor = false;
            alreadyDoubleJumped = false;
            AudioSource.PlayClipAtPoint(audioClipPlayerJump, camera.transform.position);
        }
        // If player already double-jumped and has not learned fly yet
        else if (alreadyDoubleJumped && PlayerCanFly == false) return;
        // If player hasn't double-jumped yet, or is able to fly
        else
        {
            var camera = FindObjectOfType<Camera>();
            movementCharacter.velocity = new Vector2(movementCharacter.velocity.x, jumpSpeed);
            alreadyDoubleJumped = true;
            AudioSource.PlayClipAtPoint(audioClipPlayerJump, camera.transform.position);
            StartCoroutine(PlayerDoubleJumped());
        }
    }

    private void PlayerGlide()
    {
        if (PlayerCanGlide == false) return;

        if (movementCharacter.velocity.y < 0)
            movementCharacter.gravityScale = 0.01f;
    }

    private bool PlayerAtDoor()
    {
        return bodyCharacter.IsTouchingLayers(LayerMask.GetMask(Layers.Doors));
    }

    private void EnterDoor()
    {
        var doors = GameObject.Find("Doors");
        foreach (Transform door in doors.transform)
        {
            if (door.gameObject.activeInHierarchy && this.bodyCharacter.IsTouching(door.GetComponent<BoxCollider2D>()))
            {
                var d = door.GetComponent<DoorScript>();
                d.EnterDoor();
            }
        }
    }

    internal void Die()
    {
        isAlive = false;
        StartCoroutine(PlayerDeath());
    }

    IEnumerator PlayerDeath()
    {
        var camera = FindObjectOfType<Camera>();
        gameSession.CutBGMAudio();
        AudioSource.PlayClipAtPoint(audioClipGameOver, camera.transform.position, 0.3f);
        movementAnimator.SetTrigger(Animations.Death);
        movementCharacter.velocity = new Vector2(0, 0);
        movementCharacter.gravityScale = 0f;
        yield return new WaitForSecondsRealtime(PlayerDeathDelay);
        SceneManager.LoadScene((int)SceneNumbers.Menu_GameOver, LoadSceneMode.Single);
    }

    // This function gets called after the Die animation ends
    private void DisableSpriteRenderer()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    private bool CheckFloorCollision(string layer)
    {
        return flooredCharacter.IsTouchingLayers(LayerMask.GetMask(layer));
    }

    private IEnumerator PlayerDoubleJumped()
    {
        var dustGameObject = transform.GetChild(0);
        var dust = Instantiate(dustGameObject, transform.parent, true);

        var spriteColor = dust.GetComponent<SpriteRenderer>();
        spriteColor.color = new Color(spriteColor.color.r, spriteColor.color.g, spriteColor.color.b, 1f);
        dust.position = new Vector2(transform.position.x, transform.position.y);
        dust.GetComponent<Animator>().SetTrigger("PlayerDoubleJumped");
        Destroy(dust.gameObject, 0.5f);

        yield return null;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Object that collided with me: " + collision.gameObject.name);
    }
}
