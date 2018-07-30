using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour {

	public struct Param {
		public LINE line;
		public float timeSec;

		public Param (LINE _line, float _timeSec) {
			line = _line;
			timeSec = _timeSec;
		}
	}

	public Param param;

	public void Move(float bgmTimeSec, float barPosY, float speed) {
		var pos = transform.position;
		pos.y = barPosY + (param.timeSec - bgmTimeSec) * speed;
		transform.position = pos;
	}
}
