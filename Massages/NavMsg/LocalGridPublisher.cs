using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ROS2;
using std_msgs.msg;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(ROS2UnityComponent))]
[RequireComponent(typeof(LocalGridMapBuilder))]
public class LocalGridPublisher : MonoBehaviour
{
    public string NodeName = "unity_ros2_node";
    public string TopicName = "/sentry/nav/local/grid"; // Change this to your desired image topic\
    public string FrameId = "unity"; // Change this to your desired image topic
    private IPublisher<nav_msgs.msg.OccupancyGrid> publisher;
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    public float fps = 10;
    public LocalGridMapBuilder builder;

    public std_msgs.msg.Header header;

    nav_msgs.msg.OccupancyGrid msg;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        header = new Header() { Frame_id = FrameId };
        // Initialize ROS connection
        ros2Unity = GetComponent<ROS2UnityComponent>();
        builder = GetComponent<LocalGridMapBuilder>();
        builder.Build(out sbyte[,] ignore);

        ros2Node = ros2Unity.CreateOrGetNode(NodeName);
        publisher = ros2Node.CreatePublisher<nav_msgs.msg.OccupancyGrid>(TopicName);

        msg = new()
        {
            Header = header,
            Data = new sbyte[ignore.Length]
        };

        StartCoroutine(Publisher());
    }
    IEnumerator Publisher()
    {
        while (true)
        {
            yield return new WaitForSeconds(1 / fps);
            builder.Build(out sbyte[,] ignore);
            builder.MakeESDF(ref ignore);
            msg.Info = new()
            {
                Width = (uint)ignore.GetLength(0),
                Height = (uint)ignore.GetLength(1),
                Resolution = builder.Resolution,
                Origin = new()
                {
                    // Position = new() { X = transform.position.x, Y = transform.position.z },
                    Orientation = new() { X = 0, Y = 0, Z = 0, W = 1 }
                }
            };
            Buffer.BlockCopy(ignore, 0, msg.Data, 0, ignore.Length);
            publisher.Publish(msg);
        }
    }
}
