using UnityEngine;
using System.Collections;

public class Flare : MonoBehaviour {
			

	private Light flarelight;
	private AudioSource flaresound;
	private ParticleSystemRenderer smokepParSystem;
	public AudioClip flareBurningSound;


	// Use this for initialization
	void Start () {

		flaresound = GetComponent<AudioSource>();
		flaresound.clip = flareBurningSound;
		flaresound.Play();
		flarelight = GetComponent<Light>();
		flaresound = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		flarelight.intensity = Random.Range(2f,6.0f);	
	}

}
