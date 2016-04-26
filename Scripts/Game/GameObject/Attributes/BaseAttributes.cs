/*********************************************************************************
 * 
 *基本属性包括foid groupId等
 *可能后面改成info之类的再封装一层也可以
 * 
 * *******************************************************************************/
using UnityEngine;
using System.Collections;
namespace MTB
{
    public class BaseAttributes : MonoBehaviour
    {
        private int _aoId;
        private int _groupId;
		private int _objectId;
		private bool _isNetObj;

        public int aoId
        {
            get
            {
                return _aoId;
            }
            set
            {
                _aoId = value;
            }
        }

        public int groupId
        {
            get
            {
                return _groupId;
            }
            set
            {
                _groupId = value;
            }
        }

		public int objectId
		{
			get{
				return _objectId;
			}
			set{
				_objectId = value;
			}
		}

		public bool isNetObj
		{
			get{
				return _isNetObj;
			}
			set{
				_isNetObj = value;
			}
		}

		protected virtual void Awake(){}
    }
}
