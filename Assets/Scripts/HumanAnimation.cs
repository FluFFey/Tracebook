using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanAnimation : MonoBehaviour {

    public int amountOfSpritesPerDirection = 3;
    private int leftAnimationIndexStart = 5;
    private int rightAnimationIndexStart = 15;
    private int upAnimationIndexStart = 10;
    private int downAnimationIndexStart = 0;

    private SpriteRenderer m_renderer;
    private Rigidbody2D m_body;
    private InputHandler m_inputhandler;
    public List<Sprite> sprites;

    private int spriteDirectionIndex = 0;
    private int spriteAnimationIndex = 0;
    private float animationUpdateTimer = 0.0f;

    void Start()
    {
        m_renderer = gameObject.GetComponent<SpriteRenderer>();
        m_body = gameObject.GetComponent<Rigidbody2D>();
        m_inputhandler = gameObject.GetComponent<InputHandler>();

    }
	
	// Update is called once per frame
	void Update ()
    {
        Vector2 velocity = m_body.velocity;
        if (velocity.x > 0)
        {
            spriteDirectionIndex = rightAnimationIndexStart;
        }
        else if (velocity.x < 0)
        {
            spriteDirectionIndex = leftAnimationIndexStart;
        }
        else if (velocity.y > 0)
        {
            spriteDirectionIndex = upAnimationIndexStart;
        }
        else if (velocity.y < 0)
        {
            spriteDirectionIndex = downAnimationIndexStart;
        }
        animationUpdateTimer += TimeManager.instance.gameDeltaTime;
        if (velocity.SqrMagnitude() > 0)
        {
            if (animationUpdateTimer > 0.15f)
            {
                animationUpdateTimer = 0.0f;
                spriteAnimationIndex++;
                spriteAnimationIndex = spriteAnimationIndex % amountOfSpritesPerDirection;
                m_renderer.sprite = sprites[spriteDirectionIndex + spriteAnimationIndex];
            }
        }
        else
        {
            m_renderer.sprite = sprites[spriteDirectionIndex + 1];
        }
    }
}
