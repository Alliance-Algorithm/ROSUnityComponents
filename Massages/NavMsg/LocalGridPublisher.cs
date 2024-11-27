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
    public Transform beginPoint;
    public LocalGridMapBuilder builder;

    RosMessageTypes.Std.HeaderMsg header;

    RosMessageTypes.Nav.OccupancyGridMsg msg;

    Vector3 begin;


    ROSConnection ros;
    public bool isGloabl = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!isGloabl)
            begin = transform.position;
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

        beginPoint.localPosition = new(-builder.BoxSize / 2.0f, 0, -builder.BoxSize / 2.0f);

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
                width = (uint)ignore.GetLength(1),
                height = (uint)ignore.GetLength(0),
                resolution = builder.Resolution,
                origin = new()
                {
                    position = new() { x = -begin.x + beginPoint.position.x, y = -begin.z + beginPoint.position.z },
                    orientation = new() { x = -transform.rotation.x, y = -transform.rotation.z, z = -transform.rotation.y, w = transform.rotation.w }
                }
            };
            for (int i = 0; i < msg.info.height; i++)
            {
                for (int j = 0; j < msg.info.width; j++)
                {
                    msg.data[j + i * msg.info.width] = ignore[j, i];
                }
            }
            ros.Publish(TopicName, msg);
        }
    }
}
