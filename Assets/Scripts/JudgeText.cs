using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JudgeText : MonoBehaviour {

	private static readonly Dictionary<JUDGE, Color> JUDGE_COLOR = new Dictionary<JUDGE, Color>() {
		{ JUDGE.PERFECT, Color.yellow },
		{ JUDGE.GREAT, Color.green },
		{ JUDGE.GOOD, Color.blue },
		{ JUDGE.MISS, Color.red },
	};

	private const float VANISH_TIME = 0.5f;

	private Text text;
	private float drawStartTime;

	void Awake () {
		text = GetComponent<Text> ();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (!text.enabled)
			return;

		if (Time.time - drawStartTime > VANISH_TIME) {
			text.enabled = false;
		}
	}

	public void Draw(JUDGE judge) {
		text.text = judge.ToString ();
		text.color = JUDGE_COLOR [judge];

		drawStartTime = Time.time;
		text.enabled = true;
	}
}
