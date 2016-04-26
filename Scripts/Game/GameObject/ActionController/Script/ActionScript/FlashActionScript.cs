using System;
using UnityEngine;
namespace MTB
{
	public class FlashActionScript : BaseActionScript
	{

		private ParticleSystem particle;
		private const string PARTICLE_PATH = "Effects/Game_Effects/E_smoke_kengdaochong";

		public FlashActionScript (GameObjectController gameObjectController)
			:base(gameObjectController)
		{
			GameObject particlePrefab = ResourceManager.Instance.LoadAsset<GameObject>(PARTICLE_PATH) as GameObject;
			GameObject go = GameObject.Instantiate(particlePrefab) as GameObject;
			go.transform.parent = _gameObjectController.transform;
			go.transform.localPosition = Vector3.zero;
			particle = go.GetComponentInChildren<ParticleSystem>();
			particle.Stop();
		}

		public override void ActionIn ()
		{
			if(particle.isPlaying)
			{
				particle.Stop();
				particle.Clear();
			}
			particle.Play();
		}
	}
}

