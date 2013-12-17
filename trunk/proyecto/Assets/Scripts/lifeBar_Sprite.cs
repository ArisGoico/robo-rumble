using UnityEngine;
using System.Collections;

public class lifeBar_Sprite : MonoBehaviour {

	public SpriteManager spriteManager;
	public GameObject lifeBarClient;

	private Sprite lifeBarSprite;

	public void Start()
	{
		//lifeBarClient = GameObject.Find("Robot1LifeBar");
		DrawLifeBar();
	}

	public void Update()
	{
		lifeBarClient.transform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x,
		                                                  Camera.main.transform.eulerAngles.y,
		                                                  lifeBarClient.transform.eulerAngles.z);
		spriteManager.Transform(lifeBarSprite);
	}

	public void DrawLifeBar()
	{
		lifeBarSprite = spriteManager.AddSprite(lifeBarClient, 2f, 0.5f, new Vector2(0f, 0.99f), new Vector2(1f, 0.01f), Vector3.zero, false);
	}

	public void ChangeLifeBar(float life)
	{
		spriteManager.RemoveSprite(lifeBarSprite);
		life = (float)System.Math.Round((double)life-0.01,2);
		if(life < 0)
			life = 0.0f;
		lifeBarSprite = spriteManager.AddSprite(lifeBarClient, 2f, 0.5f, new Vector2(0f, life), new Vector2(1f, 0.01f), Vector3.zero, false);
	}
}
