using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwitch : MonoBehaviour
{

    private Renderer lightMaterial;
    private Light light;
    public Material onMat;
    public Material offMat;
    // Start is called before the first frame update
    void Start()
    {
        lightMaterial = GetComponent<Renderer>();
        light = GetComponentInChildren<Light>();
        
    }

    // Update is called once per frame
    void Update()
    {
        SwitchOffOn();
    }

    void SwitchOffOn()
    {
        if(!light.gameObject.active)
        {
            lightMaterial.material = offMat;
        }
        else
        {
            lightMaterial.material = onMat;
        }
    }
}
