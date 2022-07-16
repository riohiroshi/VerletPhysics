using UnityEngine;

public class VerletObject : MonoBehaviour
{
    [field: SerializeField] public float Radius { get; private set; }

    public Vector3 PositionCurrent { get; private set; }
    public Vector3 PositionPrevious { get; private set; }
    public Vector3 Acceleration { get; private set; }

    private void Start()
    {
        PositionCurrent = PositionPrevious = transform.position;
    }

    private void LateUpdate()
    {
        transform.position = PositionCurrent;
    }

    public void SetRadius(float value)
    {
        Radius = value;
        transform.localScale = value * 2 * Vector3.one;
    }

    public void SetPositionCurrent(Vector3 value) => PositionCurrent = value;

    public void UpdatePosition(float deltaTime)
    {
        var velocity = PositionCurrent - PositionPrevious;

        PositionPrevious = PositionCurrent;

        SetPositionCurrent(PositionCurrent + velocity + 0.5f * deltaTime * deltaTime * Acceleration);

        Acceleration = Vector3.zero;
    }

    public void Accelerate(Vector3 acceleration)
    {
        Acceleration += acceleration;
    }
}