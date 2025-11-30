using System;
using System.Collections.Generic;
using System.Linq;
using NIX.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NIX.Packages
{
    public enum JoystickEvent
    {
        BeginDrag,
        Drag,
        EndDrag
    }

    [RequireComponent(typeof(UnityEngine.UI.Image))]
    public class JoystickCtrl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region Manager Joystick

        protected static Dictionary<string, JoystickCtrl> JOYSTICKS = new Dictionary<string, JoystickCtrl>();

        public static void AddJoystick(JoystickCtrl joystick, bool destroyIfExists = true)
        {
            if (joystick == null)
            {
                Debug.Log("Joystick cannot be null");
                return;
            }

            if (JOYSTICKS.ContainsKey(joystick.JoystickID))
            {
                Debug.Log($"Joystick ID: {joystick.JoystickID} exists!");
                if (!destroyIfExists) return;
                Destroy(joystick.gameObject);
                Debug.Log("Has destroyed Joystick ID: " + joystick.JoystickID);
            }
            else
            {
                JOYSTICKS.Add(joystick._JoystickID, joystick);
            }
        }

        public static void RemoveJoystick(JoystickCtrl joystick)
        {
            JOYSTICKS.Remove(joystick._JoystickID);
        }

        public static JoystickCtrl GetJoystick(string nameType)
        {
            return JOYSTICKS.GetValueOrDefault(nameType);
        }

        #endregion

        public EnumEvent<JoystickEvent> Events = new();

        [SerializeField] protected string _JoystickID;

        [SerializeField] protected Image _Joystick;
        [SerializeField] protected Transform _Handle;

        protected Vector2 _OriginPos;
        protected Vector2 _Direction;
        protected float _Radius;
        protected bool _StopControl = false;

        #region Get Method

        public virtual float Radius => _Radius;
        public virtual string JoystickID => _JoystickID;

        public virtual Vector3 Direction
        {
            get
            {
                if (_StopControl) return Vector3.zero;
                return _Direction;
            }
        }

        public virtual Vector3 Direction3D
        {
            get
            {
                if (_StopControl) return Vector3.zero;
                return new Vector3(_Direction.x, 0, _Direction.y);
            }
        }

        public virtual bool IsControl => !_Direction.Equals(Vector3.zero);
        public Image Joystick => _Joystick;

        #endregion

        protected virtual void Awake()
        {
            JoystickCtrl.AddJoystick(this);
        }

        protected virtual void Reset()
        {
            _Joystick = transform.GetComponentsInChildren<Image>()
                .FirstOrDefault(t => t.gameObject.name.Equals("Joystick"));
            _Handle = _Joystick.transform.Find("Handle");
        }

        protected virtual void Start()
        {
            _OriginPos = _Joystick.transform.position;
            RectTransform joyRect = _Joystick.GetComponent<RectTransform>();

            RectTransform rectCanvas = FindRectOfCanvasInParent(transform.parent);
            float canvasRectLocalScale = rectCanvas ? rectCanvas.localScale.x : 1f;

            _Radius = (joyRect.rect.width / 2f) * canvasRectLocalScale;
        }

        protected virtual void OnDestroy()
        {
            JoystickCtrl.RemoveJoystick(this);
        }

        protected virtual RectTransform FindRectOfCanvasInParent(Transform inParent)
        {
            if (inParent == null) return null;

            Canvas canvas = inParent.GetComponent<Canvas>();
            if (canvas != null) return canvas.GetComponent<RectTransform>();

            return FindRectOfCanvasInParent(inParent.parent);
        }

        public virtual void SetActiveJoystick(bool value)
        {
            this.enabled = value;
            if (!value) _Direction = Vector2.zero;
        }

        public virtual void StopControl(bool value) => _StopControl = value;

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            _Joystick.transform.position = eventData.position;
            Events.Publish(JoystickEvent.BeginDrag);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            Vector2 realDirection =
                Vector2.ClampMagnitude(eventData.position - (Vector2)_Joystick.transform.position, _Radius);

            _Direction = realDirection.normalized;
            _Handle.position = (Vector2)_Joystick.transform.position + realDirection;
            Events.Publish(JoystickEvent.Drag);
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            _Joystick.transform.position = _OriginPos;
            _Handle.localPosition = Vector2.zero;
            Events.Publish(JoystickEvent.EndDrag);
            _Direction = Vector2.zero;
        }
    }
}