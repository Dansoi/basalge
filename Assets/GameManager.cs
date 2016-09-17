using UnityEngine;


public class GameManager : MonoBehaviour {
	public static GameManager inst; //instance

	public Camera fullOverviewCam;
	public Camera snippetOverviewCam;
	public Camera worksheetCam;
	public Camera miscbarCam;

	public bool exerciseMode;

	void Awake(){
		inst = this;
	}
	void Start(){
		focusOverview ();
	}

	public void focusOverview(){
		exerciseMode = false;

		fullOverviewCam.enabled = true;
		snippetOverviewCam.enabled = false;
		worksheetCam.enabled = false;
		miscbarCam.enabled = false;

		Overview.inst.GetComponent<Canvas> ().worldCamera = fullOverviewCam;
		Overview.inst.GetComponentInChildren<HolderAxiomsAndExercises>().rulesSetClickable(false);
	}

	public void focusExercise(){
		exerciseMode = true;

		fullOverviewCam.enabled = false;
		snippetOverviewCam.enabled = true;
		worksheetCam.enabled = true;
		miscbarCam.enabled = true;

		Overview.inst.GetComponent<Canvas> ().worldCamera = snippetOverviewCam;

		//If method A called this method, then A should call RulesSetClickable(). A knows better what properties are allowed to be used (commutivity?)
	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Worksheet.inst.clear ();
			Overview.inst.GetComponentInChildren<ExerciseDetails> ().doHide ();
			focusOverview ();
		}
	}
}
