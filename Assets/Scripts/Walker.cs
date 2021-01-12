using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Character))]
public class Walker : MonoBehaviour
{
  
    public NavMeshAgent navMeshAgent;
    private NavMeshPath navPath;
    private List<Vector3> corners;
    
    float currentSpeed;
    float speed;
    
    private Character character;
    private System.Action didFinishWalk;
    
    void Start()
    {
        //prevent NavMeshAgent from updating this GameObject's transform
        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;
        character = GetComponent<Character>();
    }

    public bool MoveTo(Vector3 targetPosition, System.Action callback = null)
    {
        navMeshAgent.Warp(transform.position);
        didFinishWalk = callback;
        speed = character.speed;
        navPath = new NavMeshPath();
        bool pathFound = navMeshAgent.CalculatePath(targetPosition, navPath);
        if (pathFound)
        {
            corners = navPath.corners.ToList();
            return true;
        }
        return false;
    }

    public void StopMovement()
    {
        navPath = null;
        corners = null;
        currentSpeed = 0;
    }

    protected void FixedUpdate()
    {
        bool canWalk = character.CanWalk();
        if(canWalk && corners != null && corners.Count > 0)
        {
            currentSpeed = speed;
            //move the character to the position in the list
            character.characterRB.MovePosition(Vector3.MoveTowards(transform.position, corners[0], Time.fixedDeltaTime * speed));
            //once the walker reahces a corner, remove it form the list
            if(Vector3.SqrMagnitude(transform.position-corners[0]) < 0.6f)
            {
                corners.RemoveAt(0);
            }
            //flip the character when it's necesary (if direction.x >=0)
            if (corners.Count > 0) {
                currentSpeed = speed;
                Vector3 direction = transform.position - corners[0];
                character.FlipSprite(direction.x >= 0);
            } else
            {
                //Finshin walkin
                currentSpeed = 0.0f;
                if(didFinishWalk != null)
                {
                    didFinishWalk.Invoke();
                    didFinishWalk = null;
                }
            }
        }
        character.baseAnim.SetFloat("Speed", currentSpeed);
    }
}

