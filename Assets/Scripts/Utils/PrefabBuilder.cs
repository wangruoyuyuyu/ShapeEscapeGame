using UnityEngine;

namespace Monitor.Entry.Parts
{
	public class PrefabBuilder : MonoBehaviour
	{
		[SerializeField]
		public GameObject[] _prefabs;

		public GameObject[] Entities { get; private set; }
		[SerializeField] PrefabCountingStat counter;

		private void Awake()
		{
			if (counter != null)
			{
				counter.cnt = 0;
			}
			Entities = new GameObject[_prefabs.Length];
			for (int i = 0; i < _prefabs.Length; i++)
			{
				Entities[i] = Object.Instantiate(_prefabs[i], base.transform, worldPositionStays: false);
				if (counter != null)
				{
					counter.cnt++;
					Entities[i].GetComponent<levelNumSetManager>().SetNum(counter.cnt);
				}
			}
		}
		public void Build()
		{
			if (counter != null)
			{
				counter.cnt = 0;
			}
			Entities = new GameObject[_prefabs.Length];
			for (int i = 0; i < _prefabs.Length; i++)
			{
				Entities[i] = Object.Instantiate(_prefabs[i], base.transform, worldPositionStays: false);
				if (counter != null)
				{
					counter.cnt++;
					if (Entities[i].GetComponent<levelNumSetManager>() == null)
					{
						Debug.Log("is  null!");
					}
					Entities[i].GetComponent<levelNumSetManager>().SetNum(counter.cnt);
				}
			}
		}
	}
}
