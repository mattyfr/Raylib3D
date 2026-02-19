using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;
using ConsoleApp1;
using Raylib_cs;

int targetFps = 60;
int framesSinceLastShoot = 0;
List<Bullet> bulletList = [];
List<Blocks> blockList = [];

Raylib.InitWindow(Raylib.GetScreenWidth(),Raylib.GetScreenHeight(),"title");
Raylib.DisableCursor();
Raylib.ToggleFullscreen();
Raylib.SetTargetFPS(targetFps);
Camera3D camera3DMain = Camera();
Camera2D camera2DMain = Camera2D();
if (true)
{
    Blocks floor = new Blocks(); floor.pos = new Vector3(0,-2,50); floor.height = 1; floor.length = 100; floor.width = 100; floor.color = Color.Gray;
    Blocks wall1 = new Blocks(); wall1.pos = new Vector3(0,23,0); wall1.height = 50; wall1.length = 1; wall1.width = 100; wall1.color = Color.Blue;
    Blocks wall2 = new Blocks(); wall2.pos = new Vector3(0,23,100); wall2.height = 50; wall2.length = 1; wall2.width = 100; wall2.color = Color.Black;
    Blocks door21 = new Blocks(); door21.pos = new Vector3(50,23,22.5f); door21.height = 50; door21.length = 45; door21.width = 1; door21.color = Color.Orange;
    Blocks door22 = new Blocks(); door22.pos = new Vector3(50,23,77.5f); door22.height = 50; door22.length = 45; door22.width = 1; door22.color = Color.Pink;
    Blocks door23 = new Blocks(); door23.pos = new Vector3(50,28,50); door23.height = 40; door23.length = 10; door23.width = 1; door23.color = Color.Purple;
    Blocks door1 = new Blocks(); door1.pos = new Vector3(-50,23,22.5f); door1.height = 50; door1.length = 45; door1.width = 1; door1.color = Color.Orange;
    Blocks door2 = new Blocks(); door2.pos = new Vector3(-50,23,77.5f); door2.height = 50; door2.length = 45; door2.width = 1; door2.color = Color.Pink;
    Blocks door3 = new Blocks(); door3.pos = new Vector3(-50,28,50); door3.height = 40; door3.length = 10; door3.width = 1; door3.color = Color.Purple;
    blockList.Add(floor);
    blockList.Add(wall1);
    blockList.Add(wall2);
    blockList.Add(door1);
    blockList.Add(door2);
    blockList.Add(door3);
    blockList.Add(door21);
    blockList.Add(door22);
    blockList.Add(door23);
}
while (!Raylib.WindowShouldClose())
{
    Raylib.BeginDrawing();
    Draw3D();
    Draw2D();
    Movement();
    LookAround();
    Shoot();
    BulletController();
    Raylib.EndDrawing();
    framesSinceLastShoot++
}
Camera3D Camera()
{
    Camera3D camera = new Camera3D();
    camera.Position = new Vector3(0,0,0);
    camera.Up = new Vector3(0f,1f,0f);
    camera.Target = new Vector3(0f,0f,1f);
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
    foreach(var item in blockList)
    {
        Raylib.DrawCube(item.pos, item.width, item.height, item.length, item.color);
    }
    foreach(var item in bulletList)
    {
        Raylib.DrawCircle3D(item.pos, 1, Vector3.Zero, item.rotationAngle, Color.Yellow);
    }
    Raylib.EndMode3D();
}
void Draw2D()
{
    Raylib.DrawText("Text", 100, 100, 10, Color.Red);
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
        if (framesSinceLastShoot = targetFps/12)
        {
            Bullet bullet = new Bullet(); bullet.pos = camera3DMain.Position; bullet.path = Raylib.GetCameraForward(ref camera3DMain); bullet.rotationAngle = 0;
            bulletList.Add(bullet);
            framesSinceLastShoot = 0;
        }
    }
}
void BulletController()
{
    foreach(var item in bulletList)
    {
        item.pos += item.path;
    }
}