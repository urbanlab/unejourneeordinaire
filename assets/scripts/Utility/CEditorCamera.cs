using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CEditorCamera : MonoBehaviour
{

#if UNITY_EDITOR
    public float onKeyFactor = 10f;


    private Vector3 totalRotation = new Vector3(0, 0, 0);


    public GameObject GetGameObject()
    {
        return gameObject;
    }


    private void TurnCameraRight(float factor)
    {
        totalRotation.y += Time.deltaTime * factor;
    }

    private void TurnCameraLeft(float factor)
    {
        totalRotation.y -= Time.deltaTime * factor;
    }

    private void TurnCameraUp(float factor)
    {
        totalRotation.x -= Time.deltaTime * factor;
    }

    private void TurnCameraDown(float factor)
    {
        totalRotation.x += Time.deltaTime * factor;
    }

    // Use this for initialization
    void Start()
    {

    }

    void OnDestroy()
    {

    }


    void Update()
    {

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            TurnCameraRight(onKeyFactor);
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.Q))
        {
            TurnCameraLeft(onKeyFactor);
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Z))
        {
            TurnCameraUp(onKeyFactor);
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            TurnCameraDown(onKeyFactor);
        }

        gameObject.transform.localEulerAngles = totalRotation;
    }

    public void Set(Quaternion quat)
    {
        totalRotation = quat.eulerAngles;
    }
#endif
}
