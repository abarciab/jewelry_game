using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [SerializeField] bool yOnly = true;

    private void OnEnable()
    {
        Face();
    }

    void Update()
    {
        Face();
    }

    void Face()
    {
        var camPos = Camera.main.transform.position;
        camPos.y = transform.position.y;
        transform.LookAt(camPos);

    }
}
