using System.Collections;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;
using UnityEngine.AI;

public class ForwardPulisher : MonoBehaviour
{
    public string TopicName = "/sentry/forward_dir/pub"; // Change this to your desired image topic
    public float fps = 10;
    public float SpeedMax = 0.3f;
    public float SpeedMin = 0.1f;
    public float MaxSpeedDistance = 2;

    public Transform Target;

    ROSConnection ros;
    RosMessageTypes.Geometry.Pose2DMsg msg;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<RosMessageTypes.Geometry.Pose2DMsg>(TopicName);

        StartCoroutine(Publisher());
    }
    IEnumerator Publisher()
    {
        while (true)
        {
            yield return new WaitForSeconds(1 / fps);
            msg = new();
            Vector3 vec = Target.position - transform.position;
            vec = new(vec.x, 0, vec.z);
            vec = (Mathf.Clamp01(vec.magnitude / MaxSpeedDistance) * (SpeedMax - SpeedMin) + SpeedMin) * vec.normalized;
            Debug.DrawLine(transform.position, vec + transform.position);
            msg = new(vec.x, -vec.z, 0);
            ros.Publish(TopicName, msg);
        }
    }
}
