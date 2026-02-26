using System;
using Raylib_cs;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConsoleApp1;

public class Blocks
{
    public Vector3 pos;
    [JsonInclude]
    public float posX;
    [JsonInclude]
    public float posY;
    [JsonInclude]
    public float posZ;
    [JsonInclude]
    public float width;
    [JsonInclude]
    public float height;
    [JsonInclude]
    public float length;
    [JsonInclude]
    public bool isColor;
    [JsonInclude]
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
public class Wepond
{
    public float cooldown;
    public float magSize;
    public float reloadTime;
    public float bulletsPerShoot;
    public float Accuracy;
    public float damage;
}
public class AK
{
    public float cooldown = 10;
    public float magSize = 30;
    public float reloadTime = 0.125f;
    public float bulletsPerShoot = 1;
    public float Accuracy = 0.035f;
    public float damage = 51;
}
public class AWP
{
    public float cooldown = 2;
    public float magSize = 5;
    public float reloadTime = 0.15f;
    public float bulletsPerShoot = 1;
    public float Accuracy = 0;
    public float damage = 101;
}
public class ShootGun
{
    public float cooldown = 7;
    public float magSize = 7; 
    public float reloadTime = 0.135f;
    public float bulletsPerShoot = 5;
    public float Accuracy = 0.05f;
    public float damage = 32;
}