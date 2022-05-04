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