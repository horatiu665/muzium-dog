using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPlayerPosition : MonoBehaviour
{
    [SerializeField] private GameObject spawnPoint; // The spawn point GameObject

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Make sure the tag matches your player's tag
        {
            other.transform.position = spawnPoint.transform.position;  // Move the player to the spawn point position
        }
    }
}
