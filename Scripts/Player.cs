//Hunter Chu and Edward Cao
//100701653 and 100697845
//March 28th, 2022

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Cinemachine;
using UnityEngine;

public class Player : MonoBehaviour
{

    //variables
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private CharacterController controller;

    private Vector3 movement;
    [SerializeField] private float speed = 1.0f;



    // Update is called once per frame
    void Update()
    {
        //movement
        movement = new Vector3(Input.GetAxis("Horizontal"),
                0.0f, Input.GetAxis("Vertical"));
        movement = Vector3.ClampMagnitude(movement, 1.0f);

        controller.Move(movement * speed * Time.deltaTime);
    }


    //initializes player settings
    public void Init(MazeCell startCell)
    {
        //disable character controller to set position
        controller.enabled = false;
        gameObject.transform.position = startCell.transform.position
                                        + new Vector3(0, 0.585f, 0);
        controller.enabled = true;

        //set camera to follow + zoom in on player
        vcam.Follow = gameObject.transform;
        vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset
            = new Vector3(0, -6.0f);
    }
}
