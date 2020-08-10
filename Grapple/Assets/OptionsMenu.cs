using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    private MainMenu backToMenu;
    private bool goingBack;
    public Camera menuCam;
    public float camMoveSpeed;
    public Light OptionsLight;
    public Light MenuLight;
    // Start is called before the first frame update
    void Start()
    {
        backToMenu = FindObjectOfType(typeof(MainMenu)) as MainMenu;
    }

    // Update is called once per frame
    void Update()
    {
        if(goingBack)
        {
            menuCam.transform.position = Vector3.Slerp(menuCam.transform.position, backToMenu.GetInitPos(), Time.deltaTime * camMoveSpeed);
            menuCam.transform.rotation = Quaternion.Slerp(menuCam.transform.rotation, backToMenu.GetInitRot(), Time.deltaTime * camMoveSpeed);
        }
        if (menuCam.transform.position == backToMenu.GetInitPos())
        {
            goingBack = false;
        }
    }
    public void SetGoingBack(bool val)
    {

        goingBack = val;
    }

    public void CamMovementBack()
    {
        
        MenuLight.gameObject.SetActive(true);
        OptionsLight.gameObject.SetActive(false);
        backToMenu.SetIsMoving(false);
        goingBack = true;
    }
}
