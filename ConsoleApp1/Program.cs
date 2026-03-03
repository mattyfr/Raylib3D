using System.Numerics;
using ConsoleApp1;
using Raylib_cs;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Runtime.InteropServices;

int targetFps = 60;
int framesSinceLastShoot = 0;
int bulletsInMag = 0;
float reloadCooldown = 0;
List<Bullet> bulletList = [];
List<Blocks> blockList = [];
List<Blocks> room = [];
List<Room> rooms = [];
Raylib.InitWindow(Raylib.GetScreenWidth(), Raylib.GetScreenHeight(), "title");
Raylib.DisableCursor();
Raylib.ToggleFullscreen();
Raylib.SetTargetFPS(targetFps);
Camera3D camera3DMain = Camera();
Camera2D camera2DMain = Camera2D();
Wepond ChoosenWepond = new Wepond();
AK ak47 = new AK();
AWP awp = new AWP();
ShootGun shootGun = new ShootGun();
// if (true) 
// {
//     room = 
//     [
//     new Blocks{posX = 0, posY = -2, posZ = 50, pos = new Vector3(0, -2, 50), height = 1, length = 100, width = 100, isColor = false, color = Shade(new Vector3(0, -2, 50))},
//     new Blocks{posX = 0, posY = 23, posZ = 0, pos = new Vector3(0, 23, 0), height = 50, length = 1, width = 100, isColor = false, color = Shade(new Vector3(0, 23, 0))},
//     new Blocks{posX = 0, posY = 23, posZ = 100, pos = new Vector3(0, 23, 100), height = 50, length = 1, width = 100, isColor = false, color = Shade(new Vector3(0, 23, 100))},
//     new Blocks{posX = 50, posY = 23, posZ = 22.5f, pos = new Vector3(50, 23, 22.5f), height = 50, length = 45, width = 1, isColor = false, color = Shade(new Vector3(50, 23, 22.5f))},
//     new Blocks{posX = 50, posY = 23, posZ = 77.5f, pos = new Vector3(50, 23, 77.5f), height = 50, length = 45, width = 1, isColor = false, color = Shade(new Vector3(50, 23, 77.5f))},
//     new Blocks{posX = 50, posY = 28, posZ = 50, pos = new Vector3(50, 28, 50), height = 40, length = 10, width = 1, isColor = false, color = Shade(new Vector3(50, 28, 50))},
//     new Blocks{posX = -50, posY = 23, posZ = 22.5f, pos = new Vector3(-50, 23, 22.5f), height = 50, length = 45, width = 1, isColor = false, color = Shade(new Vector3(-50, 23, 22.5f))},
//     new Blocks{posX = -50, posY = 23, posZ = 77.5f, pos = new Vector3(-50, 23, 77.5f), height = 50, length = 45, width = 1, isColor = false, color = Shade(new Vector3(-50, 23, 77.5f))},
//     new Blocks{posX = -50, posY = 28, posZ = 50, pos = new Vector3(-50, 28, 50), height = 40, length = 10, width = 1, isColor = false, color = Shade(new Vector3(-50, 28, 50))},
//     ];
//     string Json = JsonSerializer.Serialize<List<Blocks>>(room);
//     File.WriteAllText("room.txt", Json);
// }
LoadRoomFromJson(room);
while (!Raylib.WindowShouldClose())
{
    Raylib.BeginDrawing();
    Draw3D(camera3DMain, blockList, bulletList, rooms, room);
    Draw2D(rooms, bulletList, ChoosenWepond);
    camera3DMain = Movement(camera3DMain);
    LookAround();
    (framesSinceLastShoot,bulletsInMag) = Shoot(ChoosenWepond, camera3DMain, framesSinceLastShoot, bulletsInMag, bulletList, reloadCooldown);
    bulletList = BulletController(bulletList, camera3DMain);
    CheckForCollisions(bulletList, rooms);
    reloadCooldown = ChangeWepond(ChoosenWepond, ak47, awp, reloadCooldown);
    rooms = CreateRooms(rooms, camera3DMain, room);
    Raylib.EndDrawing();
    framesSinceLastShoot++;
}
static Camera3D Camera()
{
    Camera3D camera = new Camera3D();
    camera.Position = new Vector3(0, 0, 0);
    camera.Up = new Vector3(0f, 1f, 0f);
    camera.Target = new Vector3(0f, 0f, 1f);
    camera.FovY = 90f;
    return camera;
}
static Camera2D Camera2D()
{
    Camera2D camera = new Camera2D();
    return camera;
}
static void Draw3D(Camera3D camera3DMain, List<Blocks> blockList, List<Bullet> bulletList, List<Room> rooms, List<Blocks> room)
{
    Raylib.BeginMode3D(camera3DMain);
    Raylib.ClearBackground(Color.SkyBlue);
    foreach (var item in blockList)
    {
        Raylib.DrawCube(item.pos, item.width, item.height, item.length, Shade(item.pos, camera3DMain));
    }
    DrawRooms(rooms, camera3DMain);
    DrawBullets(bulletList);
    Raylib.EndMode3D();
}
static void Draw2D(List<Room> rooms, List<Bullet> bulletList, Wepond ChoosenWepond)
{
    DrawCrossHair();
    DrawWepondInfo(ChoosenWepond);
    Raylib.DrawText("Text", 100, 100, 10, Color.Red);
    Raylib.DrawFPS(150, 150);
    Raylib.DrawText(@$"{rooms.Count()}", 200,200,20, Color.Red);
    Raylib.DrawText(@$"{bulletList.Count()}", 250,250,20, Color.Red);
}
static Camera3D Movement(Camera3D camera3DMain)
{
    if (Raylib.IsKeyDown(KeyboardKey.W))
    {
        Raylib.CameraMoveForward(ref camera3DMain, 1f, true);
    }
    if (Raylib.IsKeyDown(KeyboardKey.S))
    {
        Raylib.CameraMoveForward(ref camera3DMain, -1f, true);
    }
    if (Raylib.IsKeyDown(KeyboardKey.A))
    {
        Raylib.CameraMoveRight(ref camera3DMain, -1f, true);
    }
    if (Raylib.IsKeyDown(KeyboardKey.D))
    {
        Raylib.CameraMoveRight(ref camera3DMain, 1f, true);
    }
    return camera3DMain;
}
void LookAround()
{
    Vector2 mouseMovement = Raylib.GetMouseDelta();
    Raylib.CameraYaw(ref camera3DMain, mouseMovement.X / -360, true);
    Raylib.CameraPitch(ref camera3DMain, mouseMovement.Y / -360, true, true, false);
    Console.WriteLine(camera3DMain.Target);
}
static (int,int) Shoot(Wepond ChoosenWepond, Camera3D camera3DMain, int framesSinceLastShoot, int bulletsInMag, List<Bullet> bulletList, float reloadCooldown)
{
    if (Raylib.IsMouseButtonDown(MouseButton.Left))
    {
        if (framesSinceLastShoot >= Raylib.GetFPS() / ChoosenWepond.cooldown && ChoosenWepond.bulletsInMag > 0)
        {
            for(int i = 0; i < ChoosenWepond.bulletsPerShoot; i++)
            {
                Bullet bullet = new Bullet(); bullet.pos = camera3DMain.Position; bullet.path = Raylib.GetCameraForward(ref camera3DMain) + GetBulletAccuracy(ChoosenWepond); bullet.rotationAngle = 0;
                bulletList.Add(bullet);
            }
            framesSinceLastShoot = 0;
            bulletsInMag--;
        }
    }
    if (ChoosenWepond.bulletsInMag == 0)
    {
        reloadCooldown = Reload(reloadCooldown, ChoosenWepond);
    }
    return (framesSinceLastShoot,bulletsInMag);
}
static List<Bullet> BulletController(List<Bullet> bulletList, Camera3D camera3DMain)
{
    foreach (var item in bulletList)
    {
        item.pos += item.path * 4;
    }
    bulletList = RemoveFarAwayBullets(bulletList, camera3DMain);
    return bulletList;
}
static void DrawRooms(List<Room> rooms, Camera3D camera3DMain)
{
    for(int roomNumber = 0; roomNumber < rooms.Count(); roomNumber++)
    {
        DrawRoomStructure(roomNumber, rooms, camera3DMain);
        DrawEnemies(roomNumber, rooms);
    }
}
static void DrawRoomStructure(int roomNumber, List<Room> rooms, Camera3D camera3DMain)
{
    foreach (var item in rooms[roomNumber].roomStructure)
    {
        Vector3 currentPos = new Vector3(item.pos.X + roomNumber * 100, item.pos.Y, item.pos.Z);
        if (!item.isColor)
        {
            item.color = Shade(currentPos, camera3DMain);
        }
        else{}
        Raylib.DrawCube(currentPos, item.width, item.height, item.length, item.color);
    }
}
static void DrawEnemies(int roomNumber, List<Room> rooms)
{
    foreach (var item in rooms[roomNumber].enenmies)
    {
        Vector3 currentPos = new Vector3(item.pos.X + roomNumber * 100, item.pos.Y, item.pos.Z);
        Raylib.DrawCircle3D(currentPos, 1, new Vector3(0, 0, 0), 0, Color.Blue);
    }
}
static void DrawBullets(List<Bullet> bulletList)
{
    foreach (var item in bulletList)
    {
        Raylib.DrawCircle3D(item.pos, 0.05f, Vector3.Zero, item.rotationAngle, Color.Yellow);
    }
}
static Vector3 GetEnemiesPos()
{
    return new Vector3(Random.Shared.Next(0, 50), 0, Random.Shared.Next(0, 100));
}
static void CheckForCollisions(List<Bullet> bulletList, List<Room> rooms)
{

    for (int i = 0; i < bulletList.Count(); i++)
    {
        for (int j = 0; j < rooms.Count(); j++)
        {
            for (int k = 0; k < rooms[j].enenmies.Count(); k++)
            {
                Vector3 enemyPos = new Vector3(rooms[j].enenmies[k].pos.X + 100 * j, rooms[j].enenmies[k].pos.Y, rooms[j].enenmies[k].pos.Z);
                if (Raylib.CheckCollisionSpheres(bulletList[i].pos, 0.5f, enemyPos, 1f))
                {
                    bulletList.RemoveAt(i--);
                    rooms[j].enenmies.RemoveAt(k);
                    break;
                }
            }
        }
    }
}
static List<Room> CreateRooms(List<Room> rooms, Camera3D camera3DMain, List<Blocks> room)
{
    if (DisatanceToLastRoom(rooms, camera3DMain) >= -200)
    {
        rooms.Add(new Room { roomStructure = room, enenmies = [new Enemy{pos = GetEnemiesPos()}, new Enemy{pos = GetEnemiesPos()}, new Enemy{pos = GetEnemiesPos()}] });
    }
    return rooms;
}
static int DisatanceToLastRoom(List<Room> rooms, Camera3D camera3DMain)
{
    int roomAmount = rooms.Count();
    return (int)camera3DMain.Position.X - roomAmount * 100;
}
static int DisatanceBetweenObjects(Vector3 pos1, Vector3 pos2)
{
    double x1 = (int)pos1.X;
    double y1 = (int)pos1.Z;
    double z1 = (int)pos1.Y;
    double x2 = (int)pos2.X;
    double y2 = (int)pos2.Z;
    double z2 = (int)pos2.Y; 
    double distance = Math.Sqrt((x1 * x1) + (y1 * 1) + (z1 + z1)) - Math.Sqrt((x2 * x2) + (y2 * y2) + (z2 + z2));
    return (int)distance;
}
static Color Shade(Vector3 pos, Camera3D camera3DMain)
{
    pos -= camera3DMain.Position;
    double X = (int)pos.X;
    double Y = (int)pos.Z;
    double G = Math.Sqrt((X * X) + (Y * Y));
    if (G >= 255)
    {
        G = 255;
    }
    Color color = new Color((int)G, (int)G, (int)G);
    return color;
}
static List<Blocks> LoadRoomFromJson(List<Blocks> room)
{
    room = JsonSerializer.Deserialize<List<Blocks>>(File.ReadAllText("room.txt"));
    foreach (var item in room)
    {
        item.pos = new Vector3(item.posX, item.posY, item.posZ);
    }
    return room;
}
static float ChangeWepond(Wepond ChoosenWepond, AK ak47, AWP awp, float reloadCooldown)
{
    if (Raylib.IsKeyDown(KeyboardKey.Z))
    {
        ChoosenWepond.wepondName = ak47.wepondName;
        ChoosenWepond.cooldown = ak47.cooldown;
        ChoosenWepond.Accuracy = ak47.Accuracy;
        ChoosenWepond.bulletsPerShoot = ak47.bulletsPerShoot;
        ChoosenWepond.magSize = ak47.magSize;
        ChoosenWepond.reloadTime = ak47.reloadTime;
        ChoosenWepond.damage = ak47.damage;
        reloadCooldown = 0;
    }
    if (Raylib.IsKeyDown(KeyboardKey.X))
    {
        ChoosenWepond.wepondName = awp.wepondName;
        ChoosenWepond.cooldown = awp.cooldown;
        ChoosenWepond.Accuracy = awp.Accuracy;
        ChoosenWepond.bulletsPerShoot = awp.bulletsPerShoot;
        ChoosenWepond.magSize = awp.magSize;
        ChoosenWepond.reloadTime = awp.reloadTime;
        ChoosenWepond.damage = awp.damage;
        reloadCooldown = 0;
    }
    if (Raylib.IsKeyDown(KeyboardKey.C))
    {
        ChoosenWepond.wepondName = "ShootGun";
        ChoosenWepond.cooldown = 7;
        ChoosenWepond.Accuracy = 0.05f;
        ChoosenWepond.bulletsPerShoot = 5;
        ChoosenWepond.magSize = 7;
        ChoosenWepond.reloadTime = 0.235f;
        reloadCooldown = 0;
    }
    return reloadCooldown;
}
static Vector3 GetBulletAccuracy(Wepond ChoosenWepond)
{
    Vector3 offsetVector = new Vector3(Random.Shared.Next(-5, 6),Random.Shared.Next(-5, 6),Random.Shared.Next(-5, 6));
    return offsetVector / 20 * ChoosenWepond.Accuracy;
}
static float Reload(float reloadCooldown, Wepond ChoosenWepond)
{
    if (reloadCooldown == 0 && ChoosenWepond.bulletsInMag == 0)
    {
        reloadCooldown = Raylib.GetFPS() / ChoosenWepond.reloadTime;
    }
    else
    {
        reloadCooldown--;
        Raylib.DrawText("Reloading", (int)Raylib.GetScreenCenter().X,(int)Raylib.GetScreenCenter().Y, 30, Color.Red);
    }
    if (reloadCooldown <= 2)
    {
        ChoosenWepond.bulletsInMag = ChoosenWepond.magSize;
        reloadCooldown = 0;
    }
    return reloadCooldown;
}
static void DrawCrossHair()
{
    Raylib.DrawCircle((int)Raylib.GetScreenCenter().X, (int)Raylib.GetScreenCenter().Y, 1, Color.Red);
}
static void DrawWepondInfo(Wepond ChoosenWepond)
{
    Raylib.DrawText(@$"{ChoosenWepond.wepondName}", (int)(Raylib.GetScreenCenter().X * 1.5f), (int)(Raylib.GetScreenCenter().Y * 1.5f), 20, Color.Pink);
    Raylib.DrawText(@$"{ChoosenWepond.bulletsInMag} / {ChoosenWepond.magSize}", (int)(Raylib.GetScreenCenter().X * 1.5f), (int)(Raylib.GetScreenCenter().Y * 1.55f), 20, Color.Pink);
}
static List<Bullet> RemoveFarAwayBullets(List<Bullet> bulletList, Camera3D camera3DMain){
    for(int i = 0; i > bulletList.Count; i++)
    {
        if (Math.Abs(DisatanceBetweenObjects(camera3DMain.Position, bulletList[i].pos)) <= 150)
        {
            bulletList.RemoveAt(i);
        }
    }
    return bulletList;
} 
static void Log(string text)
{
    if(!File.Exists("Log.txt"))
    {
        var log = File.Create("Log.txt");
        log.Close();
    }
}