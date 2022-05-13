using AgarioShared;

namespace AgarioServer;

public class WorldManager{
    
    public static int maxPlayers = Server.maxClients;
    public static int maxOrbs = 50;
    
    public static PlayerInfo[] connectedPlayerArray = new PlayerInfo[maxPlayers];
    public static OrbInfo[] activeOrbsArray = new OrbInfo[maxOrbs];
    
    public static float scoreToSizeMultiplier = 0.25f;
    public static float _mapSizeX = 300;
    public static float _mapSizeY = 300;
    public static float minOrbSize = 1;
    
    
    //Player Start Values
    public static int playerScoreStart = 5;
    
    public static float scoreDefenseModifier = 1.1f;
    public static float playerSizeStart = 5f;
    public static float playerMovementSpeed = 20f;
    

    public static async Task SetUp(){
        Console.WriteLine("Starts World Set Up");
        connectedPlayerArray = new PlayerInfo[maxPlayers];
        activeOrbsArray = new OrbInfo[maxOrbs];
        PrepareOrbArray();
        CreateEmptyPlayerSlots();
        Console.WriteLine("Finished World Set Up");
    }
    
    
    static void CreateEmptyPlayerSlots(){
        for (int i = 1; i < maxPlayers; i++){
            connectedPlayerArray[i] = new PlayerInfo();
        }
    }
    
    
    static async Task PrepareOrbArray(){
        Console.WriteLine($"Preparing Orb Array with size of {maxOrbs}...");
       
        for (int i = 1; i < maxOrbs; i++){
            CreateNewOrb(i);
        }

        Console.WriteLine($"Prepared Orb Array with size of {maxOrbs}.");
    }

    static async Task CreateNewOrb(int i){
        Random random = new Random();
        activeOrbsArray[i] = new OrbInfo();
        activeOrbsArray[i].id = i;
        activeOrbsArray[i].score = random.Next(1, 11);
        activeOrbsArray[i].size = minOrbSize + (activeOrbsArray[i].score * scoreToSizeMultiplier);
        activeOrbsArray[i].scoreDefenseModifier = scoreDefenseModifier;
        activeOrbsArray[i].colorR = random.Next(0, 256); //255 is color max
        activeOrbsArray[i].colorG = random.Next(0, 256);
        activeOrbsArray[i].colorB = random.Next(0, 256);
        activeOrbsArray[i].positionX = random.NextSingle() * (_mapSizeX / 2f); //0-1 * 300 can be 0.34*300 == 102
        activeOrbsArray[i].positionY = random.NextSingle() * (_mapSizeY / 2f);
    }
}