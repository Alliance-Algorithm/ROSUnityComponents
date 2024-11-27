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

        ros.RegisterPublisher<RosMessageTypes.Geometry.PoseStampedMsg>(TopicName);

        StartCoroutine(Publisher());
    }
    IEnumerator Publisher()
    {
        while (true)
        {
            yield return new WaitForSeconds(1 / fps);
            RosMessageTypes.Geometry.PoseStampedMsg msg = new();
            msg = new()
            {
                pose = new()
                {
                    position = new()
                    {
                        x = transform.position.x,
                        y = transform.position.z,
                        z = transform.position.y
                    },
                    orientation = new()
                    {
                        x = -transform.rotation.x,
                        y = -transform.rotation.z,
                        z = -transform.rotation.y,
                        w = transform.rotation.w
                    }
                },
                header = new() { frame_id = FrameId }
            };

            ros.Publish(TopicName, msg);
        }
    }
}
