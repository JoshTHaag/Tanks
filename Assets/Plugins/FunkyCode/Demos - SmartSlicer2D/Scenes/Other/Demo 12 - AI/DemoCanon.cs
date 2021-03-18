using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

public class DemoCanon : MonoBehaviour {
	public GameObject spawner;
	public GameObject ballPrefab;
	public Material trajectoryMaterial;
	public float applyForce = 0;

	void Update () {
		Vector2 pos = GetMousePosition();

		float rotation = (float)Vector2D.Atan2(pos, transform.position) * Mathf.Rad2Deg;

		transform.rotation = Quaternion.Euler(0, 0, rotation);

		if (Input.GetMouseButton(0)) {
			applyForce += Time.deltaTime * 15;
			DrawTrajectory();
		} else {
			applyForce -= Time.deltaTime * 15;
		}

		if (applyForce < 10) {
			applyForce = 10;
		}

		if (applyForce > 50) {
			applyForce = 50;
		}

		if (Input.GetMouseButtonUp(0)) {
			GameObject ball = Instantiate(ballPrefab);
			Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
			rb.velocity = spawner.transform.right * applyForce;
			ball.transform.position = spawner.transform.position;

			applyForce = 10;
		}
	}

	public static Vector2 GetMousePosition() {
		return(Camera.main.ScreenToWorldPoint (Input.mousePosition));
	}

	public void DrawTrajectory() {
		Polygon2D trajectory = new Polygon2D();
		//trajectory.AddPoint(0, 0);

		Vector2 pos = spawner.transform.position;
		Vector2 gravity = Physics2D.gravity;
		Vector2 force = spawner.transform.right * applyForce;

		float timer = 0;

		while(timer < 3) {
			float delta = 0.1f;

			trajectory.AddPoint(pos);

			pos += force * delta;
			force += gravity * delta;

			timer += delta;
		}

		//Mesh mesh = Max2DMesh.CreatePolygon(transform, trajectory, -3f, 1f, false);
		//Max2DMesh.Draw(mesh, trajectoryMaterial);
	}
}
