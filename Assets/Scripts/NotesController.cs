using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotesController : MonoBehaviour {

	private static readonly Dictionary<LINE, float> LINE_POS = new Dictionary<LINE, float>() {
		{ LINE.LINE_1, -4.5f },
		{ LINE.LINE_2, -1.5f },
		{ LINE.LINE_3, 1.5f },
		{ LINE.LINE_4, 4.5f },
	};

	private static readonly List<Note.Param> MUSIC_DATA = new List<Note.Param>() {
		new Note.Param(LINE.LINE_1, 1f),
		new Note.Param(LINE.LINE_2, 2f),
		new Note.Param(LINE.LINE_3, 3f),
		new Note.Param(LINE.LINE_4, 4f),
		new Note.Param(LINE.LINE_1, 5f),
		new Note.Param(LINE.LINE_2, 6f),
		new Note.Param(LINE.LINE_3, 7f),
		new Note.Param(LINE.LINE_4, 8f),
	};

	private static readonly Dictionary<JUDGE, float> JUDGE_TIME = new Dictionary<JUDGE, float>() {
		{ JUDGE.PERFECT, 0.017f * 2},
		{ JUDGE.GREAT, 0.017f * 4},
		{ JUDGE.GOOD, 0.017f * 6},
	};

	private static readonly Dictionary<JUDGE, int> JUDGE_SCORE = new Dictionary<JUDGE, int>() {
		{ JUDGE.PERFECT, 1000},
		{ JUDGE.GREAT, 500},
		{ JUDGE.GOOD, 100},
		{ JUDGE.MISS, 0},
	};

	private const float BASE_SPEED = 10f;
	private const float INPUT_PERMISSION_TIME = 0.017f * 20f;

	[SerializeField] private AudioSource bgm;
	[SerializeField] private AudioSource se;

	[SerializeField] private GameObject notePrefab;
	[SerializeField] private GameObject barObj;

	[SerializeField] private Text scoreText;
	[SerializeField] private JudgeText[] judgeTexts;

	private Dictionary<LINE, List<Note>> notesList = new Dictionary<LINE, List<Note>>();
	private Dictionary<LINE, int> currentNotesIndexDic = new Dictionary<LINE, int>();

	private int score;

	// Use this for initialization
	void Start () {
		notesList [LINE.LINE_1] = new List<Note> ();
		notesList [LINE.LINE_2] = new List<Note> ();
		notesList [LINE.LINE_3] = new List<Note> ();
		notesList [LINE.LINE_4] = new List<Note> ();

		GenerateNotes ();

		bgm.Play ();
	}
	
	// Update is called once per frame
	void Update () {
		foreach (var notes in notesList.Values) {
			foreach (var note in notes) {
				note.Move (bgm.time, barObj.transform.position.y, BASE_SPEED);
			}
		}

		for (int i = 0; i < (int)LINE.MAX; ++i) {
			var line = (LINE)i;
			int noteIndex = currentNotesIndexDic[line];
			if (noteIndex < 0)
				continue;

			var note = notesList [line] [noteIndex];
			var diff = note.param.timeSec - bgm.time;
			if (diff < -INPUT_PERMISSION_TIME) {
				note.gameObject.SetActive (false);

				judgeTexts [(int)line].Draw (JUDGE.MISS);
				CalcScore (JUDGE.MISS);
				NextNote (noteIndex, line);
			}
		}

		if (Input.GetKeyDown(KeyCode.A)) {
			Judge (LINE.LINE_1);
		}
		if (Input.GetKeyDown(KeyCode.S)) {
			Judge (LINE.LINE_2);
		}
		if (Input.GetKeyDown(KeyCode.D)) {
			Judge (LINE.LINE_3);
		}
		if (Input.GetKeyDown(KeyCode.F)) {
			Judge (LINE.LINE_4);
		}
	}

	void GenerateNotes() {
		foreach (var master in MUSIC_DATA) {
			var noteObj = Instantiate (notePrefab, transform);
			var pos = noteObj.transform.position;
			pos.x = LINE_POS [master.line];
			pos.y = barObj.transform.position.y + (master.timeSec - bgm.time) * BASE_SPEED;
			noteObj.transform.position = pos;
			var note = noteObj.GetComponent<Note> ();
			note.param.line = master.line;
			note.param.timeSec = master.timeSec;
			notesList [master.line].Add (note);
		}

		currentNotesIndexDic [LINE.LINE_1] = 0;
		currentNotesIndexDic [LINE.LINE_2] = 0;
		currentNotesIndexDic [LINE.LINE_3] = 0;
		currentNotesIndexDic [LINE.LINE_4] = 0;
	}

	void Judge(LINE line) {
		int noteIndex = currentNotesIndexDic [line];
		var note = notesList [line] [noteIndex];

		var diff = Mathf.Abs (bgm.time - note.param.timeSec);
		if (diff >= INPUT_PERMISSION_TIME)
			return;

		var judge = JUDGE.MISS;

		if (diff < JUDGE_TIME[JUDGE.PERFECT]) {
			judge = JUDGE.PERFECT;
		} else if (diff < JUDGE_TIME[JUDGE.GREAT]) {
			judge = JUDGE.GREAT;
		} else if (diff < JUDGE_TIME[JUDGE.GOOD]) {
			judge = JUDGE.GOOD;
		}

		note.gameObject.SetActive (false);

		judgeTexts [(int)line].Draw (judge);
		CalcScore (judge);
		NextNote (noteIndex, line);
	}

	void NextNote(int noteIndex, LINE line) {
		if (noteIndex + 1 < notesList[line].Count) {
			++currentNotesIndexDic [line];
		} else {
			currentNotesIndexDic [line] = -1;
		}
	}

	void CalcScore(JUDGE judge) {
		score += JUDGE_SCORE [judge];
		scoreText.text = "SCORE:" + score.ToString ("D7");
	}
}
