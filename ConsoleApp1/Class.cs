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
    public float framesSinceFired;
}
public class Enemy()
{
    public Vector3 pos;
    public float hp;
    public Color color;
}
public class Room
{
    public List<Blocks> roomStructure;
    public List<Enemy> enenmies;
}
public class Wepond
{
    public string wepondName;
    public float cooldown;
    public int magSize;
    public int bulletsInMag;
    public int reloadTime;
    public float bulletsPerShoot;
    public float Accuracy;
    public float damage;
}
public class AK:Wepond
{
    [JsonInclude]
    public string wepondName = "Ak-67";
    [JsonInclude]
    public float cooldown = 10;
    [JsonInclude]
    public int magSize = 30;
    [JsonInclude]
    public int bulletsInMag = 30;
    [JsonInclude]
    public int reloadTime = 5;
    [JsonInclude]
    public float bulletsPerShoot = 1;
    [JsonInclude]
    public float Accuracy = 0.035f;
    [JsonInclude]
    public float damage = 51;
}
public class AWP
{
    public string wepondName = "AWP";
    public float cooldown = 2;
    public int magSize = 5;
    public int bulletsInMag = 5;
    public int reloadTime = 5;
    public float bulletsPerShoot = 1;
    public float Accuracy = 0;
    public float damage = 101;
}
public class ShootGun
{
    public string wepondName = "ShootGun";
    public float cooldown = 7;
    public int magSize = 7; 
    public int bulletsInMag = 7;
    public int reloadTime = 5;
    public float bulletsPerShoot = 5;
    public float Accuracy = 0.05f;
    public float damage = 32;
}