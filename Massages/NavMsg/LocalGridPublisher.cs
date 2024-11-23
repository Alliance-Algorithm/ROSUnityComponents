using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ROS2;
using Unity.Robotics.ROSTCPConnector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(LocalGridMapBuilder))]
public class LocalGridPublisher : MonoBehaviour
{
    public string TopicName = "/sentry/nav/local/grid"; // Change this to your desired image topic\
    public string FrameId = "unity"; // Change this to your desired image topic
    public float fps = 10;
    public LocalGridMapBuilder builder;

    RosMessageTypes.Std.HeaderMsg header;

    RosMessageTypes.Nav.OccupancyGridMsg msg;


    ROSConnection ros;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        header = new() { frame_id = FrameId };
        // Initialize ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        builder = GetComponent<LocalGridMapBuilder>();
        builder.Build(out sbyte[,] ignore);
        ros.RegisterPublisher<RosMessageTypes.Nav.OccupancyGridMsg>(TopicName);

        msg = new()
        {
            header = header,
            data = new sbyte[ignore.Length]
        };

        StartCoroutine(Publisher());
    }
    IEnumerator Publisher()
    {
        while (true)
        {
            yield return new WaitForSeconds(1 / fps);
            builder.Build(out sbyte[,] ignore);
            // builder.MakeESDF(ref ignore);
            msg.info = new()
            {
                width = (uint)ignore.GetLength(0),
                height = (uint)ignore.GetLength(1),
                resolution = builder.Resolution,
                origin = new()
                {
                    // Position = new() { X = transform.position.x, Y = transform.position.z },
                    orientation = new() { x = 0, y = 0, z = 0, w = 1 }
                }
            };
            Buffer.BlockCopy(ignore, 0, msg.data, 0, ignore.Length);
            ros.Publish(TopicName, msg);
        }
    }
}
