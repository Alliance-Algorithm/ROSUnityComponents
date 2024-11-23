using System.Collections;
using ROS2;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class OdenmetryPublisher : MonoBehaviour
{
    public string NodeName = "unity_ros2_node";
    public string TopicName = "/sentry/transform/publish"; // Change this to your desired image topic
    public string FrameId = "unity"; // Change this to your desired image topic
    public float fps = 10;
    ROSConnection ros;
    RosMessageTypes.Std.HeaderMsg header;
    public bool IsGlobal = false;
    Vector3 begin = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        if (!IsGlobal)
            begin = transform.position;
        ros.RegisterPublisher<RosMessageTypes.Nav.OdometryMsg>(TopicName);

        StartCoroutine(Publisher());
        header = new()
        {
            frame_id = FrameId
        };
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator Publisher()
    {
        while (true)
        {
            yield return new WaitForSeconds(1 / fps);
            RosMessageTypes.Nav.OdometryMsg msg = new();
            var v = transform.position - begin;
            msg = new()
            {
                header = header,
                pose = new()
                {
                    pose = new()
                    {
                        position = new() { x = v.x, y = -v.z, z = v.y },
                        orientation = new() { x = transform.rotation.x, y = -transform.rotation.z, z = transform.rotation.y, w = transform.rotation.w }
                    }
                }
            };

            ros.Publish(TopicName, msg);
        }
    }
}
