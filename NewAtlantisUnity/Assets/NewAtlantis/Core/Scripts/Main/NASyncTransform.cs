﻿using UnityEngine;
using System.Collections;



public class NASyncTransform : MonoBehaviour {

	NetworkView nv = null;
	float timer = 0f;
	public float interval = 0.2f;
	Vector3 last_received_position = Vector3.zero;
	Vector3 last_received_angles = Vector3.zero;
	Vector3 last_received_velocity = Vector3.zero;
	Quaternion last_received_rotation = Quaternion.identity;
	GameObject goTarget = null;

	Vector3 current_speed = Vector3.zero;
	Vector3 last_position = Vector3.zero;

	// Use this for initialization
	void Start () 
	{
		nv = GetComponent<NetworkView>();
		goTarget = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		goTarget.transform.localScale = Vector3.one*0.3f;
		Collider.Destroy(goTarget.GetComponent<Collider>());

	}

	void OnDestroy()
	{
		GameObject.Destroy(goTarget);
	}
	
	// Update is called once per frame
	void Update () 
	{
		timer+=Time.deltaTime;
		if (nv.owner == Network.player)
		{
			current_speed = (transform.position-last_position)/Time.deltaTime;
			last_position = transform.position;

			if (timer > interval)
			{
				timer -= interval;
				//send a sync frame
				nv.RPC("SetTransformState", RPCMode.Others, transform.position, transform.rotation, current_speed);
				//LogManager.Log("SEND SetTransformState " + gameObject.name + " " + transform.position);
			}
		}
		else
		{
			//kind of dead reckoning
			Vector3 position = transform.position;
			Quaternion rotation = transform.rotation;

			last_received_position += last_received_velocity*Time.deltaTime;
			//to do : improve this !!!
			position = Vector3.Lerp(position, last_received_position, 0.1f);
			//position = last_received_position;
			rotation = Quaternion.Lerp(rotation, last_received_rotation, 0.2f);
			//position += last_received_velocity*Time.deltaTime;
			transform.position = position;
			transform.rotation = rotation;

			if (timer > 30)
			{
				//10 seconds without update means a dead avatar
				gameObject.SetActive(false);
				NA.RemoveAvatar(gameObject);
			}
		}
	}
	[RPC]
	void SetTransformState(Vector3 position, Quaternion rotation, Vector3 velocity) 
	{
		//LogManager.Log("SetTransformState " + gameObject.name + " " + position);
		timer = 0;
		last_received_position 	= position;
		last_received_rotation 	= rotation;
		last_received_velocity 	= velocity;
		goTarget.transform.position = position;
		//transform.position = last_received_position;
	}
}
