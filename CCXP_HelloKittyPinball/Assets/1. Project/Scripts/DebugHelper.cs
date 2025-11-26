using System;
using System.Collections.Generic;
using UnityEngine;

namespace _1._Project.Scripts{

	[Serializable]
public class DebugObject
{
	public Transform Position;
	public KeyCode Key;
}

public class DebugHelper : MonoBehaviour
	{
		public GameManager GameManager;
		public List<DebugObject> DebugObjects;
		public void Update()
		{
			foreach (var debugObject in DebugObjects)
			{
				if (Input.GetKey(debugObject.Key))
				{
					MoveBallToPosition(debugObject.Position);
				}
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