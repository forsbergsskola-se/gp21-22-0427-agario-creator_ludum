using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Int So", menuName = "Int So")]
public class IntSo : ScriptableObject{
    [Min(0)]public int intSO;
}
