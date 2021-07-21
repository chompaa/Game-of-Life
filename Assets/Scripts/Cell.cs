using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public enum States {
        Alive,
        Dead
    };

    public States state;
}
