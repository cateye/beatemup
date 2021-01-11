using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class CameraBounds : MonoBehaviour
{
    public float minVisibleX;
    public float maxVisibleX;
    private float minValue;
    private float maxValue;
    public float cameraHalfWidth;

    public float offset; //for CalculateOffset

    public Transform leftBounds;
    public Transform rightBounds;


    public Camera activeCamera;
    public Transform cameraRoot;

    public Transform introWalkStart;
    public Transform introWalkEnd;
    public Transform exitWalkEnd;




    // Start is called before the first frame update
    void Start()
    {
        activeCamera = Camera.main;

        cameraHalfWidth = Mathf.Abs(activeCamera.ScreenToWorldPoint(new Vector3(0, 0, 0)).x - activeCamera.ScreenToWorldPoint(new Vector3(Screen.width,0,0)).x) * 0.5f;
        minValue = minVisibleX + cameraHalfWidth;
        maxValue = maxVisibleX - cameraHalfWidth;

        Vector3 position;
        position = leftBounds.transform.localPosition;
        position.x = transform.localPosition.x - cameraHalfWidth;
        leftBounds.transform.localPosition = position;

        position = rightBounds.transform.localPosition;
        position.x = transform.localPosition.x + cameraHalfWidth;
        rightBounds.transform.localPosition = position;

        //markers for Player entrance animation
        position = introWalkStart.transform.localPosition;
        position.x = transform.localPosition.x - cameraHalfWidth - 2.0f;
        introWalkStart.transform.localPosition = position;
        
        position = introWalkEnd.transform.localPosition;
        position.x = transform.localPosition.x - cameraHalfWidth + 2.0f;
        introWalkEnd.transform.localPosition = position;
        ////markers for Player exit animation
        position = exitWalkEnd.transform.localPosition;
        position.x = transform.localPosition.x + cameraHalfWidth + 2.0f;
        exitWalkEnd.transform.localPosition = position;
        //
    }

    public void SetXPosition(float x)
    {
        Vector3 trans = cameraRoot.position;
        trans.x = Mathf.Clamp(x + offset, minValue, maxValue);
        cameraRoot.position = trans;
    }

    public void CalculateOffset(float playerPosition)
    {
        //Computes the horizontal distance between the playerPosition and the camera's x position

        offset = cameraRoot.position.x - playerPosition;
        SetXPosition(playerPosition);
        StartCoroutine(EaseOffset());

    }

    public void EnableBounds(bool isEnable)
    {
        rightBounds.GetComponent<Collider>().enabled = isEnable;
        leftBounds.GetComponent<Collider>().enabled = isEnable;
    }

    private IEnumerator EaseOffset()
    {
        while (offset != 0)
        {
            offset = Mathf.Lerp(offset, 0, 0.1f);
            if (Mathf.Abs(offset) < 0.05f)
            {
                offset = 0;
            }
            yield return new WaitForFixedUpdate();
        }
    }


}
