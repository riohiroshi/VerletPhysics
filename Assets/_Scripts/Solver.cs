using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solver : MonoBehaviour
{
    [SerializeField] private VerletObject _verletObjectPrefab;
    [SerializeField] private VerletObject[] _verletObjectArray;

    [SerializeField] private Vector3 _gravity = new Vector3(0f, -10f, 0f);

    [SerializeField] private Transform _container;

    [Min(1f)]
    [SerializeField] private float _containerRadius = 2f;

    [Min(1)]
    [SerializeField] private int _subSteps = 8;

    [Min(1)]
    [SerializeField] private int _objectAmount = 1000;

    [Min(0.01f)]
    [SerializeField] private float _verletObjectRadius = 0.1f;

    private List<VerletObject> _verletObjectList;

    private IEnumerator Start()
    {
        _verletObjectList = new List<VerletObject>();

        var popPoint = _container.position + new Vector3(0f, _containerRadius * 1.5f, 0f);

        for (int i = 0; i < _objectAmount; i++)
        {
            var verletObject = Instantiate(_verletObjectPrefab, popPoint, Quaternion.identity, _container);

            verletObject.SetRadius(Random.Range(_verletObjectRadius * 0.5f, _verletObjectRadius * 1.5f));

            verletObject.Accelerate(Random.onUnitSphere);

            _verletObjectList.Add(verletObject);

            _verletObjectArray = _verletObjectList.ToArray();
            yield return null;
        }
    }

    private void Update()
    {
        UpdateSolver(Time.deltaTime);
    }

    private void UpdateSolver(float deltaTime)
    {
        var subDeltaTime = deltaTime / _subSteps;
        for (int i = 0; i < _subSteps; i++)
        {
            ApplyGravity();
            ApplyConstraint();
            SolveCollisions();
            UpdatePositions(subDeltaTime);
        }
    }

    private void UpdatePositions(float deltaTime)
    {
        foreach (var obj in _verletObjectArray)
        {
            obj.UpdatePosition(deltaTime);
        }
    }

    private void ApplyGravity()
    {
        foreach (var obj in _verletObjectArray)
        {
            obj.Accelerate(_gravity);
        }
    }

    private void ApplyConstraint()
    {
        var position = _container.position + new Vector3(0f, _containerRadius, 0f);

        foreach (var obj in _verletObjectArray)
        {
            var toObj = obj.PositionCurrent - position;
            var distance = toObj.magnitude;
            if (distance <= _containerRadius - obj.Radius) { continue; }

            var direction = toObj.normalized;
            obj.SetPositionCurrent(position + direction * (_containerRadius - obj.Radius));
        }
    }

    private void SolveCollisions()
    {
        var count = _verletObjectArray.Length;
        for (int i = 0; i < count; i++)
        {
            var obj1 = _verletObjectArray[i];
            for (int j = i + 1; j < count; j++)
            {
                var obj2 = _verletObjectArray[j];
                var collisionAxis = obj1.PositionCurrent - obj2.PositionCurrent;
                var distance = collisionAxis.magnitude;
                if (distance > obj1.Radius + obj2.Radius) { continue; }

                var direction = collisionAxis.normalized;
                var delta = obj1.Radius + obj2.Radius - distance;
                obj1.SetPositionCurrent(obj1.PositionCurrent + 0.5f * delta * direction);
                obj2.SetPositionCurrent(obj2.PositionCurrent - 0.5f * delta * direction);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_container == null) { return; }

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(_container.position + new Vector3(0f, _containerRadius, 0f), _containerRadius);
    }
}