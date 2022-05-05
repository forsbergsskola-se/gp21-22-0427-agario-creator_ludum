using System.Drawing;
using System.Numerics;

namespace AgarioServer;

public class Message{
    public string messageName;
}

public class Message<T> : Message{
    public T value;
}

public class ConnectToServerMessage : Message{
    public string name;
    public Color color;
}

public class PositionMessage : Message{
    public int id;
    public float positionX;
    public float positionY;
}

public class InitialServerToClientMessage : PositionMessage{
    public Dictionary<int, PlayerInfo> playerDictionary;
    public float size = 3f; 
    public int score = 0;
    public float mapSizeX = 300f;
    public float mapSizeY = 300f;
}