using System;
using UnityEngine;
namespace MTB
{
	public class DropItem : MonoBehaviour
	{
		public delegate void OnPickUpHandler(DropItem dropItem,GameObject player);
		public delegate void OnSelfDisappearHandler(DropItem dropItem);
		public event OnPickUpHandler On_PickUp;
		public event OnSelfDisappearHandler On_SelfDisappear;

		public float disappearTime = 60f;
		public float flySpeed = 5f;
		private float _curTime;
		private bool _isFly;
		private GameObject _player;
		private Renderer render;

		public UserItem userItem{get;private set;}

		public void Init(UserItem userItem,Texture tex)
		{
			this.userItem = userItem;
			render = GetComponent<Renderer>();
			render.material.mainTexture = tex;
			_isFly = false;
		}

		public void FlyToPlayer(GameObject player)
		{
			_player = player;
			_isFly = true;
		}

		void Awake()
		{
			_curTime = 0;
		}

		void Update()
		{
			if(!_isFly)
			{
				_curTime += Time.deltaTime;
				if(_curTime > disappearTime)
				{
					On_SelfDisappear(this);
				}
				WorldPos pos = Terrain.GetWorldPos(this.transform.position);
				Block block = World.world.GetBlock(pos.x,pos.y,pos.z);
				BlockAttributeCalculator calculator = BlockAttributeCalculatorFactory.GetCalculator(block.BlockType);
				int sunLight = World.world.GetSunLight(pos.x,pos.y,pos.z);
				int blockLight = World.world.GetBlockLight(pos.x,pos.y,pos.z);
				float lightIntensity = MTBSkyBox.Instance.GetLightIntensity(sunLight,blockLight);
				render.material.SetFloat("_lightIntensity",lightIntensity);
				if(calculator.GetMeshColliderType(block.ExtendId) == MeshColliderType.terrainCollider)
				{
					On_SelfDisappear(this);
				}
			}
			else
			{
				if(_player != null)
				{
					Vector3 dis = _player.transform.position + new Vector3(0,1.5f,0) - this.transform.position;
					if(dis.magnitude < 0.3f)
					{
						On_SelfDisappear(this);
						return;
					}
					Vector3 v = dis.normalized * flySpeed;
					this.transform.position += v * Time.deltaTime;
				}
				else
				{
					On_SelfDisappear(this);
				}
			}
		}

		void OnTriggerEnter(Collider collider)
		{
			if(collider.CompareTag("Player"))
			{
				Destroy(this.GetComponent<Rigidbody>());
				Destroy(this.GetComponent<BoxCollider>());
				Destroy(this.GetComponent<SphereCollider>());
				On_PickUp(this,collider.gameObject);
			}
		}

		void OnDestroy()
		{
			userItem = null;
		}
	}
}

