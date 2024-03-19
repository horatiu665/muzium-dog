using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTriggerOff : MonoBehaviour 

{
	public GameObject gameobject;

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player") )
			{
				gameobject.SetActive(false);
			}
			}
			}
