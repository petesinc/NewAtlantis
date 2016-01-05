﻿using UnityEngine;
using System.Collections;

/*
 * A very simple First Person Camera
 */
public class NACameraFirstPerson : NACamera {

	public Camera 	camera;
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	void LateUpdate()
	{
		//Pure First Person camera
		if (camera != null)
		{
			camera.transform.position = transform.position;
			camera.transform.rotation = transform.rotation;
		}
	}
}
