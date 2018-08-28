﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Drag : MonoBehaviour {

	private Collider2D drag;
	public LayerMask layer;
	[SerializeField]
	private bool clicked;
	private Touch touch;

	public LineRenderer lineFront;
	public LineRenderer lineBack;

	private Ray leftCatapultRay;
	private CircleCollider2D passaroCol;
	private Vector2 catapultToBird;
	private Vector3 pointL;

	private SpringJoint2D spring;
	private Vector2 prevVel;
	private Rigidbody2D passaroRB;

	public GameObject bomb;

	//Limite

	private Transform catapult;
	private Ray rayToMT;

	//Rastro

	private TrailRenderer rastro;

	public Rigidbody2D CatapultRB;
	public bool estouPronto = false;

	public AudioSource audioPassaro;
	public GameObject audioMortePassaro;



	void Awake()
	{
		spring = GetComponent<SpringJoint2D> ();
		lineFront = (LineRenderer)GameObject.FindWithTag ("LF").GetComponent<LineRenderer> ();
		lineBack = (LineRenderer)GameObject.FindWithTag ("LB").GetComponent<LineRenderer> ();
		CatapultRB = GameObject.FindWithTag ("LB").GetComponent<Rigidbody2D> ();
		spring.connectedBody = CatapultRB;

		//Ajuste fino
		Vector2 temp = spring.connectedAnchor;
		temp.x = 0;
		temp.y = 0;
		spring.connectedAnchor = temp;
		//

		drag = GetComponent<Collider2D> ();
		leftCatapultRay = new Ray (lineFront.transform.position, Vector3.zero);
		passaroCol = GetComponent<CircleCollider2D> ();

		passaroRB = GetComponent<Rigidbody2D> ();
		catapult = spring.connectedBody.transform;
		rayToMT = new Ray (catapult.position, Vector3.zero);
		rastro = GetComponentInChildren<TrailRenderer> ();
		audioPassaro = GetComponent<AudioSource> ();


	}

	// Use this for initialization
	void Start () {
		SetupLine ();
	}



	// Update is called once per frame
	void Update () {


		LineUpdate ();
		SpringEffect ();
		prevVel = passaroRB.velocity;

		#if UNITY_ANDROID



		if(Input.touchCount > 0)
		{
			touch = Input.GetTouch (0);


			Vector2 wp = Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position);
			RaycastHit2D hit = Physics2D.Raycast (wp, Vector2.zero, Mathf.Infinity, layer.value);

			if(hit.collider != null)
			{
				//Ajuste fino

				if(GAMEMANAGER.instance.pausado == false)
				{
					if(transform.position == GAMEMANAGER.instance.pos.position)
					{
						clicked = true;
						rastro.enabled = false;
						estouPronto = true;
					}
				}

				//


			}

			if (clicked) {
				
				if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved) {
					Vector3 tPos = Camera.main.ScreenToWorldPoint (new Vector3 (touch.position.x, touch.position.y, 10));

					catapultToBird = tPos - catapult.position;

					if(catapultToBird.sqrMagnitude > 9f)
					{
						rayToMT.direction = catapultToBird;
						tPos = rayToMT.GetPoint (3f);
					}

					transform.position = tPos;
					rastro.enabled = false;
				}

				if (touch.phase == TouchPhase.Ended) {
					passaroRB.isKinematic = false;
					clicked = false;
					rastro.enabled = true;
				}
			
			}			

		}
		#endif

		//#if UNITY_EDITOR



		if(clicked)
		{
			Dragging();

		}

		//#endif



		if (clicked == false && passaroRB.isKinematic == false && passaroRB.IsSleeping()) {			
			MataPassaro ();
			passaroRB.isKinematic = true;
		}


		if(passaroRB.isKinematic == false)
		{
			Vector3 posCam = Camera.main.transform.position;
			posCam.x = transform.position.x;
			posCam.x = Mathf.Clamp (posCam.x, GAMEMANAGER.instance.objE.position.x, GAMEMANAGER.instance.objD.position.x);
			Camera.main.transform.position = posCam;
		}

	}



	void SetupLine()
	{
		lineFront.SetPosition (0, lineFront.transform.position);
		lineBack.SetPosition (0, lineBack.transform.position);
	}

	void LineUpdate()
	{
		if(transform.name == GAMEMANAGER.instance.nomePassaro)
		{
			catapultToBird = transform.position - lineFront.transform.position;
			leftCatapultRay.direction = catapultToBird;

			pointL = leftCatapultRay.GetPoint (catapultToBird.magnitude + passaroCol.radius);

			lineFront.SetPosition (1, pointL);
			lineBack.SetPosition (1, pointL);
		}

	}


	void SpringEffect()
	{
		if(spring != null && GAMEMANAGER.instance.passarosEmCena > 0)
		{
			if(passaroRB.isKinematic == false)
			{
				if(prevVel.sqrMagnitude > passaroRB.velocity.sqrMagnitude)
				{
					lineFront.enabled = false;
					lineBack.enabled = false;
					Destroy (spring);
					passaroRB.velocity = prevVel;
				}
			}
			else if(passaroRB.isKinematic == true && transform.position == GAMEMANAGER.instance.pos.position)
			{
				lineFront.enabled = true;
				lineBack.enabled = true;
			}
		}
	}

	void MataPassaro()
	{
		if(passaroRB.velocity.magnitude == 0 && passaroRB.IsSleeping() )
		{			
			StartCoroutine (TempoMorte ());
		}
	}

	IEnumerator TempoMorte()
	{
		yield return new WaitForSeconds (0.3f);
		Instantiate (bomb, new Vector2 (transform.position.x, transform.position.y), Quaternion.identity);
		Instantiate (audioMortePassaro, new Vector2 (transform.position.x, transform.position.y), Quaternion.identity);
		Destroy (gameObject);

			GAMEMANAGER.instance.passarosNum -=1;
			GAMEMANAGER.instance.passarosEmCena = 0;
			GAMEMANAGER.instance.passaroLancado = false;
			estouPronto = false;


	}

	//mouse

	void Dragging()
	{

		if(passaroRB.isKinematic)
		{
			Vector3 mouseWP = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mouseWP.z = 0f;

			catapultToBird = mouseWP - catapult.position;

			if(catapultToBird.sqrMagnitude > 9f)
			{
				rayToMT.direction = catapultToBird;
				mouseWP = rayToMT.GetPoint (3f);
			}

			transform.position = mouseWP;

		}

	}

	void OnMouseDown()
	{
		if(GAMEMANAGER.instance.pausado == false)
		{
			if(transform.position == GAMEMANAGER.instance.pos.position)
			{
				clicked = true;
				rastro.enabled = false;
				estouPronto = true;
			}
		}

	}

	void OnMouseUp()
	{
		if(estouPronto)
		{
			passaroRB.isKinematic = false;
			clicked = false;
			rastro.enabled = true;
			GAMEMANAGER.instance.passaroLancado = true;
			audioPassaro.Play ();
		}

	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.gameObject.CompareTag("moedasTag"))
		{
			GAMEMANAGER.instance.moedasGame += 50;

			UIMANAGER.instance.moedasTxt.text = GAMEMANAGER.instance.moedasGame.ToString ();
			Destroy (col.gameObject);
		}
	}

}
