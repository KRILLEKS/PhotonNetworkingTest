using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace Services.GameScene.Input
{
    public class Input_Service : IInput_Service, IInitializable, IDisposable
    {
        private readonly Subject<Vector2> _onLeftClick = new ();
        private readonly CompositeDisposable _disposables = new ();
    
        private Camera _mainCamera;
    
        public IObservable<Vector2> OnLeftClick
        {
            get
            {
                return _onLeftClick;
            }
        }

        public void Initialize()
        {
            _mainCamera = Camera.main;
        
            if (_mainCamera == null)
            {
                Debug.LogError("Main camera not found!");
                return;
            }

            // Observe left mouse button clicks using UniRx
            Observable.EveryUpdate()
                      .Where(_ => UnityEngine.Input.GetMouseButtonDown(0))
                      .Subscribe(_ => HandleLeftClick())
                      .AddTo(_disposables);
        }

        private void HandleLeftClick()
        {
            Vector2 mousePosition = UnityEngine.Input.mousePosition;
            Vector2 worldPosition = GetWorldPosition(mousePosition);
            _onLeftClick.OnNext(worldPosition);
        }

        private Vector2 GetWorldPosition(Vector2 screenPosition)
        {
            // Convert screen position to world position
            Vector3 worldPos = _mainCamera.ScreenToWorldPoint(
                                                              new Vector3(screenPosition.x, screenPosition.y, _mainCamera.nearClipPlane));
        
            return worldPos;
        }

        public void Dispose()
        {
            _disposables.Dispose();
            _onLeftClick?.Dispose();
        }}
}