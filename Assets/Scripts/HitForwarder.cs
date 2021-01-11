using UnityEngine;

public class HitForwarder : MonoBehaviour
{
    public Character character;
    public Collider triggerCollider;

    private void OnTriggerEnter(Collider hitCollider)
    {
        Vector3 direction = new Vector3(hitCollider.transform.position.x - character.transform.position.x, 0, 0);
        direction.Normalize();

        BoxCollider collider = triggerCollider as BoxCollider;
        Vector3 centerPoint = this.transform.position;
        if (collider)
        {
            centerPoint = transform.TransformPoint(collider.center);
        }

        Vector3 startPoint = hitCollider.ClosestPointOnBounds(centerPoint);
        character.DidHitObject(hitCollider, startPoint, direction);
    }

}
