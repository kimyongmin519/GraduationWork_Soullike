using KimLIb.ModuleSystems;
using Member.KYM.Scripts.Agents;
using UnityEngine;
using UnityEngine.UI;

namespace Member.KYM.Scripts.Players.UI
{
    public class PlayerStaminaBar : MonoBehaviour, IModule, IAfterInitModule
    {
        private StaminaModule _stamina;
        private GameObject _canvasObject;
        private RectTransform _fillRect;

        public void Initialize(ModuleOwner owner)
        {
            _stamina = owner.GetModule<StaminaModule>();
            Debug.Assert(_stamina != null, "PlayerStaminaBar requires StaminaModule.");
        }

        public void AfterInit()
        {
            if (_stamina == null)
                return;

            CreateTemporaryBar();
            _stamina.OnStaminaChanged += HandleStaminaChanged;
            HandleStaminaChanged(_stamina.CurrentStamina, _stamina.MaxStamina);
        }

        private void CreateTemporaryBar()
        {
            _canvasObject = new GameObject(
                "Temporary Stamina Canvas",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler));

            Canvas canvas = _canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;

            CanvasScaler scaler = _canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            GameObject backgroundObject = new GameObject(
                "Stamina Background",
                typeof(RectTransform),
                typeof(Image));
            backgroundObject.transform.SetParent(_canvasObject.transform, false);

            RectTransform backgroundRect =
                backgroundObject.GetComponent<RectTransform>();
            backgroundRect.anchorMin = new Vector2(0f, 1f);
            backgroundRect.anchorMax = new Vector2(0f, 1f);
            backgroundRect.pivot = new Vector2(0f, 1f);
            backgroundRect.anchoredPosition = new Vector2(24f, -24f);
            backgroundRect.sizeDelta = new Vector2(240f, 18f);

            Image background = backgroundObject.GetComponent<Image>();
            background.color = new Color(0f, 0f, 0f, 0.65f);
            background.raycastTarget = false;

            GameObject fillObject = new GameObject(
                "Stamina Fill",
                typeof(RectTransform),
                typeof(Image));
            fillObject.transform.SetParent(backgroundObject.transform, false);

            _fillRect = fillObject.GetComponent<RectTransform>();
            _fillRect.anchorMin = Vector2.zero;
            _fillRect.anchorMax = Vector2.one;
            _fillRect.offsetMin = new Vector2(2f, 2f);
            _fillRect.offsetMax = new Vector2(-2f, -2f);

            Image fill = fillObject.GetComponent<Image>();
            fill.color = Color.white;
            fill.raycastTarget = false;
        }

        private void HandleStaminaChanged(float current, float max)
        {
            if (_fillRect == null)
                return;

            float normalized = max <= 0f ? 0f : Mathf.Clamp01(current / max);
            _fillRect.anchorMax = new Vector2(normalized, 1f);
            _fillRect.gameObject.SetActive(normalized > 0f);
        }

        private void OnDestroy()
        {
            if (_stamina != null)
                _stamina.OnStaminaChanged -= HandleStaminaChanged;

            if (_canvasObject != null)
                Destroy(_canvasObject);
        }
    }
}
