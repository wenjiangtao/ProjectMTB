/*****
 * 镜头锁定模式后续增加
 * ****/

using System;
using UnityEngine;
using UnityEngine.EventSystems;
namespace MTB
{
    public class MTBComicController : Singleton<MTBComicController>
    {
        private static string ComicObjPath = "Comic/Comic";
        private ComicUI _comicUI;
        private bool _playMark;
        private float _timeDelay;
        private ComicInfo _info;

        //暂时不公开
        //public void playComic(int picId)
        //{
        //    comicUI = UIManager.Instance.showUI<ComicUI>(UITypes.COMIC, ComicObjPath) as ComicUI;
        //    comicUI.showComic(picId);
        //}

        public void playComicByTime(ComicInfo info)
        {
            _info = info;
            _playMark = true;
            _timeDelay = _info.timeEachPic;
            _comicUI = UIManager.Instance.showUI<ComicUI>(UITypes.COMIC, ComicObjPath) as ComicUI;
        }

        private void finishComicPlay()
        {
            _playMark = false;
            UIManager.Instance.closeUI(UITypes.COMIC);
        }

        void Update()
        {
            if (!_playMark)
                return;
            if (_timeDelay >= _info.timeEachPic)
            {
                _timeDelay = 0;
                if (_info.index < _info.ids.Length)
                    _comicUI.showComic(_info.ids[_info.index]);
                else
                    finishComicPlay();
                _info.index++;
            }
            if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
#if IPHONE || ANDROID
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)){
#else
                if (EventSystem.current.IsPointerOverGameObject())
                {
#endif
                    if (_info.index < _info.ids.Length)
                        _comicUI.showComic(_info.ids[_info.index]);
                    else
                        finishComicPlay();
                    _info.index++;
                }
            }
            _timeDelay += Time.deltaTime;
        }
    }

    public class ComicInfo
    {
        public int index;
        public int[] ids;
        public float timeEachPic;
        public ComicInfo(int[] ids, float timeEachPic)
        {
            index = 0;
            this.ids = ids;
            this.timeEachPic = timeEachPic;
        }
    }
}
