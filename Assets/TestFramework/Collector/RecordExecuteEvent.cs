using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TestFramework
{
    public class RecordExecuteEvent : MonoBehaviour
    {
        public delegate void OnExecuteEvent(IEventSystemHandler handler, BaseEventData pointerEventData);
        public event OnExecuteEvent OnEvent;

        public bool SkipSendTouch;
        void Awake()
        {
            SkipSendTouch = false;
            FieldInfo prop;
            
            prop = typeof(ExecuteEvents).GetField("s_PointerClickHandler", System.Reflection.BindingFlags.NonPublic
            | System.Reflection.BindingFlags.Static);
            prop.SetValue(null, new ExecuteEvents.EventFunction<IPointerClickHandler>(this.Execute));
            
            prop = typeof(ExecuteEvents).GetField("s_SelectHandler", System.Reflection.BindingFlags.NonPublic
            | System.Reflection.BindingFlags.Static);
            prop.SetValue(null, new ExecuteEvents.EventFunction<ISelectHandler>(this.Execute));

            prop = typeof(ExecuteEvents).GetField("s_DeselectHandler", System.Reflection.BindingFlags.NonPublic
            | System.Reflection.BindingFlags.Static);
            prop.SetValue(null, new ExecuteEvents.EventFunction<IDeselectHandler>(this.Execute));

            prop = typeof(ExecuteEvents).GetField("s_InitializePotentialDragHandler", System.Reflection.BindingFlags.NonPublic
            | System.Reflection.BindingFlags.Static);
            prop.SetValue(null, new ExecuteEvents.EventFunction<IInitializePotentialDragHandler>(this.Execute));

            //prop = typeof(ExecuteEvents).GetField("s_BeginDragHandler", System.Reflection.BindingFlags.NonPublic
            //| System.Reflection.BindingFlags.Static);
            //prop.SetValue(null, new ExecuteEvents.EventFunction<IBeginDragHandler>(this.Execute));

            prop = typeof(ExecuteEvents).GetField("s_EndDragHandler", System.Reflection.BindingFlags.NonPublic
            | System.Reflection.BindingFlags.Static);
            prop.SetValue(null, new ExecuteEvents.EventFunction<IEndDragHandler>(this.Execute));

            prop = typeof(ExecuteEvents).GetField("s_DragHandler", System.Reflection.BindingFlags.NonPublic
            | System.Reflection.BindingFlags.Static);
            prop.SetValue(null, new ExecuteEvents.EventFunction<IDragHandler>(this.Execute));

            /*
            prop = typeof(ExecuteEvents).GetField("s_ScrollHandler", System.Reflection.BindingFlags.NonPublic
            | System.Reflection.BindingFlags.Static);
            prop.SetValue(null, new ExecuteEvents.EventFunction<IScrollHandler>(this.Execute));

            prop = typeof(ExecuteEvents).GetField("s_SubmitHandler", System.Reflection.BindingFlags.NonPublic
            | System.Reflection.BindingFlags.Static);
            prop.SetValue(null, new ExecuteEvents.EventFunction<ISubmitHandler>(this.Execute));

            prop = typeof(ExecuteEvents).GetField("s_SelectHandler", System.Reflection.BindingFlags.NonPublic
            | System.Reflection.BindingFlags.Static);
            prop.SetValue(null, new ExecuteEvents.EventFunction<ISelectHandler>(this.Execute));

            prop = typeof(ExecuteEvents).GetField("s_DeselectHandler", System.Reflection.BindingFlags.NonPublic
            | System.Reflection.BindingFlags.Static);
            prop.SetValue(null, new ExecuteEvents.EventFunction<IDeselectHandler>(this.Execute));
                        */

            prop = typeof(ExecuteEvents).GetField("s_UpdateSelectedHandler", System.Reflection.BindingFlags.NonPublic
            | System.Reflection.BindingFlags.Static);
            prop.SetValue(null, new ExecuteEvents.EventFunction<IUpdateSelectedHandler>(this.Execute));


            prop = typeof(ExecuteEvents).GetField("s_PointerDownHandler", System.Reflection.BindingFlags.NonPublic
            | System.Reflection.BindingFlags.Static);
            prop.SetValue(null, new ExecuteEvents.EventFunction<IPointerDownHandler>(this.Execute));

            prop = typeof(ExecuteEvents).GetField("s_PointerUpHandler", System.Reflection.BindingFlags.NonPublic
            | System.Reflection.BindingFlags.Static);
            prop.SetValue(null, new ExecuteEvents.EventFunction<IPointerUpHandler>(this.Execute));
        }

        private void Execute(IUpdateSelectedHandler handler, BaseEventData eventData)
        {
            var go = (handler as Component).gameObject;
            //Debug.LogWarning("IUpdateSelectedHandler : " + go.name);
            //OnEvent(handler, eventData);
            if (SkipSendTouch)
                return;

            handler.OnUpdateSelected(eventData);
        }

        private void Execute(IPointerClickHandler handler, BaseEventData eventData)
        {
            var go = (handler as Component).gameObject;
            Debug.LogWarning("IPointerClickHandler : " + go.name);
            OnEvent(handler, eventData);
            if (SkipSendTouch)
                return;

            handler.OnPointerClick(ExecuteEvents.ValidateEventData<PointerEventData>(eventData));
        }

        private void Execute(ISelectHandler handler, BaseEventData eventData)
        {
            var go = (handler as Component).gameObject;
            Debug.LogWarning("ISelectHandler : " + go.name);
            //OnEvent(handler, eventData);
            if (SkipSendTouch)
                return;

            handler.OnSelect(eventData);
        }

        private void Execute(IDeselectHandler handler, BaseEventData eventData)
        {
            var go = (handler as Component).gameObject;
            Debug.LogWarning("IDeselectHandler : " + go.name);
            //OnEvent(handler, eventData);
            if (SkipSendTouch)
                return;

            handler.OnDeselect(eventData);
        }

        private void Execute(IInitializePotentialDragHandler handler, BaseEventData eventData)
        {
            var go = (handler as Component).gameObject;
            Debug.LogWarning("IInitializePotentialDragHandler : " + go.name);
            OnEvent(handler, eventData);
            if (SkipSendTouch)
                return;

            handler.OnInitializePotentialDrag(ExecuteEvents.ValidateEventData<PointerEventData>(eventData));
        }

        private void Execute(IPointerDownHandler handler, BaseEventData eventData)
        {
            var go = (handler as Component).gameObject;
            Debug.LogWarning("IPointerDownHandler : " + go.name);
            OnEvent(handler, eventData);
            if (SkipSendTouch)
                return;

            handler.OnPointerDown(ExecuteEvents.ValidateEventData<PointerEventData>(eventData));
        }

        private void Execute(IPointerUpHandler handler, BaseEventData eventData)
        {
            var go = (handler as Component).gameObject;
            Debug.LogWarning("IPointerUpHandler : " + go.name);
            OnEvent(handler, eventData);
            if (SkipSendTouch)
                return;

            handler.OnPointerUp(ExecuteEvents.ValidateEventData<PointerEventData>(eventData));
        }

        private void Execute(IEndDragHandler handler, BaseEventData eventData)
        {
            var go = (handler as Component).gameObject;
            Debug.LogWarning("IEndDragHandler : " + go.name);
            OnEvent(handler, eventData);
            if (SkipSendTouch)
                return;

            handler.OnEndDrag(ExecuteEvents.ValidateEventData<PointerEventData>(eventData));
        }

        private void Execute(IDragHandler handler, BaseEventData eventData)
        {
            var go = (handler as Component).gameObject;
            Debug.LogWarning("IDragHandler : " + go.name);
            OnEvent(handler, eventData);
            if (SkipSendTouch)
                return;

            handler.OnDrag(ExecuteEvents.ValidateEventData<PointerEventData>(eventData));
        }
    }
}