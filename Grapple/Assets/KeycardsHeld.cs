using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeycardsHeld : MonoBehaviour
{

    private List<GameObject> keycards = new List<GameObject>();
    // Start is called before the first frame update
    public List<GameObject> ReturnList()
    {
        return keycards;
    }


}
