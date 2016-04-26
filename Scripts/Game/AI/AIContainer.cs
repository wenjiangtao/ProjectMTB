/*********************************************************************************
 * 
 * 用于同一获取或设置AIcomponent
 * 这里面可以装各的AI
 * 
 * *******************************************************************************/
using UnityEngine;
using System.Collections;
namespace MTB
{
    public class AIContainer : MonoBehaviour
    {
        private IAIComponent _aiComponent;

        public IAIComponent AIComponent
        {
            get
            {
                return _aiComponent;
            }
            set
            {
                _aiComponent = value;
                _aiComponent.attach(gameObject);
            }
        }

        void Update()
        {
            if (_aiComponent != null)
            {
                _aiComponent.onTick();
            }
        }
    }
}
