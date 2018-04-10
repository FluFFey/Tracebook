using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardScript : MonoBehaviour {

    private enum PATROL_DIRECTION
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }
    bool turning = false;
    public Vector2 patrolDirection;
    public float patrolSpeed;
    private Rigidbody2D rb2d;
    public Sprite[] sprites;
    public AudioClip spotGhostSound;
    private Sprite currentSprite;
    private int curSpriteID;
    private GameObject flashLightRotator;
    private bool spottedGhost = false;
    private SoundCaller sc;
    // Use this for initialization
    void Start () {
        sc = GetComponent<SoundCaller>();
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.velocity = patrolDirection * patrolSpeed;
        patrolDirection.Normalize();

        curSpriteID = rb2d.velocity.x < 0 ? (int)PATROL_DIRECTION.LEFT : (int)PATROL_DIRECTION.RIGHT;
        if (rb2d.velocity.y != 0)
        {
            curSpriteID = rb2d.velocity.y < 0 ? (int)PATROL_DIRECTION.UP : (int)PATROL_DIRECTION.DOWN;
        }
        flashLightRotator = transform.GetChild(0).gameObject;

        flashLightRotator.transform.rotation = Quaternion.Euler(0, 0, getAngle(patrolDirection));
        currentSprite = sprites[curSpriteID];
        GetComponent<SpriteRenderer>().sprite = currentSprite;

    }
	
    float getAngle(Vector2 direction)
    {
        if (direction.y >= 0)
        {
            return Mathf.Acos(direction.x) * Mathf.Rad2Deg;
        }
        else
        {
            if (direction.x >= 0)
            {
                return Mathf.Asin(direction.y) * Mathf.Rad2Deg;
            }
            else
            {
                return Mathf.Acos(direction.x) * Mathf.Rad2Deg + 90.0f;
            }
        }
    }
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (!spottedGhost && justBeforeCollision()) //need changes to support real time modifications in inspector, but not needed atm
        {
            StartCoroutine(turn());
        }
	}

    IEnumerator turn()
    {
        turning = true;
        Vector2 oldvel = rb2d.velocity;
        Vector2 oldDir = rb2d.velocity.normalized;
        Vector2 newDir = rb2d.velocity.normalized * -1;
        rb2d.velocity = Vector2.zero;
        curSpriteID = curSpriteID + 1 - 2 * ((curSpriteID) % 2); //yep
        currentSprite = sprites[curSpriteID];
        
        
        float turnTimer = 2.2f;
        float newZ;
        float pd;
        for (float i = 0; i < turnTimer; i+=Time.deltaTime)
        {
            pd = i / turnTimer;
            newZ = getAngle(oldDir)*(1- pd) + getAngle(newDir)*pd;
            flashLightRotator.transform.rotation = Quaternion.Euler(0, 0, newZ);
            rb2d.velocity = oldvel * -pd*pd;
            if (pd > 0.5)
            {
                GetComponent<SpriteRenderer>().sprite = currentSprite;
            }
            yield return null;
        }
        flashLightRotator.transform.rotation = Quaternion.Euler(0, 0, getAngle(newDir));
        rb2d.velocity = oldvel * -1;
        turning = false;
    }

    bool justBeforeCollision()
    {
        RaycastHit2D[] boxCasts = Physics2D.BoxCastAll(transform.position, 
            transform.localScale,
            0,
            rb2d.velocity,
            1);
        for (int i = 0; i < boxCasts.Length; i++) 
        {
            if (boxCasts[i].collider.tag != "Player")
            {
                return true;
            }
        }
        return false;
    }

    public IEnumerator spotGhost(float angle)
    {
        sc.attemptSound(spotGhostSound);
        if (!turning)
        {
            spottedGhost = true;
            if (rb2d.velocity.x > 0)
            {
                angle *= -1;
            }
            Vector2 oldVel = rb2d.velocity;
            rb2d.velocity = Vector2.zero;

            float lookTime = 0.3f;
            float oldAngle = flashLightRotator.transform.rotation.eulerAngles.z;
            float newAngle = oldAngle;
            for (float i = 0; i < lookTime; i += TimeManager.instance.fixedGameDeltaTime)
            {
                float pd = i / lookTime;
                newAngle = oldAngle + angle * pd;
                flashLightRotator.transform.rotation = Quaternion.Euler(0, 0, newAngle);
                yield return null;
            }
            yield return new WaitForSeconds(2.2f);

            flashLightRotator.transform.rotation = Quaternion.Euler(0, 0, oldAngle);
            rb2d.velocity = oldVel;
            spottedGhost = false;
        }
    }
}
