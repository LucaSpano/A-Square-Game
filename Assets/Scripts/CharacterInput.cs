using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rewired;
using UnityEngine;

public class CharacterInput : MonoBehaviour
{
    public int inputSetIndex; 
    
    public class InputSet
    {
        public KeyCode right;
        public KeyCode left;
        public KeyCode jump;
    }
    
    public struct InputState
    {
        public float  horizontal;
        public bool   jump;
    }

//    static InputSet[] _inputSets =
//    {
//        new InputSet() { right = KeyCode.D, left = KeyCode.A, jump = KeyCode.W },
//        new InputSet() { right = KeyCode.L, left = KeyCode.J, jump = KeyCode.I },
//        new InputSet() { right = KeyCode.RightArrow, left = KeyCode.LeftArrow, jump = KeyCode.UpArrow }        
//    };
    
    public InputState PoolInputs()
    {
        Player rePlayer;
        if (ReInput.players.GetPlayerIds().Contains(inputSetIndex)) {
            rePlayer = ReInput.players.GetPlayer(inputSetIndex);
        }
        else {
            rePlayer = null;
        }

        if (rePlayer == null) {
            return new InputState() {
                horizontal = 0f,
                jump = false,
            };
        }
        return new InputState()
        {
            horizontal = rePlayer.GetAxis("Move Horizontal"),
            jump = rePlayer.GetButton("Jump")
        };   
    }

    float GetAxis(KeyCode neg, KeyCode pos)
    {
        return (Input.GetKey(neg) ? -1f : 0f) + (Input.GetKey(pos) ? 1f : 0f);
    }
}
