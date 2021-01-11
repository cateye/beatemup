using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    //reference to the baseSprite of any instance of the Actor class
    public SpriteRenderer baseSprite;

    public Animator baseAnim;
    public Rigidbody characterRB;
    public SpriteRenderer shadowSprite;

    public float speed = 2;

    protected Vector3 frontVector;

    public float maxLife = 100.0f;
    public float currentLife = 100.0f;
    public bool isGrounded; //detect collision with the floor
    public bool isAlive = true;

    protected Coroutine knockdownRoutine;
    public bool isKnockedOut;

    public AttackData normalAttack;
    protected virtual void Start()
    {
        currentLife = maxLife;
        isAlive = true;
        baseAnim.SetBool("isAlive", isAlive);
    }

    public virtual void Update()
    {
        //Virtual allows any derived class to override this method
        //Keep Shadow in the floor
        Vector3 shadowSpritePosition = shadowSprite.transform.position;
        shadowSpritePosition.y = 0;
        shadowSprite.transform.position = shadowSpritePosition;
    }

    public virtual void Attack()
    {
        baseAnim.SetTrigger("Attack");
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "Floor")
        {
            isGrounded = true;
            baseAnim.SetBool("isGrounded", isGrounded);
            DidLand();

        }
    }

    protected virtual void OnCollisionExit(Collision collision)
    {
        if (collision.collider.name == "Floor")
        {
            isGrounded = false;
            baseAnim.SetBool("isGrounded", isGrounded);

        }
    }

    protected virtual void DidLand() { }


    public void FlipSprite(bool isFacingLeft)
    {
        //Flip the sprite so the character is going to look at the direction that moves
        if (isFacingLeft)
        {
            frontVector = new Vector3(-1, 0, 0);
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            frontVector = new Vector3(1, 0, 0);
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public virtual void DidHitObject(Collider collider, Vector3 hitPoint, Vector3 hitVector)
    {
        Character character = collider.GetComponent<Character>();
        if (character != null && character.CanBeHit() && collider.tag != gameObject.tag)
        {
            if (collider.attachedRigidbody != null)
            {
                HitCharacter(character, hitPoint, hitVector);
            }
        }
    }

    protected virtual void HitCharacter(Character character, Vector3 hitPoint, Vector3 hitVector)
    {
        //Executed after sombody get puched
        character.EvaluateAttackData(normalAttack, hitVector, hitPoint);
    }

    protected virtual void Die()
    {
        if (knockdownRoutine != null)
        {
            StopCoroutine(knockdownRoutine);
        }
        isAlive = false;
        baseAnim.SetBool("isAlive", isAlive);
        StartCoroutine(Flicker());
        

    }

    protected virtual void SetOpacity(float value)
    {
        Color color = baseSprite.color;
        color.a = value;
        baseSprite.color = color;

    }

    private IEnumerator Flicker()
    {
        int i = 0;
        while(i < 5)
        {
            SetOpacity(0.5f);
            yield return new WaitForSeconds(0.1f);
            SetOpacity(1f);
            yield return new WaitForSeconds(0.1f);
            i++;
        }
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    public virtual void TakeDamage(float damage, Vector3 hitVector, bool knockdown = false)
    {
        FlipSprite(hitVector.x > 0);
        baseAnim.SetTrigger("isHurt");
        currentLife -= damage;
        if(isAlive && currentLife <= 0)
        {
            Die();
        }
        else if (knockdown)
        {
            if (knockdownRoutine == null)
            {
                Vector3 pushbackVector = (hitVector + Vector3.up * 0.75f).normalized;
                characterRB.AddForce(pushbackVector * 250);
                knockdownRoutine = StartCoroutine(KnockdownRoutine());
            }
        }
    }

    public virtual bool CanWalk()
    {
        return true;
    }

    public virtual void FaceTarget(Vector3 targetPoint)
    {
        FlipSprite(transform.position.x - targetPoint.x > 0);
    }

    public virtual bool CanBeHit()
    {
        return isAlive && !isKnockedOut;
    }

    public void DidGetUp()
    {
        isKnockedOut = false;
    }

    public virtual void EvaluateAttackData(AttackData data, Vector3 hitVector, Vector3 hitPoint)
    {
        characterRB.AddForce(data.force * hitVector);
        TakeDamage(data.attackDamage, hitVector, data.knockdown);
        
    }

    protected virtual IEnumerator KnockdownRoutine()
    {
        isKnockedOut = true;
        baseAnim.SetTrigger("Knockout");
        yield return new WaitForSeconds(1.0f);
        baseAnim.SetTrigger("GetUp");
        knockdownRoutine = null;
        
    }
}

[System.Serializable] //so it will appear in the inspector
public class AttackData
{
    public float attackDamage = 10;
    public float force = 50;
    public bool knockdown = false;
}
