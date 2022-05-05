using System.Drawing;
using System.Numerics;

namespace AgarioServer;

public class Message{
    public string messageName;
}

public class Message<T> : Message{
    public T value;
}

// public class ConnectToServerMessage : Message{
//     public string name;
//     public Color color;
// }

public class PositionMessage : Message{
    public int id;
    public float positionX;
    public float positionY;
}

public class InitialServerToClientMessage : PositionMessage{
    public int score;
    public float size;
    public float mapSizeX;
    public float mapSizeY;
}

public class PlayerInfoMessage : Message{
    public PlayerInfo playerInfo;
}

public class AllPlayerInfoMessage : Message{
    public Dictionary<int, PlayerInfo> allPlayerDictionary;
}