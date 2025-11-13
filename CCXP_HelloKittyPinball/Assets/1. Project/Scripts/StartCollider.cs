using System.Collections;
using UnityEngine;

namespace _1._Project.Scripts
{
	public class StartCollider : MonoBehaviour
	{
		public GameObject ballCollider;
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Player"))
			{
				StartCoroutine(WaitToTurnOn());
			}
		}

		private IEnumerator WaitToTurnOn()
		{
			yield return new WaitForSeconds(0.5f);
			ballCollider.SetActive(true);
		}
	}
}