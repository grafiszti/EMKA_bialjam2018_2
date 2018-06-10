using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportController : MonoBehaviour {
    public GameObject outPortal;
    public Vector2 spawnPoint;
    public GameObject player;

    public bool disabled = false;

    public AudioClip teleportSound;

    public TeleportController outPortalController;

    public void Awake()
    {
        outPortalController = outPortal.GetComponentInChildren<TeleportController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        AudioSource.PlayClipAtPoint(teleportSound, transform.position);
        if(collision.gameObject.tag == "Player" && !disabled){
            outPortalController.disabled = true;
            player.GetComponentInChildren<Rigidbody2D>().position = outPortalController.spawnPoint;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        disabled = false;
    }
}
