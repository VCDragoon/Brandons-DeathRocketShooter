using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour {

    [Tooltip("In meters per second")] [SerializeField] float xSpeed = 20f;
    [Tooltip("In meters per second")] [SerializeField] float ySpeed = 15f;
    [Tooltip("In meters")] [SerializeField] float xRange = 5.5f;
    [Tooltip("In meters")] [SerializeField] float yRange = 4f;
    [SerializeField] GameObject[] guns;
    [SerializeField] float smoothing = 50f;

    public int gunPower = 5;

    [SerializeField] float pitchPositionFactor = -5f;
    [SerializeField] float pitchControlFactor = -25f;

    [SerializeField] float yawPositionFactor = 8f;
    //[SerializeField] float yawControlFactor = 5f;

    //[SerializeField] float rollPositionFactor = 5f;
    [SerializeField] float rollControlFactor = -20f;

    float xThrow;
    float yThrow;

    bool isControlEnabled = true;

    // Use this for initialization
    void Start () {
        Cursor.lockState = CursorLockMode.Locked;

	}
	
	// Update is called once per frame
	void Update ()
    {
        if (isControlEnabled)
        {
            ProcessTranslation();
            ProcessRotation();
            ProcessFiring();
        }

    }

   void OnPlayerDeath() //call by string ref
    {
        isControlEnabled = false;
    }
    private void ProcessTranslation()
    {
        xThrow = CrossPlatformInputManager.GetAxis("Horizontal");
        yThrow = CrossPlatformInputManager.GetAxis("Vertical");

        float xOffset = xThrow * xSpeed * Time.deltaTime;
        float yOffset = yThrow * ySpeed * Time.deltaTime;

        float rawXPos = transform.localPosition.x + xOffset;
        float rawYPos = transform.localPosition.y + yOffset;

        float clampedXPos = Mathf.Clamp(rawXPos, -xRange, xRange);
        float clampedYPos = Mathf.Clamp(rawYPos, -yRange, yRange);


        transform.localPosition = new Vector3(clampedXPos, clampedYPos, transform.localPosition.z);
    }

    private void ProcessRotation()
    {
        float pitchPosition = transform.localPosition.y * pitchPositionFactor;
        float pitchThrow = yThrow * pitchControlFactor;
        float pitch = pitchPosition + pitchThrow;

        float yaw = transform.localPosition.x * yawPositionFactor;
        float roll = xThrow * rollControlFactor;
        //transform.localRotation = Quaternion.Euler(pitch, yaw, roll);
        //this disables the above code in this method


        //todo set option to get from mouse or from keyboard!!
        // Gets input from mouse
        Vector3 input = new Vector3(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"), 0); // May have to change these inputs around to your needs.
                                                                                                   // Calculate rotation

        Quaternion rotation = transform.localRotation *= Quaternion.Euler(input * smoothing * Time.deltaTime);
        // Clamp rotions y to 40%
        //rotation.y = Mathf.Clamp(rotation.y, -0.4f, 0.4f);
        // Clamp rotions x to 30%
        //rotation.x = Mathf.Clamp(rotation.x, -0.3f, 0.3f);
        // Set rotations z to 0
        rotation.z = 0;
        // Set cockpits rotation
        transform.localRotation = rotation;

    }







    void ProcessFiring()
    {
        if (CrossPlatformInputManager.GetButton("Fire"))
        {
            SetGunsActive(true);
        }
        else
        {
            SetGunsActive(false);
        }
    }



    private void SetGunsActive(bool isActive)
    {
        foreach (GameObject gun in guns)
        {
            var emissionModule = gun.GetComponent<ParticleSystem>().emission;
            emissionModule.enabled = isActive;
        }
    }
}
