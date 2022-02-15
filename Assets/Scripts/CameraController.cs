using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    public int cameraSettings = -1;
    public List<CameraSettings> settings;
    public bool won;
    [SerializeField]
    Transform player;
    [SerializeField]
    Vector3 cameraOffset;
    [SerializeField]
    float cameraCatchUpDistance = 2f;
    [SerializeField]
    float cameraSpeed = 1f;

    Vector3? cameraDestination;

    private void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (player != null)
        {
            Vector3 destination = player.position + cameraOffset;
            if (Mathf.Abs(destination.z - transform.position.z) > cameraCatchUpDistance || Mathf.Abs(destination.x - transform.position.x) > cameraCatchUpDistance)
                cameraDestination = destination;
            //if (cameraDestination != null && (Mathf.Abs(destination.z - transform.position.z) < cameraCatchUpDistance / 2 || Mathf.Abs(destination.x - transform.position.x) < cameraCatchUpDistance / 2))
            //    cameraDestination = null;
            if (cameraDestination != null)
                transform.position = Vector3.Lerp(transform.position, cameraDestination.Value, cameraSpeed * Time.deltaTime);
        }
    }

    public void ChangeSettings (int _settings)
    {
        cameraSettings = _settings;
        FocusOnPlayer();
    }

    public void IncreaseOffsetY()
    {
        cameraOffset = new Vector3(cameraOffset.x, cameraOffset.y + 0.2f, cameraOffset.z);
    }

    public void AddYOffset()
    {
        cameraOffset.y += 0.1f;
        cameraOffset.z -= 0.1f;
    }

    public void FocusOnPlayer()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player != null)
        {
            if (cameraSettings > -1)
            {
                cameraOffset = settings[cameraSettings].offset;
                transform.DORotate(settings[cameraSettings].angle, 0.75f);
                transform.DOMove(player.position + cameraOffset, 0.75f);
            }
            cameraDestination = null;
        }
        //transform.position = player.position + cameraOffset;
    }
}

[System.Serializable]
public class CameraSettings
{
    public Vector3 offset;
    public Vector3 angle;
}