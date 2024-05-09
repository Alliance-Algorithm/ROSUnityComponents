using System;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class MapBuilder : MonoBehaviour
{
    public int SizeX = 1500;
    public int SizeY = 2800;
    internal Vector3 XY2Vector3(int x, int y)
    {
        return ((x - SizeX / 2.0f) * 15.0f / SizeX) * Vector3.right + ((y - SizeY / 2.0f) * 28.0f / SizeY) * Vector3.forward + 3 * Vector3.up;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string head = string.Format("uint8_t GlobalMap[{0,10}][{1,10}] = ", SizeX, SizeY) + '{';
        for (int i = 0; i < SizeX; i++)
        {
            head += "{";

            for (int j = 0; j < SizeY; j++)
            {
                if (Physics.Raycast(XY2Vector3(i, j), Vector3.down, out RaycastHit hit, 6))
                {
                    if (NavMesh.SamplePosition(hit.point, out NavMeshHit ignore, 0.3f, NavMesh.AllAreas))
                    {
                        head += "0";
                        if (j != SizeY - 1)
                            head += ",";
                        continue;
                    }
                }
                head += "1";
                if (j != SizeY - 1)
                    head += ",";
            }

            head += "}";
            if (i != SizeX - 1)
                head += ",\n";
        }
        head += "};\n";

        File.WriteAllText("./Build/Map/GlobalMap.hpp", head);
    }
}
