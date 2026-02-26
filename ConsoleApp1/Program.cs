using System.Numerics;
using ConsoleApp1;
using Raylib_cs;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Runtime.InteropServices;

int targetFps = 60;
int framesSinceLastShoot = 0;
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
if (true) 
{
    room = 
    [
    new Blocks{posX = 0, posY = -2, posZ = 50, pos = new Vector3(0, -2, 50), height = 1, length = 100, width = 100, isColor = false, color = Shade(new Vector3(0, -2, 50))},
    new Blocks{posX = 0, posY = 23, posZ = 0, pos = new Vector3(0, 23, 0), height = 50, length = 1, width = 100, isColor = false, color = Shade(new Vector3(0, 23, 0))},
    new Blocks{posX = 0, posY = 23, posZ = 100, pos = new Vector3(0, 23, 100), height = 50, length = 1, width = 100, isColor = false, color = Shade(new Vector3(0, 23, 100))},
    new Blocks{posX = 50, posY = 23, posZ = 22.5f, pos = new Vector3(50, 23, 22.5f), height = 50, length = 45, width = 1, isColor = false, color = Shade(new Vector3(50, 23, 22.5f))},
    new Blocks{posX = 50, posY = 23, posZ = 77.5f, pos = new Vector3(50, 23, 77.5f), height = 50, length = 45, width = 1, isColor = false, color = Shade(new Vector3(50, 23, 77.5f))},
    new Blocks{posX = 50, posY = 28, posZ = 50, pos = new Vector3(50, 28, 50), height = 40, length = 10, width = 1, isColor = false, color = Shade(new Vector3(50, 28, 50))},
    new Blocks{posX = -50, posY = 23, posZ = 22.5f, pos = new Vector3(-50, 23, 22.5f), height = 50, length = 45, width = 1, isColor = false, color = Shade(new Vector3(-50, 23, 22.5f))},
    new Blocks{posX = -50, posY = 23, posZ = 77.5f, pos = new Vector3(-50, 23, 77.5f), height = 50, length = 45, width = 1, isColor = false, color = Shade(new Vector3(-50, 23, 77.5f))},
    new Blocks{posX = -50, posY = 28, posZ = 50, pos = new Vector3(-50, 28, 50), height = 40, length = 10, width = 1, isColor = false, color = Shade(new Vector3(-50, 28, 50))},
    ];
    string Json = JsonSerializer.Serialize<List<Blocks>>(room);
    File.WriteAllText("room.txt", Json);
}
LoadRoomFromJson();
while (!Raylib.WindowShouldClose())
{
    Raylib.BeginDrawing();
    Draw3D();
    Draw2D();
    Movement();
    LookAround();
    Shoot();
    BulletController();
    CheckForCollisions();
    ChangeWepond();
    Raylib.EndDrawing();
    framesSinceLastShoot++;
}
Camera3D Camera()
{
    Camera3D camera = new Camera3D();
    camera.Position = new Vector3(0, 0, 0);
    camera.Up = new Vector3(0f, 1f, 0f);
    camera.Target = new Vector3(0f, 0f, 1f);
    camera.FovY = 90f;
    return camera;
}
Camera2D Camera2D()
{
    Camera2D camera = new Camera2D();
    return camera;
}
void Draw3D()
{
    Raylib.BeginMode3D(camera3DMain);
    Raylib.ClearBackground(Color.SkyBlue);
    foreach (var item in blockList)
    {
        Raylib.DrawCube(item.pos, item.width, item.height, item.length, Shade(item.pos));
    }
    DrawRooms();
    DrawBullets();
    Raylib.EndMode3D();
}
void Draw2D()
{
    Raylib.DrawCircle((int)Raylib.GetScreenCenter().X, (int)Raylib.GetScreenCenter().Y, 5, Color.Red);
    Raylib.DrawText("Text", 100, 100, 10, Color.Red);
    Raylib.DrawFPS(150, 150);
    Raylib.DrawText(@$"{rooms.Count()}", 200,200,20, Color.Red);
    foreach(var item in rooms)
    {
        Raylib.DrawText(@$"{item.enenmies.Count()}", 220,220,20, Color.RayWhite);
    }
    Raylib.DrawText(@$"{bulletList.Count()}",240,240,20,Color.Red);
    Raylib.DrawText(@$"{framesSinceLastShoot}",260,260,20,Color.Red);
    Raylib.DrawText(@$"{ChoosenWepond.cooldown}",280,280,20,Color.Red);
    Raylib.DrawText(@$"{shootGun.cooldown}",300,300,20,Color.Red);
    
}
void Movement()
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
}
void LookAround()
{
    Vector2 mouseMovement = Raylib.GetMouseDelta();
    Raylib.CameraYaw(ref camera3DMain, mouseMovement.X / -360, true);
    Raylib.CameraPitch(ref camera3DMain, mouseMovement.Y / -360, true, true, false);
    Console.WriteLine(camera3DMain.Target);
}
void Shoot()
{
    if (Raylib.IsMouseButtonDown(MouseButton.Left))
    {
        if (framesSinceLastShoot >= targetFps / ChoosenWepond.cooldown)
        {
            Bullet bullet = new Bullet(); bullet.pos = camera3DMain.Position; bullet.path = Raylib.GetCameraForward(ref camera3DMain); bullet.rotationAngle = 0;
            bulletList.Add(bullet);
            framesSinceLastShoot = 0;
        }
    }
}
void BulletController()
{
    foreach (var item in bulletList)
    {
        item.pos += item.path * 4;
    }
}
void DrawRooms()
{
    CreateRooms();
    for(int roomNumber = 0; roomNumber < rooms.Count(); roomNumber++)
    {
        DrawRoomStructure(roomNumber);
        DrawEnemies(roomNumber);
    }
}
void DrawRoomStructure(int roomNumber)
{
    foreach (var item in rooms[roomNumber].roomStructure)
    {
        Vector3 currentPos = new Vector3(item.pos.X + roomNumber * 100, item.pos.Y, item.pos.Z);
        if (!item.isColor)
        {
            item.color = Shade(currentPos);
        }
        else{}
        Raylib.DrawCube(currentPos, item.width, item.height, item.length, item.color);
    }
}
void DrawEnemies(int roomNumber)
{
    foreach (var item in rooms[roomNumber].enenmies)
    {
        Vector3 currentPos = new Vector3(item.pos.X + roomNumber * 100, item.pos.Y, item.pos.Z);
        Raylib.DrawCircle3D(currentPos, 1, new Vector3(0, 0, 0), 0, Color.Blue);
    }
}
void DrawBullets()
{
    foreach (var item in bulletList)
    {
        Raylib.DrawCircle3D(item.pos, 0.05f, Vector3.Zero, item.rotationAngle, Color.Yellow);
    }
}
Vector3 GetEnemiesPos()
{
    return new Vector3(Random.Shared.Next(0, 50), 0, Random.Shared.Next(0, 100));
}
void CheckForCollisions()
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
                }
            }
        }
    }
}
void CreateRooms()
{
    if (DisatanceToLastRoom() >= -200)
    {
        rooms.Add(new Room { roomStructure = room, enenmies = [new Enemy{pos = GetEnemiesPos()}, new Enemy{pos = GetEnemiesPos()}, new Enemy{pos = GetEnemiesPos()}] });
    }
}
int DisatanceToLastRoom()
{
    int roomAmount = rooms.Count();
    return (int)camera3DMain.Position.X - roomAmount * 100;
}
Color Shade(Vector3 pos)
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
void LoadRoomFromJson()
{
    room = JsonSerializer.Deserialize<List<Blocks>>(File.ReadAllText("room.txt"));
    foreach (var item in room)
    {
        item.pos = new Vector3(item.posX, item.posY, item.posZ);
    }
}
void ChangeWepond()
{
    if (Raylib.IsKeyDown(KeyboardKey.Z))
    {
        ChoosenWepond.cooldown = 10;
        ChoosenWepond.Accuracy = 0.035f;
        ChoosenWepond.bulletsPerShoot = 1;
        ChoosenWepond.magSize = 30;
        ChoosenWepond.reloadTime = 0.125f;
        ChoosenWepond.damage = 51;
        
    }
    if (Raylib.IsKeyDown(KeyboardKey.X))
    {
        ChoosenWepond.cooldown = 0.75f;
        ChoosenWepond.Accuracy = 0;
        ChoosenWepond.bulletsPerShoot = 1;
        ChoosenWepond.magSize = 5;
        ChoosenWepond.reloadTime = 0.15f;
        ChoosenWepond.damage = 101;
    }
    if (Raylib.IsKeyDown(KeyboardKey.C))
    {
        ChoosenWepond.cooldown = 7;
        ChoosenWepond.Accuracy = 0.05f;
        ChoosenWepond.bulletsPerShoot = 5;
        ChoosenWepond.magSize = 7;
        ChoosenWepond.reloadTime = 0.135f;
    }
}