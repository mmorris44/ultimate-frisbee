using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceAnimations : MonoBehaviour {
	public class eyeExpressions {
		public GameObject rightEye;
		public GameObject leftEye;
	}

	//public eyeExpressions[] faceExpressions;
	public GameObject[] mouths;
	public GameObject[] leftEye;
	public GameObject[] rightEye;

	void Start () {
		if (leftEye.Length>0){
			leftEye[0].SetActive(true);
		}
		if (rightEye.Length>0){
			rightEye[0].SetActive(true);
		}
		if (mouths.Length>0){
			mouths[0].SetActive(true);
		}
	}
				
}
