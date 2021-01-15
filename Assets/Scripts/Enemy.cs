using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : Character
{
    public EnemyAI ai;
    public static int TotalEnemies;
    public Walker walker;
    public bool stopMovementWhenHit = true;


    protected override void Start()
    {
        base.Start();
        lifeBar =
      GameObject.FindGameObjectWithTag("EnemyLifeBar").GetComponent<LifeBar>();
        lifeBar.FillHpBar(currentLife / maxLife);
    }

    public void RegisterEnemy()
    {
        TotalEnemies++;
    }

    protected override void Die()
    {
        base.Die();
        ai.enabled = false;
        walker.enabled = false;
        TotalEnemies--;

    }

    public void MoveTo(Vector3 targetPosition)
    {
        walker.MoveTo(targetPosition);
    }

    public void MoveToOffset(Vector3 targetPosition, Vector3 offset)
    {
        if (!walker.MoveTo(targetPosition + offset))
        {
            walker.MoveTo(targetPosition - offset);
        }
    }

    public void Wait()
    {
        walker.StopMovement();
    }

    public override void TakeDamage(float value, Vector3 hitVector, bool knockdown = false)
    {
        if (stopMovementWhenHit)
        {
            walker.StopMovement();
        }
        base.TakeDamage(value, hitVector, knockdown);
    }

    public override bool CanWalk()
    {
        return !baseAnim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_hurt") && !baseAnim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_getup");
    }
}
