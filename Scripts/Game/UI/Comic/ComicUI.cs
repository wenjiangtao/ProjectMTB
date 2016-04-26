using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace MTB
{
    public class ComicUI : UIOperateBase
    {
        private static string ComicPicPath = "Comic/Pic/comic_p";
        private Image picImage;

        public override void Init(params object[] paras)
        {
            base.Init(paras);
        }

        public override void InitView()
        {
            base.InitView();
            picImage = gameObject.GetComponent<Image>();
        }

        public void showComic(int PicId)
        {
            if (picImage == null)
                picImage = gameObject.GetComponent<Image>();
            string path = ComicPicPath + PicId;
			Sprite s = Resources.Load<Sprite>(path);
            picImage.sprite = s;
        }
    }
}
