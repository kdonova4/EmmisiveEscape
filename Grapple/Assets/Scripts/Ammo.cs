using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Ammo : MonoBehaviour
{
    [Header("Ammo")]
    public int maxAmmo = 7;
    public int currentAmmo = 7;
    

            


        bool canShoot()
    {
        if (currentAmmo > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    

    public int getAmmo()
    {
        return currentAmmo;
    }
    public int getMaxAmmo()
    {
        return maxAmmo;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        

    }
}
