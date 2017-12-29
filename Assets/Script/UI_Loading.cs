using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Loading : MonoBehaviour {

    
    public Sprite[] imgSprites;
    private Image imgLoading;
    private const int FPS = 12; // image frame per second

	// Use this for initialization
	void Start () {
        imgLoading = GetComponent<Image>();
	}
	
	
	void Update () {

        // change the image periodicly to have animation
        int spriteIndex = (int) (Time.time * FPS) % imgSprites.Length;
        imgLoading.sprite = imgSprites[spriteIndex];


    }
}
