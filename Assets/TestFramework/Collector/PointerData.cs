using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestFramework
{
    public class PointerData
    {
        public GameObject SelectGameObject;

        private Vector2 _position;

        public Vector2 position
        {
            get { return _position; }
            set { _position = value; UpdateCubePosition(); }
        }

        public bool IsMouseDown { get; private set; }
        public bool IsMouseUp { get; private set; }
        public bool IsMouse { get; private set; }

        private GameObject cube;

        public void TouchDown()
        {
            IsMouseUp = false;
            IsMouseDown = IsMouse = true;

            cube = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            
            UpdateCubePosition();
        }

        private void UpdateCubePosition()
        {
            if(cube == null)
                return;

            var mousePos = new Vector3(position.x, position.y);
            mousePos.z = 1.5f;
            var worldPos = Camera.main.ScreenToWorldPoint(mousePos);

            cube.transform.position = worldPos;
        }

        public void RevertTouchDown()
        {
            IsMouseDown = false;
        }

        public void RevertTouchUp()
        {
            IsMouseUp = false;
        }

        public void TouchUp()
        {
            IsMouseUp = true;
            IsMouse = false;

            GameObject.Destroy(cube);
        }
    }
}