using System.Collections.Generic;
using SystemDefender.Scripts.Services.Abstractions;
using SystemDefender.Scripts.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SystemDefender.Scripts.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviourSinglton<PlayerController>
    {
        private CharacterController _characterController;


        [SerializeField]
        private float _playerSpeed = 2f;
        private Vector2 _moveInput;
        private Vector3 _movement;

        [SerializeField]
        private float _ramCapacity;
        [SerializeField]
        private float _ramReliefPerFrame;
        [SerializeField]
        private float _cpuCapacity;
        [SerializeField]
        private float _cpuDamagePerClick;
        [SerializeField]
        private float _cpuReliefPerFrame;
        [SerializeField]
        private float _gpuCapacity;
        [SerializeField]
        private float _systemCapacity;

        [SerializeField]
        private BarController _ramBarController;
        [SerializeField]
        private BarController _cpuBarController;
        [SerializeField]
        private BarController _gpuBarController;
        [SerializeField]
        private BarController _systemBarController;


        [SerializeField]
        private GameObject _bulletPrefab;

        [SerializeField]
        private AudioClip _bulletSpawn;
        [SerializeField]
        private AudioClip _damageSound;

        [SerializeField]
        private GameObject _gameOverScreen;

        [SerializeField]
        private GameObject _turret;

        [SerializeField]
        private GameObject _tutorial;

        private bool isPressed = false;

        private void Start()
        {
            _characterController = GetComponent<CharacterController>();

            _ramBarController.initialization(0, 0, _ramCapacity);
            _cpuBarController.initialization(0, 0, _cpuCapacity);
            _gpuBarController.initialization(0, 0, _gpuCapacity);
            _systemBarController.initialization(0, 0, _systemCapacity);

            Destroy(_tutorial, 5f);
        }

        private void OnMove(InputValue __value)
        {
            _moveInput = __value.Get<Vector2>();
        }

        private void Update()
        {
            _movement.x = _moveInput.x * _playerSpeed;
            _movement.z = _moveInput.y * _playerSpeed;

            _characterController.Move(_movement * Time.deltaTime);

            cpuRelief();
            ramRelief();
        }

        private void OnClick(InputValue __value)
        {
            if (isPressed)
            {
                isPressed = false;

                return;
            }

            isPressed = true;

            if (_cpuBarController.currentValue + _cpuDamagePerClick >= _cpuCapacity)
            {
                return;
            }

            cpuUse();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit3D;
            bool hitWorld = Physics.Raycast(ray, out hit3D, Mathf.Infinity);

            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            if (hitWorld || results.Count > 0)
            {
                Vector3 spawnPosition;

                if (results.Count > 0)
                {
                    RaycastResult uiHit = results[0];
                    spawnPosition = uiHit.worldPosition;
                }
                else
                {
                    spawnPosition = hit3D.point;
                }

                GameObject newBullet = Instantiate(_bulletPrefab, spawnPosition, Quaternion.identity);

                float deltaX = _turret.transform.position.x - spawnPosition.x;
                float deltaZ = _turret.transform.rotation.z - spawnPosition.z;

                float angle = Mathf.Atan2(deltaX, deltaZ) * Mathf.Rad2Deg;

                angle = Mathf.Clamp(angle, -45, 45);

                _turret.transform.rotation = Quaternion.Euler(0, 0, angle);

                Destroy(newBullet, 0.5f);
            }

            AudioSource.PlayClipAtPoint(_bulletSpawn, transform.position, 60f);
        }

        private void cpuUse()
        {
            _cpuBarController.currentValue += _cpuDamagePerClick;
        }

        private void cpuRelief()
        {
            _cpuBarController.currentValue -= _cpuReliefPerFrame * Time.deltaTime;
        }

        private void ramRelief()
        {
            _ramBarController.currentValue -= _ramReliefPerFrame * Time.deltaTime;
        }

        public void enemyInView(float __gpuDamage)
        {
            float currentValue = _gpuBarController.currentValue + __gpuDamage;
            _gpuBarController.currentValue = currentValue;

            if (currentValue >= _gpuCapacity)
            {
                systemDamage(currentValue - _gpuCapacity);
            }
        }

        public void enemyOutOfView(float __gpuDamage)
        {
            _gpuBarController.currentValue -= __gpuDamage;
        }

        public void systemDamage(float __damage)
        {
            float currentRamValue = _ramBarController.currentValue;

            AudioSource.PlayClipAtPoint(_damageSound, transform.position, 30f);

            if (currentRamValue <= _ramCapacity)
            {
                if (currentRamValue + __damage <= _ramCapacity)
                {
                    _ramBarController.currentValue += __damage;

                    return;
                }

                _ramBarController.currentValue = _ramCapacity;

                _systemBarController.currentValue += currentRamValue + __damage - _ramCapacity;
            }


            if (_systemBarController.currentValue >= _systemCapacity)
            {
                _gameOverScreen.SetActive(true);
            }
        }

        public string getRamHoverText()
        {
            return $"RAM a.k.a. Temporal shield, will accept damage while is not full\nafter this point damage will corrupt system, avoid it\nRegeneration: {_ramReliefPerFrame}% per second\nCurrent value: {_ramBarController.currentValue}%";
        }

        public string getCpuHovertext()
        {
            return $"CPU a.k.a. limit of click attack, will rise with every click while will not be full\nafter this point you will not be able to attack for a time, avoid it\n Regeneration: {_cpuReliefPerFrame}% per second\nCurrent value: {_cpuBarController.currentValue}%";
        }

        public string getGpuHovertext()
        {
            return $"GPU a.k.a. limit of ad on the screen, will rise with every new ad in view while will not be full\nafter this point you will be accept damage from every new ad, avoid it\nCurrent value: {_gpuBarController.currentValue}%";
        }

        public string getSCPHovertext()
        {
            return $"System corruption a.k.a. your health will lower from damage while value will not be zero\nafter this point you will failure, avoid it\nCurrent value: {_systemBarController.currentValue}%";
        }
    }
}