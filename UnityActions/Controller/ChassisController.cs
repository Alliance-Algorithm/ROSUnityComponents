using UnityEngine;

public class ChassisController : MonoBehaviour
{
    public Vector3 TargetAcc;
    public Vector3 TargetVel;
    public Vector3 CurrentVelocity;
    public Vector3 CurrentAcc;
    private Vector3 lastPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        CurrentVelocity = (transform.position - lastPos) / Time.deltaTime;
        lastPos = transform.position;
        transform.position += TargetVel * Time.deltaTime;
        // transform.position += (TargetAcc * Time.deltaTime + 2 * CurrentVelocity) / 2 * Time.deltaTime;
        CurrentAcc = TargetAcc;
    }
}
