using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{

    public bool smallSideDown;
    public GameObject Shadow;
    public GameObject ShadowSmall;
    bool isActiveBlock;
    int currentAngle = 0;
    Rigidbody2D rb;
    // Use this for initialization
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        if (smallSideDown)
        {
            ShadowSmall.SetActive(true);
        }
        else
        {
            Shadow.SetActive(true);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {



        if (isActiveBlock)
        {
            rb.rotation = currentAngle;

        }

    }


    public GameObject explosion;
    public float collisionLimit;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isActive() && collision.gameObject.CompareTag("GroundedBlock"))
        {
            toggleInActive();
            GameManager.instance.StopBlock();
            if (collision.relativeVelocity.y > GameManager.instance.LimitToShake)  //hit.collider != null)
            {
                GameManager.instance.shakeCam();
                //Destroy(Instantiate(explosion, new Vector2(transform.position.x + Random.value / 2, transform.position.y - 0.25f - Random.value / 2), Quaternion.identity), 0.3f);
                //Destroy(Instantiate(explosion, new Vector2(transform.position.x + Random.value / 2, transform.position.y - 0.25f + Random.value / 2), Quaternion.identity), 0.3f);
                //Destroy(Instantiate(explosion, new Vector2(transform.position.x - Random.value / 2, transform.position.y - 0.2f - Random.value / 2), Quaternion.identity), 0.3f);
            }

        }
        
    }

    internal void groundBlock()
    {
        rb.gravityScale = GameManager.instance.gravity;
        rb.velocity = new Vector2(0, GameManager.instance.DropSpeed / 2); ;
        rb.mass = GameManager.instance.StapledMass;
        rb.drag = GameManager.instance.StapledDrag;
        GameManager.instance.addStapledBlock(this);
        Destroy(Shadow);
        Destroy(ShadowSmall);
        tag = "GroundedBlock";
        GameManager.instance.CurrentBlock = null;

    }

    public void ToggleShadow()
    {
        smallSideDown = !smallSideDown;

        if (smallSideDown)
        {
            Shadow.SetActive(false);
            ShadowSmall.SetActive(true);
        }
        else
        {
            ShadowSmall.SetActive(false);
            Shadow.SetActive(true);
        }

        currentAngle += 90;
        if(currentAngle >= 360)
        {
            currentAngle = 0;
        }

    }

    public void setStatic()
    {
        rb.bodyType = RigidbodyType2D.Static;
 
    }

    public bool isStatic()
    {
        return (rb.bodyType == RigidbodyType2D.Static) ? true : false;
    }

    public void toggleActive()
    {
        isActiveBlock = true;
    }

    public void toggleInActive()
    {
        isActiveBlock = false;
    }

    public bool isActive()
    {
        return isActiveBlock;
    }

    public Vector2 getVelocity()
    {
        return rb.velocity;
    }
}
