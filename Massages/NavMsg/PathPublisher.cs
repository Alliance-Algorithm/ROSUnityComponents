using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ROS2;
using RosMessageTypes.Std;
using Unity.Robotics.ROSTCPConnector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
public class PathPublisher : MonoBehaviour
{
    public string TopicName = "/sentry/nav/global/path"; // Change this to your desired image topic\
    public string FrameId = "unity"; // Change this to your desired image topic
    public float fps = 10;
    ROSConnection ros;
    public NavMeshUpdater navUpdater;
    private Vector3 begin;

    public HeaderMsg header;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        begin = transform.position;
        // Initialize ROS connection
        ros = ROSConnection.GetOrCreateInstance();

        ros.RegisterPublisher<RosMessageTypes.Nav.PathMsg>(TopicName);

        StartCoroutine(Publisher());
        header = new() { frame_id = FrameId };
    }
    IEnumerator Publisher()
    {
        while (true)
        {
            yield return new WaitForSeconds(1 / fps);
            RosMessageTypes.Nav.PathMsg msg = new();
            List<RosMessageTypes.Geometry.PoseStampedMsg> poses = new();
            foreach (var c in navUpdater.Path.corners)
            {
                var d = c - begin;
                // Debug.Log(begin);
                poses.Add(new() { pose = new() { position = new() { x = d.x, y = d.z } }, header = header });
            }
            msg.poses = poses.ToArray();
            msg.header = header;
            ros.Publish(TopicName, msg);
        }
    }
}
