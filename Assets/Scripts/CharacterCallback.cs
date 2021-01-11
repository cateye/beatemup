using UnityEngine;

public class CharacterCallback : MonoBehaviour
{
    public Character character;

    public void DidGetUp()
    {
        character.DidGetUp();
    }
}
