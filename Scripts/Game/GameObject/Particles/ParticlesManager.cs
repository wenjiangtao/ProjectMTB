using UnityEngine;
using System.Collections;

namespace MTB
{
	public class ParticlesManager : Singleton<ParticlesManager> {
		void Awake()
		{
			Instance = this;
		}

		public void Init()
		{

		}

		public void PlayParticle(){}

	}
}