using UnityEngine;
using ILogger = AgarioShared.ILogger;

namespace _Scripts{
    public class UnityLogger : ILogger{
        public void Log(string text){
            Debug.Log(text);
        }
    }
}