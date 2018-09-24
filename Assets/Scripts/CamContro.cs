using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamContro : MonoBehaviour {

    float yHeight;
    public GameObject Background;

	// Use this for initialization
	void Start () {

        yHeight = 1 +  Mathf.Round(transform.position.y);

	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void LateUpdate()
    {

        float y = Mathf.Lerp(transform.position.y, 2.5f + GameManager.instance.currentHeight, 0.1f);
        transform.position = new Vector3(transform.position.x, y, transform.position.z);


        if(transform.position.y >= yHeight)
        {
            yHeight += 0.5f;
            Background.transform.position -= Vector3.up/8;
        }
    }
}
