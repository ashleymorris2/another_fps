using System;
using UnityEngine;

namespace ToExport.Scripts.Core
{
    public class GameObjectFactory<T>
    {
        private readonly GameObject _gameObject;
        private readonly Action<T> _factoryMethod;

        public GameObjectFactory(Action<T> factoryMethod, GameObject gameObjectPrefab = null)
        {
            _factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));
            _gameObject = gameObjectPrefab == null ? new GameObject() : gameObjectPrefab;
        }

        public GameObject Instantiate(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            GameObject instance;

            if (parent != null)
                instance = GameObject.Instantiate(_gameObject, position, rotation, parent);
            else
                instance = GameObject.Instantiate(_gameObject, position, rotation);

            _factoryMethod(_gameObject.GetComponent<T>());

            return instance;
        }
    }
}