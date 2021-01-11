using UnityEngine;

public class InputHandler : MonoBehaviour
{
    float horizontal;
    float vertical;
    bool jump;

    bool attack;

    float lastJumpTime;
    bool isJumping;
    public float maxJumpDuration = 0.2f;

    
    public float GetVerticalAxis()
    {
        return vertical;
    }

    public float GetHorizontalAxis()
    {
        return horizontal;
    }

    public bool GetAttackButtonDown()
    {
        return attack;
    }
    public bool GetJumpButtonDown()
    {
        return jump;
    }

    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        attack = Input.GetButtonDown("Attack");
        if(!jump && !isJumping && Input.GetButton("Jump"))
        {
            jump = true;
            lastJumpTime = Time.time;
            isJumping = true;
            
        } else if (!Input.GetButton("Jump"))
        {
            jump = false;
            isJumping = false;
            
        }

        if(jump && Time.time > lastJumpTime + maxJumpDuration)
        {
            jump = false;
        }
    }

}
