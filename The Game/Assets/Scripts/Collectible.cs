using UnityEngine;

public class Collectible : MonoBehaviour
{
    private GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    //public void CollectItem()
}
