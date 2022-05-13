using System;

namespace AgarioShared
{
    [Serializable] public class OrbInfo{
        public int id;
        public int score;
        
        public int colorR;
        public int colorG;
        public int colorB;

        public float size;
        public float scoreDefenseModifier;
        public float positionX;
        public float positionY;
    }
}
