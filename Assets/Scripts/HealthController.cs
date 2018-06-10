using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{

    private PlayerController knifeScript;
    public GameObject player;
    private Slider slider;

    void Awake()
    {
        this.knifeScript = player.GetComponent<PlayerController>();
        this.slider = GetComponentInChildren<Slider>();
    }

    void FixedUpdate()
    {
        float sharpness = knifeScript.sharpness;
        slider.value = sharpness;
    }
}
