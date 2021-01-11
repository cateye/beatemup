using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HeroDetector : MonoBehaviour
{
    public bool playerIsNearby;

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            playerIsNearby = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerIsNearby = false;
        }
    }

}
