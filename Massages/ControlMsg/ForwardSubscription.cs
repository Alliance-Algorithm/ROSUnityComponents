using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class ForwardSubscription : MonoBehaviour
{
    public float Speed = 0.1f;
    public float ErrorRate = 0.01f;
    public string NodeName = "unity_ros2_node";
    public string TopicName = "/sentry/transform/velocity"; // Change this to your desired image topic
    public string FrameId = "unity"; // Change this to your desired image topic
    public float fps = 10;

    ROSConnection ros;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<RosMessageTypes.Geometry.Pose2DMsg>(TopicName, CallBack);
    }
    Vector3 v = new();
    void CallBack(RosMessageTypes.Geometry.Pose2DMsg msg)
    {
        v = new Vector3((float)msg.x, 0, (float)msg.y);
    }
    void Update()
    {
        var t = (float)v.z * transform.forward + (float)v.x * transform.right;
        t = t + Quaternion.AngleAxis(Random.Range(-180, 180), Vector3.up) * Vector3.right * ErrorRate * Mathf.Clamp(t.magnitude, 0, 1);
        // Debug.Log(v);
        transform.position += t * Time.deltaTime;
        Debug.DrawLine(transform.position + Vector3.up * 6, transform.position + t + Vector3.up * 6, Color.blue);
    }
}
