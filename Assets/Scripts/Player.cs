using System;
using System.Collections;
using UnityEngine;

public class Player : Character
{

    public InputHandler input; //reference to the InputHandler

    private const float WALK_SPEED = 2;
    private const float RUN_SPEED = 5;

    Vector3 currentDir;

    bool isFacingLeft;
    bool isRunning;
    bool isMoving;
    float lastWalk;
    public bool canRun = true;
    float tapAgainToRunTime = 0.2f;
    Vector3 lastWalkVector;



    //Jump variables
    bool isJumpLandAnim;
    bool isJumpingAnim;
    public float jumpForce = 1750;
    private float jumpDuration = 0.2f;
    private float lastJumpTime;
    public bool canJumpAttack = true;

    private int currentAttackChain = 1;
    public int evaluatedAttackChain = 0;

    public AttackData jumpAttack;

    //Attack variables
    bool isAttackingAnim;
    float lastAttackTime;
    float attackLimit = 0.14f;
    bool isHurtAnim;

    //Walking in/ou variables
    public Walker walker;
    public bool isAutoPiloting;
    public bool controllable = true;

    //Run Attack Data
    public AttackData runAttack;
    public float runAttackForce = 1.8f;

    //Combo Data
    public AttackData normalAttack2;
    public AttackData normalAttack3;
    
    float chainComboTimer;
    public float chainComboLimit = 0.3f;
    const int maxCombo = 3;

    //tolerance to being hit often
    public float hurtTolerance;
    public float hurtLimit = 20f;
    public float recoveryRate = 5f;

    protected override void Start()
    {
        base.Start();
        lifeBar =
      GameObject.FindGameObjectWithTag("PlayerLifeBar").GetComponent<LifeBar>();
        lifeBar.FillHpBar(currentLife / maxLife);
    }

    public override void Update()
    {
        base.Update();
        if (!isAlive) { return; }
        //check if hurt animation is playing
        isHurtAnim = baseAnim.GetCurrentAnimatorStateInfo(0).IsName("Player_hurt");
        //check if the attack animation is playing
        isAttackingAnim =
          baseAnim.GetCurrentAnimatorStateInfo(0).IsName("attack1") ||
          baseAnim.GetCurrentAnimatorStateInfo(0).IsName("Player_attack_2") ||
          baseAnim.GetCurrentAnimatorStateInfo(0).IsName("Player_attack_3") ||
          baseAnim.GetCurrentAnimatorStateInfo(0).IsName("Player_run_attack") ||
          baseAnim.GetCurrentAnimatorStateInfo(0).IsName("Player_jump_attack");
        //check if any of the jump animation is playing
        isJumpLandAnim = baseAnim.GetCurrentAnimatorStateInfo(0).IsName("Player_jump_land");
        isJumpingAnim = baseAnim.GetCurrentAnimatorStateInfo(0).IsName("Player_jump_rise") || baseAnim.GetCurrentAnimatorStateInfo(0).IsName("Player_jump_fall");
        //
        if (isAutoPiloting) { return; }

        float h = input.GetHorizontalAxis();
        float v = input.GetVerticalAxis();
        bool jump = input.GetJumpButtonDown();
        bool attack = input.GetAttackButtonDown();

        currentDir = new Vector3(h, 0, v);
        currentDir.Normalize();
        float now = Time.time;
        if (!isAttackingAnim && !isKnockedOut)
        {
            if (chainComboTimer > 0)        
            {
                chainComboTimer -= Time.deltaTime;
                if (chainComboTimer < 0)
                {
                    chainComboTimer = 0;
                    currentAttackChain = 0;
                    evaluatedAttackChain = 0;
                    baseAnim.SetInteger("CurrentChain", currentAttackChain);
                    baseAnim.SetInteger("EvaluatedChain", evaluatedAttackChain);
                }
            }
                if (v == 0 && h == 0) 
            {
                PlayerStop();
                isMoving = false;
            }
            else if (!isMoving && (v != 0 || h != 0))
            {
                isMoving = true;
                //a positive dotProduct means the same direction was pressed twice
                float dotProduct = Vector3.Dot(currentDir, lastWalkVector);

                //if tap two times in the same direction the player run 
                if (canRun && now < lastWalk + tapAgainToRunTime && dotProduct > 0)
                {
                    PlayerRun();
                }
                else
                {
                    PlayerWalk();
                    //Store current movement direction and current time (only horizontal)
                    if (h != 0)
                    {
                        lastWalkVector = currentDir;
                        lastWalk = now;
                    }
                }
            }
        }

        if (jump && !isJumpLandAnim && !isKnockedOut && !isAttackingAnim && (isGrounded || now < lastJumpTime + jumpDuration))
        {
            PlayerJump(currentDir);

        }

        if (!isKnockedOut && attack && (now >= lastAttackTime + attackLimit))
        {
            lastAttackTime = now;
            Attack();
        }

        //after few hits it will trigger the knockdown
        if(hurtTolerance < hurtLimit)
        {
            hurtTolerance += Time.deltaTime * recoveryRate;
            hurtTolerance = Mathf.Clamp(hurtTolerance, 0, hurtLimit);
        }

    }

    private void FixedUpdate()
    {
        if (!isAlive) { return; }
        if (!isAutoPiloting)
        {

            Vector3 moveVector = currentDir * speed;
            //move character
            if (isGrounded && !isAttackingAnim && !isKnockedOut && !isHurtAnim)
            {
                characterRB.MovePosition(transform.position + moveVector * Time.deltaTime);
                baseAnim.SetFloat("Speed", moveVector.magnitude);
            }


            baseAnim.SetBool("isRunning", isRunning);

            if (moveVector != Vector3.zero && isGrounded && !isKnockedOut && !isAttackingAnim)
            {
                if (moveVector.x != 0)
                {
                    isFacingLeft = moveVector.x < 0;
                }
                FlipSprite(isFacingLeft);
            }
        }
    }


    public void PlayerStop()
    {
        speed = 0;
        isRunning = false;
        baseAnim.SetFloat("Speed", speed);
    }

    public void PlayerWalk()
    {
        speed = WALK_SPEED;
        isRunning = false;
        baseAnim.SetFloat("Speed", speed);
    }

    public void PlayerRun()
    {
        speed = RUN_SPEED;
        isRunning = true;
        // baseAnim.SetBool("isRunning", isRunning);
        baseAnim.SetFloat("Speed", speed);
    }

    public void PlayerJump(Vector3 currentDir)
    {
        if (!isJumpingAnim)
        {
            //Trigger animation
            baseAnim.SetTrigger("Jump");
            lastJumpTime = Time.time;
            //Add force to the Characther in de horizontal direction
            Vector3 horizontalVector = new Vector3(currentDir.x, 0, currentDir.z) * speed * 40;
            characterRB.AddForce(horizontalVector, ForceMode.Force);

        }
        //Add force to the character to jump
        Vector3 verticalVector = Vector3.up * jumpForce * Time.deltaTime;

        characterRB.AddForce(verticalVector, ForceMode.Force);
    }

    protected override void DidLand()
    {
        base.DidLand();
        canJumpAttack = true;
        PlayerWalk();
    }

    public override void Attack()
    {
        if (currentAttackChain <= maxCombo)
        {
            if (!isGrounded)
            {

                if (isJumpingAnim && canJumpAttack)
                {

                    canJumpAttack = false;
                    currentAttackChain = 1;
                    evaluatedAttackChain = 0;
                    baseAnim.SetInteger("EvaluatedChain", evaluatedAttackChain);
                    baseAnim.SetInteger("CurrentChain", currentAttackChain);
                    //pause the player rigidbody in the air until animation ends
                    characterRB.velocity = Vector3.zero;
                    characterRB.useGravity = false;

                }
            }
            else
            if (isRunning)
            {
                //creates lunge with upward and forward force
                characterRB.AddForce((Vector3.up + (frontVector * 5)) * runAttackForce, ForceMode.Impulse);

                currentAttackChain = 1;
                evaluatedAttackChain = 0;
                baseAnim.SetInteger("CurrentChain", currentAttackChain);
                baseAnim.SetInteger("EvaluatedChain", evaluatedAttackChain);

            }
            else
            {
                if (currentAttackChain == 0 || chainComboTimer == 0)
                {
                    currentAttackChain = 1;
                    evaluatedAttackChain = 0;
                }
                baseAnim.SetInteger("EvaluatedChain", evaluatedAttackChain);
                baseAnim.SetInteger("CurrentChain", currentAttackChain);
            }
        }


    }

    public void DidChain(int chain )
    {
        //This is called from the animation Event
        evaluatedAttackChain = chain;
        baseAnim.SetInteger("EvaluatedChain", evaluatedAttackChain);
    }

    //Method that animate the entrance of the character using  walker class
    public void AnimateTo(Vector3 position, bool shouldRun, Action callback)
    {
        if (shouldRun)
        {
            PlayerRun();
        }
        else
        {
            PlayerWalk();
        }
        walker.MoveTo(position, callback);
    }
    //enable or disable the walker
    public void UseAutopilot(bool useAutopilot)
    {
        isAutoPiloting = useAutopilot;
        walker.enabled = useAutopilot;
    }

    public override void TakeDamage(float damage, Vector3 hitVector, bool knockdown = false)
    {
        hurtTolerance -= damage;
        if(hurtTolerance <= 0 || !isGrounded)
        {
            hurtTolerance = hurtLimit;
            knockdown = true;
        }
        base.TakeDamage(damage, hitVector, knockdown);

    }

    public void DidJumpAttack()
    {
        characterRB.useGravity = true;
        currentAttackChain = 0;
        evaluatedAttackChain = 0;
        baseAnim.SetInteger("EvaluatedChain", evaluatedAttackChain);
        baseAnim.SetInteger("CurrentChain", currentAttackChain);
    }

    private void AnalizeSpecialAttack(AttackData attackData, Character character, Vector3 hitPoint, Vector3 hitVector)
    {
        character.EvaluateAttackData(attackData, hitVector, hitPoint);
        chainComboTimer = chainComboLimit; //update the chain timer when there's a special attack
    }

    private void AnalyzeNormalAttack(AttackData attackData, int attackChain, Character character, Vector3 hitPoint, Vector3 hitVector)
    {
        character.EvaluateAttackData(attackData, hitVector, hitPoint);
        currentAttackChain = attackChain;
        chainComboTimer = chainComboLimit;
    }


    protected override void HitCharacter(Character character, Vector3 hitPoint, Vector3 hitVector)
    {
        if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("attack1"))
        {
            AnalyzeNormalAttack(normalAttack, 2, character, hitPoint, hitVector);

        }
        else if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("Player_attack_2"))
        {
            AnalyzeNormalAttack(normalAttack2, 3, character, hitPoint, hitVector);
        }
        else if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("Player_attack_3"))
        {
            AnalyzeNormalAttack(normalAttack3, 1, character, hitPoint, hitVector);
        }
        else if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("Player_jump_attack"))
        {
            AnalizeSpecialAttack(jumpAttack, character, hitPoint, hitVector);
        }
        else if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("Player_run_attack"))
        {
            AnalizeSpecialAttack(runAttack, character, hitPoint, hitVector);
        }
    }

    public override bool CanWalk()
    {
        return (isGrounded && !isAttackingAnim && !isKnockedOut && !isHurtAnim && !isJumpLandAnim);
    }

    protected override IEnumerator KnockdownRoutine()
    {
        characterRB.useGravity = true;
        return base.KnockdownRoutine();
    }

}