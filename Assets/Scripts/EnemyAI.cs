using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyAI : MonoBehaviour
{
    public enum EnemyAction
    {
        None, Wait, Attack, Chase, Roam
    }

    public class DecisionWeight
    {
        public int weight;
        public EnemyAction action;
        public DecisionWeight(int weight, EnemyAction action)
        {
            this.weight = weight;
            this.action = action;
        }
    }

    Enemy enemy;
    GameObject playerObj;


    public float attackReachMin;
    public float attackReachMax;
    public float personalSpace;

    public HeroDetector detector;
    List<DecisionWeight> weights;

    public EnemyAction currentAction = EnemyAction.None;

    private float decisionDuration;

    private void Start()
    {
        weights = new List<DecisionWeight>();
        enemy = GetComponent<Enemy>();

        playerObj = GameObject.FindGameObjectWithTag("Player");
    }

    private void Chase()
    {
        // move the enemy into attack position
        //go to the hero's position + an offset


        Vector3 directionVector = playerObj.transform.position -
          transform.position;
        directionVector.z = directionVector.y = 0; //we only need 
        directionVector.Normalize();
        //set the enemy to a poin in front of the payer (not on top)
        directionVector *= -1f;
        directionVector *= personalSpace;
        //to get a more natura attack position we use random z.
        directionVector.z += Random.Range(-0.4f, 0.4f);
        //move the enemy to offset
        enemy.MoveToOffset(playerObj.transform.position,
          directionVector);

        decisionDuration = Random.Range(0.2f, 0.4f);
    }

    private void Wait()
    {
        decisionDuration = Random.Range(0.2f, 0.5f);
        enemy.Wait();
    }

    private void Attack()
    {
        enemy.FaceTarget(playerObj.transform.position);
        enemy.Attack();
        decisionDuration = Random.Range(1.0f, 1.5f);
    }

    private void Roam()
    {
        float randomDegree = Random.Range(0, 360);
        Vector2 offset = new Vector2(Mathf.Sin(randomDegree),
          Mathf.Cos(randomDegree));
        float distance = Random.Range(1, 3);
        offset *= distance;
        Vector3 directionVector = new Vector3(offset.x, 0, offset.y);
        enemy.MoveTo(enemy.transform.position + directionVector);
        decisionDuration = Random.Range(0.3f, 0.6f);
    }

    private void DecideWithWeights(int attack, int wait, int chase, int move)
    {
        weights.Clear();
        //1
        if (attack > 0)
        {
            weights.Add(new DecisionWeight(attack, EnemyAction.Attack));
        }
        if (chase > 0)
        {
            weights.Add(new DecisionWeight(chase, EnemyAction.Chase));
        }
        if (wait > 0)
        {
            weights.Add(new DecisionWeight(wait, EnemyAction.Wait));
        }
        if (move > 0)
        {
            weights.Add(new DecisionWeight(move, EnemyAction.Roam));
        }
        //2
        int total = attack + chase + wait + move;
        int intDecision = Random.Range(0, total - 1);
        //3
        foreach (DecisionWeight weight in weights)
        {
            intDecision -= weight.weight;
            if (intDecision <= 0)
            {
                SetDecision(weight.action);
                break;
            }
        }
    }

    private void SetDecision(EnemyAction action)
    {
        currentAction = action;
        if (action == EnemyAction.Attack)
        {
            Attack();
        }
        else if (action == EnemyAction.Chase)
        {
            Chase();
        }
        else if (action == EnemyAction.Roam)
        {
            Roam();
        }
        else if (action == EnemyAction.Wait)
        {
            Wait();
        }
    }

    void Update()
    {
        //Distance between Player and Enemy (squared distance or magnitud to run battery smooth)
        float sqrDistance = Vector3.SqrMagnitude(
          playerObj.transform.position - transform.position);
        
        bool canReach = attackReachMin * attackReachMin < sqrDistance
          && sqrDistance < attackReachMax * attackReachMax;
        
        bool samePlane = Mathf.Abs(playerObj.transform.position.z -
          transform.position.z) < 0.5f;
        
        if (canReach && currentAction == EnemyAction.Chase)
        {
            SetDecision(EnemyAction.Wait);
        }
        
        if (decisionDuration > 0.0f)
        {
            decisionDuration -= Time.deltaTime;
        }
        else
        {
            if (!detector.playerIsNearby)
            {
                DecideWithWeights(0, 20, 80, 0);
            }
            else
            {
                if (samePlane)
                {
                    if (canReach)
                    {
                        DecideWithWeights(70, 15, 0, 15);
                    }
                    else
                    {
                        DecideWithWeights(0, 10, 80, 10);
                    }
                }
                else
                {
                    DecideWithWeights(0, 20, 60, 20);
                }
            }
        }
    }
}