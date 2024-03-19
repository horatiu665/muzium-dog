using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTriggerDeactivate : MonoBehaviour 

{
	public GameObject gameobject;

	void OnTriggerExit(Collider other)
	{
		if(other.CompareTag("Player") )
			{
				gameobject.SetActive(false);
			}
			}
			}
