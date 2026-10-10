using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SystemDefender.Scripts.UI
{
    public class BarController : MonoBehaviour
    {
        private Slider _slider = null;
        private Image _sliderFillImage = null;

        [SerializeField]
        private List<Color> _palette = null;
        [SerializeField]
        private int _paletteSize = -1;

        [SerializeField]
        private float _currentValue = 0;
        [SerializeField]
        private float _maxValue = 1;
        [SerializeField]
        private float _minValue = 0;

        [SerializeField]
        private bool _isInverted = false;


        public float currentValue
        {
            get { return _currentValue; }
            set
            {
                if (value < _minValue)
                {
                    //Debug.LogError("Current value is smaller than min value\n" + gameObject.name);
                    _currentValue = _minValue;
                }
                else if (value > _maxValue)
                {
                    //Debug.LogError("Current value is bigger than max value\n" + gameObject.name);
                    _currentValue = _maxValue;
                }
                else _currentValue = value;
            }
        }

        public float maxValue
        {
            get { return _maxValue; }
            set
            {
                if (value < _currentValue)
                {
                    Debug.LogWarning("Max value is smaller than current value\n" + gameObject.name);
                    currentValue = _minValue;
                }
                else if (value < _minValue)
                {
                    Debug.LogError("Max value is smaller than min value\n" + gameObject.name);
                }
                else _maxValue = value;
            }
        }

        public float minValue
        {
            get { return _minValue; }
            set
            {
                if (value > _currentValue)
                {
                    Debug.LogWarning("Min value is bigger than current value\n" + gameObject.name);
                    currentValue = value;
                }
                else if (value > _maxValue)
                {
                    Debug.LogError("Min value is bigger than max value\n" + gameObject.name);
                }
                else _minValue = value;
            }
        }

        public void setPalette(List<Color> __palette)
        {
            _palette = new List<Color>(__palette);
            _paletteSize = _palette.Count;
        }

        public void initialization(float __currentValue = 0, float __minValue = 0, float __maxValue = 1)
        {
            currentValue = __currentValue;
            minValue = __minValue;
            maxValue = __maxValue;
        }

        private void Start()
        {
            _slider = gameObject.GetComponent<Slider>();

            if (_slider is null)
            {
                Debug.LogError("Slider cannot be found\n" + gameObject.name);
            }
            else
            {
                _sliderFillImage = _slider.fillRect.GetComponent<Image>();
            }

            _paletteSize = _palette.Count;
        }

        private void FixedUpdate()
        {
            float calculatedValue = (_currentValue - _minValue) / (_maxValue - minValue);

            Color colorA = Color.green;
            Color colorB = Color.red;
            float lerpValue = calculatedValue;

            if (_palette is not null && _paletteSize >= 2)
            {
                float index = _paletteSize * calculatedValue;

                int indexA = (int)MathF.Max(0, Mathf.FloorToInt(index) - 1);
                int indexB = (int)MathF.Max(0, MathF.Min(_paletteSize - 1, Mathf.CeilToInt(index) - 1));

                lerpValue -= indexA / (_paletteSize - 1);

                colorA = _palette[indexA];
                colorB = _palette[indexB];
            }

            colorA.a = 255;
            colorB.a = 255;

            if (_isInverted)
            {
                _slider.value = 1 - calculatedValue;
                _sliderFillImage.color = Color.Lerp(colorB, colorB, lerpValue);
            }
            else
            {
                _slider.value = calculatedValue;
                _sliderFillImage.color = Color.Lerp(colorA, colorB, lerpValue);
            }
        }
    }
}