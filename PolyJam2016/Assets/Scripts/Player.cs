﻿using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public float speed = 15;
    public bool isPlayerOne = true;
    public Vector2 playerSpeedMulti;

	Animator animator;
    ParticleSystem particleSystem;

    private string horizontal;
    private string vertical;
    private string action;

    // Use this for initialization
    void Start () {
		animator = GetComponentInChildren<Animator> ();
        particleSystem = GetComponentInChildren<ParticleSystem>();

        // choosing right player inputs
        if (isPlayerOne)
        {
            horizontal = "Player One Horizontal";
            vertical = "Player One Vertical";
            action = "Player One Action";
        }
        else
        {
            horizontal = "Player Two Horizontal";
            vertical = "Player Two Vertical";
            action = "Player Two Action";
        }
    }

    void FixedUpdate()
    {
        // input normalization
        Vector2 inputs = new Vector2(Input.GetAxis(horizontal), Input.GetAxis(vertical));
        if (inputs.magnitude > 1)
        {
            inputs.Normalize();
        }
        if(inputs.magnitude > 0)
        {
            float inputDots = Vector2.Dot(inputs.normalized, new Vector2(1, 0));
            float arcCos = Mathf.Acos(inputDots);
            float yRotation = Mathf.Rad2Deg * arcCos;

            if (inputs.normalized.y > 0)
            {
                yRotation *= -1;
            }

            yRotation += 90;

            transform.rotation = Quaternion.Euler(
                0,
                yRotation,
                0);
        }

		if (inputs.magnitude <= 0) {
			animator.SetBool ("walking", false);
            particleSystem.Stop();
        } else {
			animator.SetBool ("walking", true);
            particleSystem.Emit(1);
        }

        // update player position
        Vector3 positionSwitch = new Vector3(inputs.x * speed * Time.deltaTime * playerSpeedMulti.x,
            0,
            inputs.y * speed * Time.deltaTime * playerSpeedMulti.y);
        transform.position += positionSwitch;
            
        // we don't want player to fall
        //transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
