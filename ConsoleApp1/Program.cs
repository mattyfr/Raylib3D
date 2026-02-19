using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using ConsoleApp1;
using Raylib_cs;

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
if (true)  // Generic room structure
{
    Blocks floor = new Blocks(); floor.pos = new Vector3(0, -2, 50); floor.height = 1; floor.length = 100; floor.width = 100; floor.isColor = false; floor.color = Shade(floor.pos);
    Blocks wall1 = new Blocks(); wall1.pos = new Vector3(0, 23, 0); wall1.height = 50; wall1.length = 1; wall1.width = 100; wall1.isColor = false; wall1.color = Shade(wall1.pos);
    Blocks wall2 = new Blocks(); wall2.pos = new Vector3(0, 23, 100); wall2.height = 50; wall2.length = 1; wall2.width = 100; wall2.isColor = false; wall2.color = Shade(wall2.pos);
    Blocks door21 = new Blocks(); door21.pos = new Vector3(50, 23, 22.5f); door21.height = 50; door21.length = 45; door21.width = 1; door21.isColor = false; door21.color = Shade(door21.pos);
    Blocks door22 = new Blocks(); door22.pos = new Vector3(50, 23, 77.5f); door22.height = 50; door22.length = 45; door22.width = 1; door22.isColor = false; door22.color = Shade(door22.pos);
    Blocks door23 = new Blocks(); door23.pos = new Vector3(50, 28, 50); door23.height = 40; door23.length = 10; door23.width = 1; door23.isColor = false; door23.color = Shade(door23.pos);
    Blocks door1 = new Blocks(); door1.pos = new Vector3(-50, 23, 22.5f); door1.height = 50; door1.length = 45; door1.width = 1; door1.isColor = false; door1.color = Shade(door1.pos);
    Blocks door2 = new Blocks(); door2.pos = new Vector3(-50, 23, 77.5f); door2.height = 50; door2.length = 45; door2.width = 1; door2.isColor = false; door2.color = Shade(door2.pos);
    Blocks door3 = new Blocks(); door3.pos = new Vector3(-50, 28, 50); door3.height = 40; door3.length = 10; door3.width = 1; door3.isColor = false; door3.color = Shade(door3.pos);
    room.Add(floor);
    room.Add(wall1);
    room.Add(wall2);
    room.Add(door1);
    room.Add(door2);
    room.Add(door3);
    room.Add(door21);
    room.Add(door22);
    room.Add(door23);
}
GetEnemiesPos();
CreateRooms();
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
    Raylib.DrawText("Text", 100, 100, 10, Color.Red);
    Raylib.DrawFPS(150, 150);
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
        if (framesSinceLastShoot >= targetFps / 12)
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
        item.pos += item.path;
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
        else
        {
        }
        Raylib.DrawCube(currentPos, item.width, item.height, item.length, item.color);
    }
}
void DrawEnemies(int roomNumber)
{
    foreach (var item in rooms[roomNumber].enenmies)
    {
        Vector3 currentPosStart = new Vector3(item.pos.X + roomNumber * 100, item.pos.Y, item.pos.Z);
        Vector3 currentPosEnd = new Vector3(currentPosStart.X, 2, currentPosStart.Z);
        Raylib.DrawCircle3D(currentPosStart, 1, new Vector3(0, 0, 0), 0, Color.Blue);
    }
}
void DrawBullets()
{
    foreach (var item in bulletList)
    {
        Raylib.DrawCircle3D(item.pos, 1, Vector3.Zero, item.rotationAngle, Color.Yellow);
    }
}
void GetEnemiesPos()
{
    for (int i = 0; i < rooms.Count(); i++)
    {
        foreach (var item in rooms[i].enenmies)
        {
            item.pos = new Vector3(Random.Shared.Next(0, 100), 0, Random.Shared.Next(0, 100));
        }
    }

}
void CheckForCollisions()
{

    for (int i = 0; i < bulletList.Count(); i++)
    {
        for (int j = 0; j < rooms.Count(); j++)
        {
            for (int k = 0; k < rooms[j].enenmies.Count(); k++)
            {
                if (bulletList[i].pos.X + 0.5 >= rooms[j].enenmies[k].pos.X && bulletList[i].pos.X - 0.5 <= rooms[j].enenmies[k].pos.X && bulletList[i].pos.Y + 0.5 >= rooms[j].enenmies[k].pos.Y && bulletList[i].pos.Y - 0.5 <= rooms[j].enenmies[k].pos.Y && bulletList[i].pos.Y + 0.5 >= rooms[j].enenmies[k].pos.Y && bulletList[i].pos.Y - 0.5 <= rooms[j].enenmies[k].pos.Y)
                {
                    bulletList.RemoveAt(i);
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
        rooms.Add(new Room { roomStructure = room, enenmies = [new Enemy(), new Enemy(), new Enemy()] });
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
