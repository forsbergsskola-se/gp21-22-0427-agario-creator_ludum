using System.Drawing;

namespace AgarioServer;


[Serializable]public class PlayerInfo{
    public int id;
    public int score;
    
    public string name;
    
    public int colorR;
    public int colorG;
    public int colorB;

    public float size;
    public float positionX;
    public float positionY;
}