using System;
using AgarioShared;


namespace Network{
    public class Message{
        public string messageName;

        public Message(){
            messageName = GetType().FullName;
        }
    }

    public class Message<T> : Message{
        
    }

    public class PositionMessage : Message{
        public int id;
        public float positionX;
        public float positionY;
    }

    public class InitialServerToClientMessage : PositionMessage{
        public int score;
        public int maxPlayers;
        public float size;
        public float movementSpeed;
        public float mapSizeX;
        public float mapSizeY;
    }
    public class PlayerInfoMessage : Message{
        public PlayerInfo playerInfo;
    }
    
     public class AllPlayerInfoMessage : Message{
        public PlayerInfo[] allPlayersInfoArray; //Dictionary Converted to string
    }
    
    //Orbs 
    
    public class OrbInfoMessage : Message{
        public OrbInfo orb;
    }

    
    public class AllOrbsInfoMessage : Message{
        public OrbInfo[] allOrbsArray;
    }
    
    
 
}