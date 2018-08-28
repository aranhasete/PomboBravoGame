﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactCod : MonoBehaviour {

	private int limite;
	private SpriteRenderer spriteR;
	[SerializeField]
	private Sprite[] sprites;
	[SerializeField]
	private GameObject bomb,pontos1000;
	private AudioSource audioObj;
	[SerializeField]
	private AudioClip[] clips;

	// Use this for initialization
	void Start () {

		limite = 0;
		spriteR = GetComponent<SpriteRenderer> ();
		spriteR.sprite = sprites [0];
		audioObj = GetComponent<AudioSource> ();

	}

	void OnCollisionEnter2D(Collision2D col)
	{

		if(GAMEMANAGER.instance.jogoComecou == true)
		{

			if(col.relativeVelocity.magnitude > 4 && col.relativeVelocity.magnitude < 10)
			{
				if(limite < sprites.Length - 1)
				{
					limite++;
					spriteR.sprite = sprites [limite];
					audioObj.clip = clips [0];
					audioObj.Play ();

				}
				else if(limite == sprites.Length -1)
				{
					Instantiate (pontos1000, new Vector2 (transform.position.x, transform.position.y), Quaternion.identity);
					Instantiate (bomb, new Vector2 (transform.position.x, transform.position.y), Quaternion.identity);
					audioObj.clip = clips [1];
					audioObj.Play ();
					Destroy (gameObject,1);
					GAMEMANAGER.instance.pontosGame += 1000;
					UIMANAGER.instance.pontosTxt.text = GAMEMANAGER.instance.pontosGame.ToString ();

				}
			}
			else if(col.relativeVelocity.magnitude > 12 && col.gameObject.CompareTag("Player"))
			{
				Instantiate (pontos1000, new Vector2 (transform.position.x, transform.position.y), Quaternion.identity);
				Instantiate (bomb, new Vector2 (transform.position.x, transform.position.y), Quaternion.identity);
				audioObj.clip = clips [1];
				audioObj.Play ();
				Destroy (gameObject,1);
				GAMEMANAGER.instance.pontosGame += 1000;
				UIMANAGER.instance.pontosTxt.text = GAMEMANAGER.instance.pontosGame.ToString ();

			}
		}
	}

}
