using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message{
 public string messageName;
}

// public class Message<T> : Message{
//  public T value;
// }

public class ConnectToServerMessage : Message{
 public string name;
 public Color color;
}

public class PositionMessage : Message{
 public int id;
 public Vector2 position;
}

public class InitialServerToClientMessage : PositionMessage{
 public Dictionary<int, Player> playerDictionary;
 public float size; 
 public int score;
}
