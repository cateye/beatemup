using UnityEngine;

public class PlayerCallback : MonoBehaviour
{
    public Player player;

    public void DidChain(int chain)
    {
        player.DidChain(chain);
    }
    public void DidJumpAttack()
    {
        player.DidJumpAttack();
    }
}
