using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lumiere : MonoBehaviour {
	[SerializeField] private MeshRenderer lumiereMesh;
	private Material lumiereMat;

	void Start() {
		lumiereMat = lumiereMesh.material;
	}

	private void OnTriggerEnter(Collider other) {
		this.lumiereMat.color = other.gameObject.GetComponent<MeshRenderer>().material.color;
	}

	private void OnTriggerExit(Collider other) {
		this.lumiereMat.color = Color.black;
	}

}
