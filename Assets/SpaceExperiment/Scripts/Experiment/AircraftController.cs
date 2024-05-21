using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftController : MonoBehaviour
{
    private Rigidbody rb;
    public float airDensity;
    public float wingArea, aileronArea, elevatorArea, finArea;
    public Vector3
        rightWingPosition, leftWingPosition,
        rightaileronPosition, leftaileronPosition,
        rightElevatorPosition, leftElevatorPosition,
        finPosition;
    public float throttleCoefficient;
    public float pitchInputMax, yawInputMax, rollInputMax;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float throttleInput = 0.0f;
        if (Input.GetKey(KeyCode.J))
        {
            throttleInput = 1.0f;
        }
        rb.AddForce(throttleInput * throttleCoefficient * transform.forward);

        float pitchInput = -Input.GetAxis("Vertical");
        float yawInput = Input.GetAxis("Horizontal");
        float rollInput = 0.0f;
        if (Input.GetKey(KeyCode.Q))
        {
            rollInput = 1.0f;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            rollInput = -1.0f;
        }
        pitchInput = Mathf.Clamp(pitchInput, -pitchInputMax, pitchInputMax) * Mathf.PI / 180;
        yawInput = Mathf.Clamp(yawInput, -yawInputMax, yawInputMax) * Mathf.PI / 180;
        rollInput = Mathf.Clamp(rollInput, -rollInputMax, rollInputMax) * Mathf.PI / 180;

        if (rb.velocity.magnitude > 0.0f)
        {
            addForce(getHorizontalWingForce(0, wingArea), rightWingPosition);
            addForce(getHorizontalWingForce(0, wingArea), leftWingPosition);
            addForce(getHorizontalWingForce(rollInput, aileronArea), rightaileronPosition);
            addForce(getHorizontalWingForce(-rollInput, aileronArea), leftaileronPosition);
            addForce(getHorizontalWingForce(pitchInput, elevatorArea), rightElevatorPosition);
            addForce(getHorizontalWingForce(pitchInput, elevatorArea), leftElevatorPosition);
            if(Input.GetKey(KeyCode.A)|| Input.GetKey(KeyCode.D))
                addForce(getVerticalWingForce(yawInput, finArea), getVerticalWingPosition(yawInput, finPosition));
        }

    }

    private Vector3 getHorizontalWingForce(float angleInput, float area)
    {
        Vector3 localVelocity = transform.InverseTransformDirection(-rb.velocity);
        float angleOfVelocity = Mathf.Atan(localVelocity.y / -localVelocity.z);
        float angelOfAttack = angleInput + angleOfVelocity;
        float dynamicPressure = 0.5f * airDensity * localVelocity.sqrMagnitude;

        Vector3 dragDirection = -rb.velocity.normalized;
        Vector3 drag = dragDirection * dragCurve(angelOfAttack) * dynamicPressure * area;

        Vector3 liftDirection = Vector3.Cross(transform.right, dragDirection).normalized;
        Vector3 lift = liftDirection * liftCurve(angelOfAttack) * dynamicPressure * area;

        return drag + lift;
    }

    private Vector3 getVerticalWingForce(float angleInput, float area)
    {
        Vector3 localVelocity = transform.InverseTransformDirection(-rb.velocity);
        //float angleOfVelocity = Mathf.Atan(localVelocity.x / -localVelocity.z);
        //float angelOfAttack = angleInput + angleOfVelocity;
        float dynamicPressure = 0.5f * airDensity * localVelocity.sqrMagnitude;

        Vector3 dragDirection = -rb.velocity.normalized;
        Vector3 drag = dragDirection * dragCurve(angleInput) * dynamicPressure * area;

        return drag;
    }

    private Vector3 getVerticalWingPosition(float angleInput, Vector3 position)
    {
        position.x += angleInput * 10;
        return position;
    }

    private void addForce(Vector3 force, Vector3 position)
    {
        Vector3 worldPosition = transform.position + transform.forward * position.z + transform.up * position.y + transform.right * position.x;
        rb.AddForceAtPosition(force, worldPosition);
    }

    private float dragCurve(float angle)
    {
        return (-Mathf.Cos(2 * angle) + 1) / 2;
    }

    private float liftCurve(float angle)
    {
        return -Mathf.Cos(2 * angle + 3 * Mathf.PI / 5);
    }
}
