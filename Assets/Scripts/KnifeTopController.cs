using UnityEngine;
using System.Collections;

public class KnifeTopController : MonoBehaviour
{
    private Rigidbody2D knifeBody;
    private PlayerController knifeScript;
    private Animator knifeAnimator;

    public AudioClip hitStone;
    public AudioClip hitPlank;
    public AudioClip hitMetal;

    private float dyingTime = 60.0f;

    void Awake()
    {
        this.knifeBody = GetComponentInParent<Rigidbody2D>();
        this.knifeScript = GetComponentInParent<PlayerController>();
        this.knifeAnimator = GetComponentInParent<Animator>();
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < dyingTime; i++)
        {
            Camera.main.orthographicSize -= 9 / 60;
            
        }
    }

    void OnCollisionEnter2D(Collision2D theCollision)
    {
        string tag = theCollision.gameObject.tag;

        if (tag == "Ground" || tag == "Plank")
        {
            knifeBody.bodyType = RigidbodyType2D.Static;
            AudioSource.PlayClipAtPoint(hitPlank, transform.position);
        }
        else if (tag == "Stone")
        {
            knifeBody.bodyType = RigidbodyType2D.Static;
            knifeScript.sharpness -= 0.4f;
            AudioSource.PlayClipAtPoint(hitStone, transform.position);
        }
        else if (tag == "Rust")
        {
            knifeScript.sharpness -= 0.4f;
            AudioSource.PlayClipAtPoint(hitMetal, transform.position);
        }

        if(knifeScript.sharpness <= 0f & !knifeScript.dying){
            knifeBody.rotation = 0;
            knifeScript.dying = true;
            knifeAnimator.SetTrigger("Death");
        }
    }
}
