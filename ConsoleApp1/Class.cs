using System;
using Raylib_cs;
using System.Numerics;
using System.Runtime.InteropServices;

namespace ConsoleApp1;

public class Blocks
{
    public Vector3 pos;
    public float width;
    public float height;
    public float length;
    public Color color;
}
public class Bullet
{
    public Vector3 pos;
    public Vector3 path; 
    public float rotationAngle;
}