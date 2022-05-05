using System.Drawing;

namespace AgarioServer;


public class PlayerInfo{
    public int id;
    public int score;
    
    public string name;
    
    public Color color;
    public Dictionary<int, PlayerInfo> playerDictionary;
    
    public float size;
    public float positionX;
    public float positionY;

    public PlayerInfo(int _id){
        id = _id;
    }
}