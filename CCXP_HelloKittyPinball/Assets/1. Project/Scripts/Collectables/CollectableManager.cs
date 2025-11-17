using UnityEngine;

namespace _1._Project.Scripts.Collectables
{
	public class CollectableManager : MonoBehaviour
	{
		public int AmountOnRow1;
		public int AmountOnRow2;
		public int AmountOnRow3;
		public int AmountOnRow4;
		public GameManager GameManager;
		
		public void CollectableCollected(int rowId)
		{
			switch (rowId)
			{
				case 1:
					AmountOnRow1--;
					if (AmountOnRow1<=0)
					{
						GameManager.CreateBall();
					}
					break;
				case 2:
					AmountOnRow2--;
					if (AmountOnRow2<=0)
					{
						GameManager.CreateBall();
					}
					break;
				case 3:
					AmountOnRow3--;
					if (AmountOnRow3<=0)
					{
						GameManager.CreateBall();
					}
					break;
				case 4:
					AmountOnRow4--;
					if (AmountOnRow4<=0)
					{
						GameManager.CreateBall();
					}
					break;
			}
		}
	}
}