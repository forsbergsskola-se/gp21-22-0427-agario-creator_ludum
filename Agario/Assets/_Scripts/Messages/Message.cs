using System.Collections.Generic;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public class Message{
 public string messageName;
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
 public float size; 
 public int score;
 
 public float mapSizeX;
 public float mapSizeY;
}
