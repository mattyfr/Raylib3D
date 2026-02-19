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
    public bool isColor;
    public Color color;
}
public class Bullet
{
    public Vector3 pos;
    public Vector3 path; 
    public float rotationAngle;
}
public class Enemy()
{
    public Vector3 pos;
    public Color color;
}
public class Room
{
    public List<Blocks> roomStructure;
    public List<Enemy> enenmies;
}