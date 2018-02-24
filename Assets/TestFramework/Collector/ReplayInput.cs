using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TestFramework
{
    public class ReplayInput : BaseInput
    {
        //public TouchRecordComponent RecordComponent;
        public InputReplayComponent ReplayComponent;

        /// <summary>
        ///   <para>Interface to base.mousePosition. Can be overridden to provide custom base instead of using the base class.</para>
        /// </summary>
        public override Vector2 mousePosition
        {
            get
            {
                if (ReplayComponent != null && ReplayComponent.CurrentPointerData != null)
                {
                    return ReplayComponent.CurrentPointerData.position;
                }

                return base.mousePosition;
            }
        }

        /// <summary>
        ///   <para>Interface to base.GetMouseButtonDown. Can be overridden to provide custom base instead of using the base class.</para>
        /// </summary>
        /// <param name="button"></param>
        public override bool GetMouseButtonDown(int button)
        {
            bool result;
            if (button == 0 && ReplayComponent != null && ReplayComponent.CurrentPointerData != null)
            {
                result = ReplayComponent.CurrentPointerData.IsMouseDown;
            }
            else
            {
                result = base.GetMouseButtonDown(button);
            }

            return result;
        }

        /// <summary>
        ///   <para>Interface to base.GetMouseButtonUp. Can be overridden to provide custom base instead of using the base class.</para>
        /// </summary>
        /// <param name="button"></param>
        public override bool GetMouseButtonUp(int button)
        {
            bool result;
            if (button == 0 && ReplayComponent != null && ReplayComponent.CurrentPointerData != null)
            {
                result = ReplayComponent.CurrentPointerData.IsMouseUp;
            }
            else
            {
                result = base.GetMouseButtonUp(button);
            }

            return result;
        }

        /// <summary>
        ///   <para>Interface to base.GetMouseButton. Can be overridden to provide custom base instead of using the base class.</para>
        /// </summary>
        /// <param name="button"></param>
        public override bool GetMouseButton(int button)
        {
            bool result;
            if (button == 0 && ReplayComponent != null && ReplayComponent.CurrentPointerData != null)
            {
                result = ReplayComponent.CurrentPointerData.IsMouse;
            }
            else
            {
                result = base.GetMouseButton(button);
            }

            //Debug.LogError(GetCurrentMethod() + "_" + button);
            return result;
        }
    }
}