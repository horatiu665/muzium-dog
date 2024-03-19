using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTriggerActivate : MonoBehaviour 

{
	public GameObject gameobject;

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player") )
			{
				gameobject.SetActive(true);
			}
			}
			}
