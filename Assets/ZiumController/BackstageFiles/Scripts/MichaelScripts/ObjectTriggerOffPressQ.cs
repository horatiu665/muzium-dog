using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTriggerOffPressQ : MonoBehaviour 

{
	public GameObject gameobject;

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player") && (Input.GetKeyDown("q")))
			{
				gameobject.SetActive(false);
			}
			}
			}
