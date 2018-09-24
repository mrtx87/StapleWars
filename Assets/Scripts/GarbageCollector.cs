using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageCollector : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	} 
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Block") || collision.gameObject.CompareTag("GroundedBlock"))
        {
            if (collision.gameObject.GetComponent<Block>() == GameManager.instance.CurrentBlock)
                GameManager.instance.CurrentBlock = null;
            else
                GameManager.instance.removeStapledBlock(collision.gameObject.GetComponent<Block>());


            GameManager.instance.lostBlocks += 1;
            Destroy(collision.gameObject);
        }
    }
}
