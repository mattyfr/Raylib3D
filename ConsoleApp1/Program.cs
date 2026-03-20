using System.Numerics;
using ConsoleApp1;
using Raylib_cs;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Runtime.InteropServices;

int targetFps = 60;
int framesSinceLastShoot = 0;
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
AWP awp = new AWP();
ShootGun shootGun = new ShootGun();

if (true)
{
    // AK ak67 = new AK();
    // ak67.wepondName = "ak67";
    // ak67.cooldown = 10;
    // ak67.magSize = 30;
    // ak67.bulletsInMag = 30;
    // ak67.reloadTime = 0.125f;
    // ak67.bulletsPerShoot = 1;
    // ak67.Accuracy = 0.035f;
    // ak67.damage = 51;

    // string Json = JsonSerializer.Serialize<AK>(ak67);
    // File.WriteAllText("..\\..\\..\\ak67.txt", Json);
}
room = LoadRoomFromJson(room);
while (!Raylib.WindowShouldClose())
{
    Raylib.BeginDrawing();
    Draw3D(camera3DMain, blockList, bulletList, rooms, room);
    Draw2D(rooms, bulletList, ChoosenWepond);
    Raylib.EndDrawing();
    camera3DMain = Movement(camera3DMain);
    LookAround();
    (framesSinceLastShoot, ChoosenWepond, reloadCooldown) = Shoot(ChoosenWepond, camera3DMain, framesSinceLastShoot, bulletList, reloadCooldown);
    bulletList = BulletController(bulletList, rooms, ChoosenWepond);
    (ChoosenWepond,reloadCooldown) = ChangeWepond(ChoosenWepond, reloadCooldown);
    rooms = CreateRooms(rooms, camera3DMain, room);
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
    Raylib.DrawText(@$"{rooms.Count()}", 200, 200, 20, Color.Red);
    Raylib.DrawText(@$"{bulletList.Count()}", 250, 250, 20, Color.Red);
    
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
static (int, Wepond, float) Shoot(Wepond ChoosenWepond, Camera3D camera3DMain, int framesSinceLastShoot, List<Bullet> bulletList, float reloadCooldown)
{
    if (Raylib.IsMouseButtonDown(MouseButton.Left))
    {
        if (framesSinceLastShoot >= Raylib.GetFPS() / ChoosenWepond.cooldown && ChoosenWepond.bulletsInMag > 0)
        {
            for (int i = 0; i < ChoosenWepond.bulletsPerShoot; i++)
            {
                Bullet bullet = new Bullet(); bullet.pos = camera3DMain.Position; bullet.path = Raylib.GetCameraForward(ref camera3DMain) + GetBulletAccuracy(ChoosenWepond); bullet.rotationAngle = 0; bullet.framesSinceFired = 0;
                bulletList.Add(bullet);
            }
            framesSinceLastShoot = 0;
            ChoosenWepond.bulletsInMag--;
        }
    }
    if (ChoosenWepond.bulletsInMag >= 0)
    {
        (reloadCooldown, ChoosenWepond) = Reload(reloadCooldown, ChoosenWepond);
    }
    return (framesSinceLastShoot, ChoosenWepond, reloadCooldown);
}
static List<Bullet> BulletController(List<Bullet> bulletList, List<Room> rooms, Wepond ChoosenWepond)
{
    for (int i = 0; i < 5; i++)
    {
        foreach (var item in bulletList)
        {
            item.pos += item.path;
            item.framesSinceFired += 0.25f;
        }
        (bulletList,rooms) = CheckForCollisionsBulletToEnemy(bulletList, rooms, ChoosenWepond);
        bulletList = RemoveFarAwayBullets(bulletList);
    }
    return bulletList;
}
static void DrawRooms(List<Room> rooms, Camera3D camera3DMain)
{
    for (int roomNumber = 0; roomNumber < rooms.Count(); roomNumber++)
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
        Raylib.DrawCube(currentPos, item.width, item.height, item.length, item.color);
    }
    if (!(rooms[roomNumber].enenmies.Count() == 0))
    {
        Raylib.DrawCube(new Vector3(50 + roomNumber * 100, 3, 50), 1, 10, 10, Color.Blue);
    }
}
static void DrawEnemies(int roomNumber, List<Room> rooms)
{
    foreach (var item in rooms[roomNumber].enenmies)
    {
        Vector3 currentPos = new Vector3(item.pos.X + roomNumber * 100, item.pos.Y, item.pos.Z);
        Raylib.DrawCircle3D(currentPos, 1, new Vector3(0, 0, 0), 0, Color.Blue);
        DrawEnemiesHP(item, currentPos);
    }
}
static void DrawEnemiesHP(Enemy item,Vector3 currenpos)
{
    Raylib.DrawCube(currenpos, 0.1f, 1f, item.hp/50f, Color.Black);
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
static (List<Bullet>, List<Room>) CheckForCollisionsBulletToEnemy(List<Bullet> bulletList, List<Room> rooms, Wepond choosenWepond)
{
    List<int> bulletsColidedIndex = [];
    List<(int,int)> enemiesHitIndex = [];
    for (int i = 0; i < bulletList.Count(); i++)
    {
        for (int j = 0; j < rooms.Count(); j++)
        {
            for (int k = 0; k < rooms[j].enenmies.Count(); k++)
            {
                Vector3 enemyPos = new Vector3(rooms[j].enenmies[k].pos.X + 100 * j, rooms[j].enenmies[k].pos.Y, rooms[j].enenmies[k].pos.Z);
                if (Raylib.CheckCollisionSpheres(bulletList[i].pos, 0.5f, enemyPos, 1f))
                {
                    bulletsColidedIndex.Add(i);
                    enemiesHitIndex.Add((j,k));
                }
            }
        }
    }
    List<int> bulletsToRemove = bulletsColidedIndex.Distinct().ToList();
    List<(int,int)>enemiesToRemove = enemiesHitIndex.Distinct().ToList();
    
    for(int i = 0; i < bulletsColidedIndex.Count(); i++)
    {
        Raylib.DrawText(@$"{bulletsColidedIndex[i]}", 250, 250 + 10*i, 20, Color.Red);
        bulletList = RemoveBullets(bulletList, i);
    }
    foreach(var item in enemiesToRemove)
    {
        if(rooms[item.Item1].enenmies[item.Item2].hp - choosenWepond.damage <= 0)
        {
            rooms[item.Item1].enenmies.RemoveAt(item.Item2);
        }
        else
        {
            rooms[item.Item1].enenmies[item.Item2].hp -= choosenWepond.damage;
        }
    }
    return (bulletList, rooms);
}
static List<Room> CreateRooms(List<Room> rooms, Camera3D camera3DMain, List<Blocks> room)
{
    if (DisatanceToLastRoom(rooms, camera3DMain) >= -200)
    {
        rooms.Add(new Room { roomStructure = room, enenmies = 
        [
        new Enemy { pos = GetEnemiesPos(), hp = 100}, 
        new Enemy { pos = GetEnemiesPos(), hp = 100}, 
        new Enemy { pos = GetEnemiesPos(), hp = 100}
        ] });
    }
    return rooms;
}
static int DisatanceToLastRoom(List<Room> rooms, Camera3D camera3DMain)
{
    int roomAmount = rooms.Count();
    return (int)camera3DMain.Position.X - roomAmount * 100;
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
    room = JsonSerializer.Deserialize<List<Blocks>>(File.ReadAllText("..\\..\\..\\room.txt"));
    foreach (var item in room)
    {
        item.pos = new Vector3(item.posX, item.posY, item.posZ);
    }
    return room;
}
static Wepond LoadWepondsFromJson(string name)
{
    Wepond wepond = JsonSerializer.Deserialize<Wepond>(File.ReadAllText($"..\\..\\..\\{name}.txt"));
    return wepond;
}
static (Wepond,float) ChangeWepond(Wepond ChoosenWepond, float reloadCooldown)  
{ 
    AK ak67 = JsonSerializer.Deserialize<AK>(File.ReadAllText("..\\..\\..\\weponds\\ak67.txt"));
    AWP awp = JsonSerializer.Deserialize<AWP>(File.ReadAllText("..\\..\\..\\weponds\\awp.txt"));
    ShootGun shootGun = JsonSerializer.Deserialize<ShootGun>(File.ReadAllText("..\\..\\..\\weponds\\shootgun.txt"));
    if (Raylib.IsKeyDown(KeyboardKey.Z))
    {
        ChoosenWepond.wepondName = ak67.wepondName;
        ChoosenWepond.cooldown = ak67.cooldown;
        ChoosenWepond.magSize = ak67.magSize;
        ChoosenWepond.bulletsInMag = ak67.bulletsInMag;
        ChoosenWepond.Accuracy = ak67.Accuracy;
        ChoosenWepond.bulletsPerShoot = ak67.bulletsPerShoot;
        ChoosenWepond.damage = ak67.damage;
        reloadCooldown = 0;
    }
    if (Raylib.IsKeyDown(KeyboardKey.X))
    {
        ChoosenWepond.wepondName = awp.wepondName;
        ChoosenWepond.cooldown = awp.cooldown;
        ChoosenWepond.magSize = awp.magSize;
        ChoosenWepond.bulletsInMag = awp.bulletsInMag;
        ChoosenWepond.Accuracy = awp.Accuracy;
        ChoosenWepond.bulletsPerShoot = awp.bulletsPerShoot;
        ChoosenWepond.damage = awp.damage;
        reloadCooldown = 0;
    }
    if (Raylib.IsKeyDown(KeyboardKey.C))
    {
        ChoosenWepond.wepondName = shootGun.wepondName;
        ChoosenWepond.cooldown = shootGun.cooldown;
        ChoosenWepond.magSize = shootGun.magSize;
        ChoosenWepond.bulletsInMag = shootGun.bulletsInMag;
        ChoosenWepond.Accuracy = shootGun.Accuracy;
        ChoosenWepond.bulletsPerShoot = shootGun.bulletsPerShoot;
        ChoosenWepond.damage = shootGun.damage;
        reloadCooldown = 0;
    }
    return (ChoosenWepond,reloadCooldown);
}
static Vector3 GetBulletAccuracy(Wepond ChoosenWepond)
{
    Vector3 offsetVector = new Vector3(Random.Shared.Next(-5, 6), Random.Shared.Next(-5, 6), Random.Shared.Next(-5, 6));
    return offsetVector / 20 * ChoosenWepond.Accuracy;
}
static (float, Wepond) Reload(float reloadCooldown, Wepond ChoosenWepond)
{
    if (reloadCooldown == 0 && ChoosenWepond.bulletsInMag == 0)
    {
        reloadCooldown = StartReload(ChoosenWepond);
    }
    else if (!(reloadCooldown == 0) && ChoosenWepond.bulletsInMag == 0)
    {
        reloadCooldown--;
        Raylib.DrawText("Reloading", (int)Raylib.GetScreenCenter().X, (int)Raylib.GetScreenCenter().Y, 30, Color.Red);
    }
    if (reloadCooldown < 2 && reloadCooldown > 0)
    {
        ChoosenWepond.bulletsInMag = ChoosenWepond.magSize;
        reloadCooldown = 0;
    }
    return (reloadCooldown, ChoosenWepond);
}
static float StartReload(Wepond ChoosenWepond)
{
    float reloadCooldown = Raylib.GetFPS() / ChoosenWepond.reloadTime;
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
static List<Bullet> RemoveFarAwayBullets(List<Bullet> bulletList)
{
    for(int i = 0; i < bulletList.Count(); i++)
    {
        if(bulletList[i].framesSinceFired >= Raylib.GetFPS() * 3)
        {
            bulletList = RemoveBullets(bulletList, i);
        }
    }
    return bulletList;
}
static List<Bullet> RemoveBullets(List<Bullet> bulletList, int index)
{
    bulletList.RemoveAt(index);
    return bulletList;
}

// unused functions
// static int DisatanceBetweenObjects(Vector3 pos1, Vector3 pos2)
// {
//     double x1 = (int)pos1.X;
//     double y1 = (int)pos1.Z;
//     double z1 = (int)pos1.Y;
//     double x2 = (int)pos2.X;
//     double y2 = (int)pos2.Z;
//     double z2 = (int)pos2.Y;
//     double distance = Math.Sqrt((x1 * x1) + (y1 * y1) + (z1 + z1)) - Math.Sqrt((x2 * x2) + (y2 * y2) + (z2 + z2));
//     return (int)distance;
// }
// static void Log(string text)
// {
//     if (!File.Exists(@"log.txt"))
//     {
//         var log = File.Create(@"\log.txt");
//         log.Close();
//     }
//     text = @$"{File.ReadAllText(@"log.txt")}{text}";
//     File.WriteAllText(@"log.txt", text);
// }