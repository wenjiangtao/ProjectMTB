using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
	public class BlockExplodeController : Singleton<BlockExplodeController>
	{
		private const string PARTICLE_PATH = "Effects/Game_Effects/E_posui";
		private const string PLANT_PARTICLE_PATH = "Effects/Game_Effects/E_Plant_disappear";
		private Queue<ParticleSystem> particleQueue;
		private Queue<GameObject> plantParticleQueue;
		
		public void Init()
		{
			particleQueue = new Queue<ParticleSystem>(5);
			ParticleSystem particlePrefab = ResourceManager.Instance.LoadAsset<ParticleSystem>(PARTICLE_PATH) as ParticleSystem;
			for (int i = 0; i < 5; i++) {
				ParticleSystem particle = GameObject.Instantiate<ParticleSystem>(particlePrefab);
				particle.transform.parent = this.transform;
				particle.Stop();
				particleQueue.Enqueue(particle);
			}
			plantParticleQueue = new Queue<GameObject>(5);
			GameObject plantGameObj = ResourceManager.Instance.LoadAsset<GameObject>(PLANT_PARTICLE_PATH) as GameObject;
			for (int i = 0; i < 5; i++) {
				GameObject gameObj = GameObject.Instantiate<GameObject>(plantGameObj);
				ParticleSystem particle = gameObj.GetComponentInChildren<ParticleSystem>();
				gameObj.transform.parent = this.transform;
				particle.Stop();
				plantParticleQueue.Enqueue(gameObj);
			}
		}
		
		public void Explode(WorldPos pos,Block block,Vector3 normal)
		{
			Item item = ItemManager.Instance.GetItemByBlockType((byte)block.BlockType,block.ExtendId);
			if(item != null && item.itemType == (int)ItemType.Block)
			{
				if(item.itemSubType == (int)ItemBlockType.Geology)
				{
					StartCoroutine(DelayCreateExplode(pos,block,normal));
				}
				else if(item.itemSubType == (int)ItemBlockType.Plant)
				{
					CreatePlantParticle(pos,block);
				}

//				CreateExplodeParticle(pos,block,normal);
//				if(item.itemSubType == (int)ItemBlockType.terrain)
//				{
//					CreateExplodeParticle(pos,block,normal);
//				}
//				else if(item.itemSubType == (int)ItemBlockType.plant)
//				{
//					CreatePlantParticle(pos,block);
//				}
			}
		}

		private IEnumerator DelayCreateExplode(WorldPos pos,Block block,Vector3 normal)
		{
			yield return new WaitForSeconds(0.1f);
			CreateExplodeParticle(pos,block,normal);
		}


		private void CreatePlantParticle(WorldPos pos,Block block)
		{
			GameObject obj = plantParticleQueue.Dequeue();
			ParticleSystem plantParticle = obj.GetComponentInChildren<ParticleSystem>();
			plantParticle.Play();
			obj.transform.position = new Vector3(pos.x + 0.5f,pos.y,pos.z + 0.5f);
			plantParticleQueue.Enqueue(obj);
		}
		
		private void CreateExplodeParticle(WorldPos pos,Block block,Vector3 normal)
		{
			ParticleSystem explode = particleQueue.Dequeue();
			
			Renderer render = explode.GetComponent<Renderer>();
			Direction dir = Direction.left;
			//更新光照
			int direction = CheckNormal(normal);
			WorldPos lightPos = pos;
			if(direction != 0)
			{
				dir = (Direction)direction;
				lightPos = new WorldPos(pos.x + Mathf.RoundToInt(normal.x),pos.y + Mathf.RoundToInt(normal.y),
				                        pos.z + Mathf.RoundToInt(normal.z));
			}
			
			int sunLightLevel = World.world.GetSunLight(lightPos.x,lightPos.y,lightPos.z);
			int blockLightLevel = World.world.GetBlockLight(lightPos.x,lightPos.y,lightPos.z);
			float lightIntensity = MTBSkyBox.Instance.GetLightIntensity(sunLightLevel,blockLightLevel);
			render.material.SetFloat("_lightIntensity",lightIntensity);
			//更新贴图
			ResetExplode(explode);
			BlockAttributeCalculator calculator = BlockAttributeCalculatorFactory.GetCalculator(block.BlockType);
			byte resExtendId = calculator.GetResourceExtendId(block.ExtendId);
			WorldTextureType type = WorldTextureProvider.Instance.GetTextureType(block.BlockType,resExtendId);
			render.material.mainTexture = WorldTextureProvider.Instance.GetAtlasTexture(type);
			Rect rect = WorldTextureProvider.Instance.GetBlockTextureRect(block.BlockType,resExtendId,dir);
			if(rect != null)
			{
				render.material.SetFloat("_OffsetX",rect.x);
				render.material.SetFloat("_OffsetY",rect.y);
				render.material.SetFloat("_Width",rect.width);
				render.material.SetFloat("_Random",UnityEngine.Random.Range(0,1f));
				explode.transform.position = new Vector3(pos.x + 0.5f,pos.y + 0.1f,pos.z + 0.5f);
			}
			explode.Play();
			particleQueue.Enqueue(explode);
		}
		
		private void ResetExplode(ParticleSystem explode)
		{
			explode.Stop();
			explode.Clear();
		}
		
		private int CheckNormal(Vector3 normal)
		{
			int direction = 0;
			if(normal == Vector3.up)
			{
				direction = (int)Direction.up;
			} else if(normal == Vector3.down)
			{
				direction = (int)Direction.down;
			} else if(normal == Vector3.left)
			{
				direction = (int)Direction.left;
			}else if(normal == Vector3.right)
			{
				direction = (int)Direction.right;
			}else if(normal == Vector3.forward)
			{
				direction =(int)Direction.front;
			}else if(normal == Vector3.back)
			{
				direction = (int)Direction.back;
			}
			return direction;
		}

		void OnDestroy()
		{
			particleQueue.Clear();
		}
	}
}

