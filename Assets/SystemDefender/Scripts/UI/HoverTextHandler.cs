using SystemDefender.Scripts.Constants;
using SystemDefender.Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SystemDefender.Scripts.UI
{
    public class HoverTextHandler : MonoBehaviour
    {
        [SerializeField]
        private Vector2 _textOffset = new Vector2(10, 10);

        [SerializeField]
        private Vector2 _mouseOffset = new Vector2(15, 15);

        [SerializeField]
        private GameObject _textObject;

        [SerializeField]
        private PlayerStatTypes _playerStatType;

        private PlayerController _playerController = null;

        private bool _isActive = false;


        private void Start()
        {
            _textObject.SetActive(false);

            _playerController = PlayerController.Instance;

            if (_playerController is null)
            {
                Debug.LogError("Player controller is null");
            }
        }

        public void OnPointerEnter()
        {
            _isActive = true;
            _textObject.SetActive(true);
            UpdateTextPosition();
        }

        public void OnPointerExit()
        {
            _isActive = false;
            _textObject.SetActive(false);
        }

        void Update()
        {
            if (_isActive)
            {
                UpdateTextPosition();
            }
        }

        void UpdateTextPosition()
        {
            Vector2 mousePosition = Input.mousePosition;

            Vector2 textPosition = mousePosition + _mouseOffset;

            RectTransform textRect = _textObject.GetComponent<RectTransform>();
            TextMeshProUGUI textComponent = _textObject.GetComponentInChildren<TextMeshProUGUI>();

            string currentText = getText();

            Vector2 textSize = textComponent.GetPreferredValues(currentText) + _textOffset;
            textComponent.text = currentText;
            textRect.sizeDelta = textSize;

            Vector2 screenBounds = new Vector3(Screen.width, Screen.height);

            if (textPosition.x + textSize.x > screenBounds.x)
            {
                textPosition.x = screenBounds.x - _mouseOffset.x - textSize.x;
            }
            else if (textPosition.x < 0)
            {
                textPosition.x = _mouseOffset.x;
            }

            if (textPosition.y + textSize.y > screenBounds.y)
            {
                textPosition.y = screenBounds.y - _mouseOffset.y - textSize.y;
            }
            else if (textPosition.y < 0)
            {
                textPosition.y = _mouseOffset.y;
            }

            _textObject.GetComponent<RectTransform>().position = textPosition;

        }

        private string getText()
        {
            switch (_playerStatType)
            {
                case PlayerStatTypes.RAM:
                    return _playerController.getRamHoverText();
                case PlayerStatTypes.CPU:
                    return _playerController.getCpuHovertext();
                case PlayerStatTypes.GPU:
                    return _playerController.getGpuHovertext();
                case PlayerStatTypes.SCP:
                    return _playerController.getSCPHovertext();
                default:
                    return "Error";
            }
        }
    }
}