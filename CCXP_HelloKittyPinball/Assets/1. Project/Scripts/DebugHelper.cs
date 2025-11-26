using System;
using UnityEngine;

namespace _1._Project.Scripts
{
	public class DebugHelper : MonoBehaviour
	{
		public GameManager GameManager;
		public Transform PositionChoco;
		public KeyCode ChocoKey;
		public Transform PositionHelloKitty;
		public KeyCode HelloKittyKey;
		public Transform PositionKeroppi;
		public KeyCode KeroppiKey;
		public Transform PositionPotchaco;
		public KeyCode PotchacoKey;
		public Transform PositionCinammon;
		public KeyCode CinammonKey;
		public Transform PositionMaru;
		public KeyCode MaruKey;
		public Transform PositionMyMelody;
		public KeyCode MyMelodyKey;
		public Transform PositionKuromi;
		public KeyCode KuromiKey;

		public void Update()
		{
			if (Input.GetKey(ChocoKey))
			{
				MoveBallToPosition(PositionChoco);
			}
			if (Input.GetKey(HelloKittyKey))
			{
				MoveBallToPosition(PositionHelloKitty);
			}

			if (Input.GetKey(KeroppiKey))
			{
				MoveBallToPosition(PositionKeroppi);
			}
			if (Input.GetKey(PotchacoKey))
			{
				MoveBallToPosition(PositionPotchaco);
			}
			if (Input.GetKey(CinammonKey))
			{
				MoveBallToPosition(PositionCinammon);
			}
			if (Input.GetKey(MaruKey))
			{
				MoveBallToPosition(PositionMaru);
			}
			if (Input.GetKey(MyMelodyKey))
			{
				MoveBallToPosition(PositionMyMelody);
			}
			if (Input.GetKey(KuromiKey))
			{
				MoveBallToPosition(PositionKuromi);
			}
		}
		
		private void MoveBallToPosition(Transform position)
		{
			if (GameManager.listBall.Count == 0)
			{
				return;
			}
			
			GameManager.listBall[0].transform.position = position.position;
			GameManager.listBall[0].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			GameManager.listBall[0].transform.localEulerAngles = Vector3.zero;
		}
	}
}