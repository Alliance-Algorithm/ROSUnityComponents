using System.Collections;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class TransformSubscription2D : MonoBehaviour
{
    public string TopicName = "/sentry/transform/show"; // Change this to your desired image topic
    public bool IsGlobal = false;
    ROSConnection ros;
    private CameraCapturer capturer;
    Vector3 begin = new();
    Vector3 v = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize ROS connection
        ros = ROSConnection.GetOrCreateInstance();

        if (!IsGlobal)
            begin = transform.position;

        ros.Subscribe<RosMessageTypes.Geometry.Pose2DMsg>(TopicName, CallBack);

    }
    void CallBack(RosMessageTypes.Geometry.Pose2DMsg msg)
    {
        v = new Vector3((float)msg.x, 0, (float)msg.y);
        // transform.localEulerAngles = new(0, (float)msg.Theta, 0);
    }
    void Update()
    {
        transform.position = v + begin;
    }
}
