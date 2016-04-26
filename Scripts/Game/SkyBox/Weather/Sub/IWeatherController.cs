using UnityEngine;
using System.Collections;
namespace MTB
{
    interface IWeatherController
    {
        void setEnable(bool b);
        void setParent(Transform p);
        void setPosition(Vector3 p);
        void updateView();
        void dispose();
    }
}
