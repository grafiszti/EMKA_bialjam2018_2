﻿using UnityEngine;
using System.Collections;

public class KnifeTopController : MonoBehaviour
{
    private Rigidbody2D knifeBody;
    private PlayerController knifeScript;
    private Animator knifeAnimator;
    // Use this for initialization
    void Awake()
    {
        this.knifeBody = GetComponentInParent<Rigidbody2D>();
        this.knifeScript = GetComponentInParent<PlayerController>();
        this.knifeAnimator = GetComponentInParent<Animator>();
    }

    private void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D theCollision)
    {
        string tag = theCollision.gameObject.tag;

        if(tag == "Ground" || tag == "Plank"){
            knifeBody.bodyType = RigidbodyType2D.Static;
            knifeScript.sharpness -= 0.1f;
        } else if (tag == "Stone"){
            knifeBody.bodyType = RigidbodyType2D.Static;
            knifeScript.sharpness -= 0.4f;
        }

        if(knifeScript.sharpness <= 0f){
            knifeAnimator.SetTrigger("Death");
        }
    }
}
