using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameObject player;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Implementing the Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate GameManager instances
            return;
        }

        Instance = this;
        
        DontDestroyOnLoad(gameObject); // Persist this GameManager across scenes
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); // Find the player object by tag   
    }

    public void OnClickUIBrakePress()
    {   Console.WriteLine("Brake Pressed");
        // Handle UI brake click event
        if (player != null)
        {Console.WriteLine("Brake Pressed Player not null");
            player.GetComponent<BikeController>().ExternalBrakePress(); // Call the Brake method on the PlayerController component
        }
    }

    public void OnClickUIBrakeRelease()
    {
        // Handle UI brake click event
        if (player != null)
        {
            player.GetComponent<BikeController>().ExternalBrakeRelease(); // Call the Brake method on the PlayerController component
        }
    }
}