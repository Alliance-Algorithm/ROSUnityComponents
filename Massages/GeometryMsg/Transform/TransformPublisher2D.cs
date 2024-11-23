using System.Collections;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;
public class TransformPublisher : MonoBehaviour
{
    public string TopicName = "/sentry/transform/publish"; // Change this to your desired image topic
    public string FrameId = "unity"; // Change this to your desired image topic
    public float fps = 10;
    ROSConnection ros;
    private CameraCapturer capturer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize ROS connection
        ros = ROSConnection.GetOrCreateInstance();

        ros.RegisterPublisher<RosMessageTypes.Geometry.Pose2DMsg>(TopicName);

        StartCoroutine(Publisher());
    }
    IEnumerator Publisher()
    {
        while (true)
        {
            yield return new WaitForSeconds(1 / fps);
            RosMessageTypes.Geometry.Pose2DMsg msg = new();
            msg = new()
            {
                x = transform.position.x,
                y = transform.position.z,
                theta = -transform.localEulerAngles.y
            };

            ros.Publish(TopicName, msg);
        }
    }
}
