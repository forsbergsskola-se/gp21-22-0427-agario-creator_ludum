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

[Serializable] public class PlayerInfoMessage : Message{
    public PlayerInfo playerInfo;
}

[Serializable] public class AllPlayerInfoMessage : Message{
    public Dictionary<string, PlayerInfo> allPlayersInfoDictionary;
}