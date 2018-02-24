using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace TestFramework
{
    public class TouchController {
        struct TouchInfo
        {
            public int Frame;
            public float X, Y;
        }

        private RectTransform targeTransform;
        private PointerData targetPointer;

        private TouchInfo[] touchInfos;
        private int currentTouchIndex;
        private int currentFrameIndex;

        public TouchController(RectTransform rectTransform, string pathFile, PointerData pointerData)
        {
            targeTransform = rectTransform;
            targetPointer = pointerData;

            var lines = File.ReadAllLines(pathFile);
            touchInfos = new TouchInfo[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var ar = line.Split(new[] {'|'}, 3);
                var frame = int.Parse(ar[0]);
                var x = float.Parse(ar[1]);
                var y = float.Parse(ar[2]);
                touchInfos[i] = new TouchInfo()
                {
                    Frame = frame,
                    X = x,
                    Y = y
                };
            }

            currentFrameIndex = 0;
            currentTouchIndex = 0;
        }

        public void NextFrame()
        {
            if (currentTouchIndex >= touchInfos.Length && currentFrameIndex > touchInfos[touchInfos.Length].Frame)
            {
                targetPointer.RevertTouchUp();
                return;
            }

            if (currentTouchIndex == 0 && currentFrameIndex == 0)
            {
                targetPointer.TouchDown();
            }

            var currentTouch = touchInfos[currentTouchIndex];

            Vector3 min = targeTransform.TransformPoint(targeTransform.rect.min + new Vector2(currentTouch.X, currentTouch.Y));
            //Vector3 max = rectTransform.TransformPoint(rectTransform.rect.max);

            var min2D = RectTransformUtility.WorldToScreenPoint(Camera.main, min);
            targetPointer.position = min2D;

            currentFrameIndex++;
            //if(currentFrameIndex)
        }
    }
}