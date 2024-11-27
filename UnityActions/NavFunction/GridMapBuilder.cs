using System;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class GridMapBuilder : MonoBehaviour
{
    public int SizeX = 15;
    public int SizeY = 28;

    internal Vector3 XY2Vector3(int x, int y)
    {
        return (x - SizeX / 2.0f) * 15.0f / SizeX * Vector3.right + (y - SizeY / 2.0f) * 28.0f / SizeY * Vector3.forward + 3 * Vector3.up;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Build(out sbyte[,] Map, float Resolution)
    {
        SizeX = (int)(SizeX / Resolution);
        SizeY = (int)(SizeY / Resolution);

        Map = new sbyte[SizeX, SizeY];

        for (int i = 0; i < SizeX; i++)
        {

            for (int j = 0; j < SizeY; j++)
            {
                if (Physics.Raycast(XY2Vector3(i, j), Vector3.down, out RaycastHit hit, 6))
                {
                    if (NavMesh.SamplePosition(hit.point, out NavMeshHit ignore, 0.03f, NavMesh.AllAreas))
                    {
                        Map[i, j] = 100;
                    }
                }
                Map[i, j] = 0;
            }
        }

        SizeX = 15;
        SizeY = 28;
    }
}
