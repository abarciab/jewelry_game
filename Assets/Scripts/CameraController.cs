using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraMode { None, Environment, Crafting}

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform camTransform;
    [SerializeField] private CameraMode _currentMode;

    [Header("Crafting")]
    [SerializeField] private Quaternion _craftingRot;
    [SerializeField] private Vector3 _craftingPosition;

    [Header("Environment")]
    [SerializeField] private Quaternion _environmentRot;
    [SerializeField] private Vector3 _environmentStartPosition;
    [SerializeField] private float _moveSmoothness = 0.1f;
    [SerializeField] Vector4 _panLimits;
    private Vector3 _touchStart;
    private Vector3 _directionDelta;

    [Header("Camera Shake")]
    [SerializeField] bool _2D;
    [SerializeField] float defaultShakeRadius;
    [SerializeField] float defaultShakeTime;
    [SerializeField] float defaultShakeSpeed;
    [SerializeField] float distCutoff = 0.01f;
    [SerializeField] float radiusDieMod;
    [SerializeField] float radiusThreshold = 0.01f;
    [SerializeField] AnimationCurve smoothnessCuve;
    bool shaking;
    Vector3 currentTarget;
    Vector3 oldPos;
    float currentDist, timeLeftThisShake, timeTotalThisShake;

    private void Update()
    {
        if (shaking) return;

        if (_currentMode == CameraMode.Environment) PanCamera();        
    }

    private void PanCamera()
    {
        var touchPos = MouseToWorldPos();

        if (Input.GetMouseButtonDown(0)) _touchStart = touchPos;
        if (Input.GetMouseButton(0)) _directionDelta = _touchStart - touchPos;
        else _directionDelta = Vector3.Lerp(_directionDelta, Vector3.zero, _moveSmoothness);
        _directionDelta.y = 0;

        transform.position += _directionDelta;

        ClampPosition();
    }

    private Vector3 MouseToWorldPos()
    {
        var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(mouseRay, out var hitInfo);

        Debug.DrawRay(mouseRay.origin, mouseRay.direction);

        return hitInfo.point;
    }

    private void ClampPosition()
    {
        var pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, _panLimits.x, _panLimits.y);
        pos.z = Mathf.Clamp(pos.z, _panLimits.z, _panLimits.w);

        transform.position = pos;
    }

    [ButtonMethod]
    public void SetEnvironmentPositionAndRotation()
    {
        _environmentRot = transform.rotation;
        _environmentStartPosition = transform.position;
    }

    [ButtonMethod]
    public void SetCraftingPositionAndRotation()
    {
        _craftingRot = transform.rotation;
        _craftingPosition = transform.position;
    }

    [ButtonMethod]
    public void EnterEnvironmentMode()
    {
        transform.rotation = _environmentRot;
        transform.position = _environmentStartPosition;
        _currentMode = CameraMode.Environment;
    }

    [ButtonMethod]
    public void EnterCraftingMode()
    {
        _environmentStartPosition = transform.position;
        transform.rotation = _craftingRot;
        transform.position = _craftingPosition;
        _currentMode = CameraMode.Crafting;
    }

    [ButtonMethod]
    public void ShakeFixed()
    {
        Shake(defaultShakeRadius, defaultShakeSpeed, defaultShakeTime);
    }

    [ButtonMethod]
    public void ShakeGradual()
    {
        Shake(defaultShakeRadius, defaultShakeSpeed);
    }


    public void Shake(float radius, float speed, float time = 0)
    {
        StopAllCoroutines();
        shaking = true;
        StartCoroutine(AnimateShake(radius, speed, time));
    }

    IEnumerator AnimateShake(float radius, float speed, float time = 0)
    {
        var originalPos = camTransform.localPosition;
        var newRadius = SetPos(radius, speed);
        if (time == 0) radius = newRadius;
        while (time == 0 ? radius > radiusThreshold : time > 0) {
            float dist = Vector3.Distance(camTransform.localPosition, currentTarget);
            if (dist < distCutoff) {
                newRadius = SetPos(radius, speed);
                if (time == 0) radius = newRadius;
            }

            float progress = smoothnessCuve.Evaluate(timeLeftThisShake / timeTotalThisShake);
            camTransform.localPosition = Vector3.Lerp(oldPos, currentTarget, progress);

            timeLeftThisShake -= Time.deltaTime;
            if (time != 0) time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        timeTotalThisShake = Vector3.Distance(camTransform.localPosition, originalPos) / speed;
        timeLeftThisShake = timeTotalThisShake;
        while (timeLeftThisShake > 0) {
            float progress = smoothnessCuve.Evaluate(1 - timeLeftThisShake / timeTotalThisShake);
            camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, originalPos, progress);
            timeLeftThisShake -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        camTransform.localPosition = originalPos;

        shaking = false;
    }

    float SetPos(float radius, float speed)
    {
        oldPos = camTransform.localPosition;
        var pos = Random.insideUnitSphere.normalized * radius;
        if (_2D) pos.z = 0;
        currentTarget = pos;
        currentDist = Vector3.Distance(oldPos, currentTarget);
        timeTotalThisShake = currentDist / speed;
        timeLeftThisShake = timeTotalThisShake;
        return radius * radiusDieMod;
    }

    private void OnDrawGizmosSelected()
    {
        DrawGizmoRectangle();
    }

    private void DrawGizmoRectangle()
    {
        Gizmos.color = Color.green;

        Vector3 topLeft = new Vector3(_panLimits.x, transform.position.y, _panLimits.z);
        Vector3 topRight = new Vector3(_panLimits.y, transform.position.y, _panLimits.z);
        Vector3 bottomRight = new Vector3(_panLimits.y, transform.position.y, _panLimits.w);
        Vector3 bottomLeft = new Vector3(_panLimits.x, transform.position.y, _panLimits.w);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}
