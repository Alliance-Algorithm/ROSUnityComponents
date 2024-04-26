using System.Collections;
using UnityEngine;

public class AvgFilter : MonoBehaviour
{
    Quaternion lastRotation = new();
    Vector3 lastVector = new();
    public float t = 0.0001f;
    public float deadAngle = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastRotation = transform.rotation;

    }

    // Update is called once per frame
    void Update()
    {
        if (Quaternion.Angle(lastRotation, transform.rotation) < deadAngle)
            transform.rotation = lastRotation;
        transform.rotation = Quaternion.Slerp(lastRotation, transform.rotation, t);
        transform.position = Vector3.Lerp(lastVector, transform.position, t);
        // Quaternion q = last / transform.rotation;
        lastRotation = transform.rotation;
        lastVector = transform.position;
    }
}
