using UnityEngine;
using System.Collections;

public class KnifeTopController : MonoBehaviour
{
    private Rigidbody2D knifeBody;
    // Use this for initialization
    void Awake()
    {
        this.knifeBody = GetComponentInParent<Rigidbody2D>();
    }

    private void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D theCollision)
    {
        Debug.Log("asdasasda12313");
        string tag = theCollision.gameObject.tag;
        Debug.Log("TAG: " + tag);
        if(tag == "Stone" || tag == "Ground" || tag == "Plank"){
            knifeBody.bodyType = RigidbodyType2D.Static;
            Debug.Log("asdasasda");
        }
    }
}
