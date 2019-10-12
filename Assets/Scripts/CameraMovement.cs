using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    [SerializeField]
    private GameObject _player;
    // Start is called before the first frame update

    private float offset = 6.0f;
    private bool isMovingRight = false;

    [SerializeField]
    private float anchorMovingRight;

    [SerializeField]
    private float anchorMovingLeft;

    [SerializeField]
    private float thresholdChangeAnchorMovingLeft;

    [SerializeField]
    private float thresholdChangeAnchorMovingRight;

    private bool isAnchorMovingRightActive = true;

    private Camera camera;
    private bool isMovingCamera = false;
    private float movingCameraTime = 1.0f;
    void Start()
    {
        camera = GetComponent<Camera>();

    }

    void DrawAnchorsAndThresholds()
    {
        Vector3 startPosAnchorMovingRight = new Vector3(camera.transform.position.x + anchorMovingRight, 5, 1);
        Vector3 endPosAnchorMovingRight = new Vector3(camera.transform.position.x + anchorMovingRight, -5, 1);
        Vector3 startPosAnchorMovingLeft = new Vector3(camera.transform.position.x + anchorMovingLeft, 5, 1);
        Vector3 endosAnchorMovingLeft = new Vector3(camera.transform.position.x + anchorMovingLeft, -5, 1);
        Vector3 startThresholdChangeAnchorMovingLeft = new Vector3(camera.transform.position.x + thresholdChangeAnchorMovingLeft, 5, 1);
        Vector3 endThresholdChangeAnchorMovingLeft = new Vector3(camera.transform.position.x + thresholdChangeAnchorMovingLeft, -5, 1);
        Vector3 startThresholdChangeAnchorMovingRight = new Vector3(camera.transform.position.x + thresholdChangeAnchorMovingRight, 5, 1);
        Vector3 endThresholdChangeAnchorMovingRight = new Vector3(camera.transform.position.x + thresholdChangeAnchorMovingRight, -5, 1);

        Debug.DrawLine(startPosAnchorMovingRight, endPosAnchorMovingRight, Color.white, 0, false);
        Debug.DrawLine(startPosAnchorMovingLeft, endosAnchorMovingLeft, Color.white, 0, false);
        Debug.DrawLine(startThresholdChangeAnchorMovingLeft, endThresholdChangeAnchorMovingLeft, Color.white, 0, false);
        Debug.DrawLine(startThresholdChangeAnchorMovingRight, endThresholdChangeAnchorMovingRight, Color.white, 0, false);
    }
    // Update is called once per frame
    void Update()
    {
        //DrawAnchorsAndThresholds();

        //Vector3 playerScreenPos = GetComponent<Camera>().WorldToScreenPoint(_player.transform.position);

        //float playerOffsetScreen = playerScreenPos.x - GetComponent<Camera>().pixelWidth / 2;

        float newCameraX = transform.position.x;
        float playerOffset = _player.transform.position.x - transform.position.x;

        if (isAnchorMovingRightActive)
        {
            if (playerOffset > anchorMovingRight)
            {
                //Debug.Log("ANCHOR RIGHT");
                newCameraX = _player.transform.position.x - anchorMovingRight;
            }
            if (playerOffset < thresholdChangeAnchorMovingLeft)
            {
                //Debug.Log("CHANGE ANCHOR LEFT");
                newCameraX = _player.transform.position.x - anchorMovingLeft;
                isAnchorMovingRightActive = false;
                isMovingCamera = true;
            }
        }
        else
        {
            if (playerOffset < anchorMovingLeft)
            {
                //Debug.Log("ANCHOR LEFT");

                newCameraX = _player.transform.position.x - anchorMovingLeft;
            }
            if (playerOffset > thresholdChangeAnchorMovingRight)
            {
                //Debug.Log("CHANGE ANCHOR RIGHT");

                newCameraX = _player.transform.position.x + anchorMovingRight;
                isAnchorMovingRightActive = true;
                isMovingCamera = true;
            }
        }






        if (playerOffset > anchorMovingLeft && !isAnchorMovingRightActive || playerOffset < anchorMovingRight && isAnchorMovingRightActive)
        {
            movingCameraTime = 1.0f;
            isMovingCamera = false;
        }

        if (isMovingCamera)
        {   
            newCameraX = 1.0f;
            if (!isAnchorMovingRightActive)
            {
                newCameraX = -1.0f;
            }

            newCameraX *= Time.deltaTime * 20.0f;
            newCameraX += transform.position.x;
            movingCameraTime -= Time.deltaTime;
            if (movingCameraTime < 0)
            {
                movingCameraTime = 1.0f;
                isMovingCamera = false;
            }
        }

        float newCameraY = transform.position.y;
        
        if (_player.GetComponent<Mario>().isFlying && _player.transform.position.y > 2.0 || newCameraY > 0) {
            transform.Translate(0, (_player.transform.position.y - newCameraY) * Time.deltaTime * 10f, 0);
        }
        
        if (newCameraX < 0)
        {
            //Debug.Log("MINUS ZERO");
            newCameraX = 0;
            movingCameraTime = 1.0f;
            isMovingCamera = false;
            transform.position = new Vector3(0, transform.position.y, transform.position.z);
            //isAnchorMovingRightActive = true;
            return;
        }
        if (newCameraX > 12)
        {
            //Debug.Log("MINUS ZERO");
            newCameraX = 12;
            movingCameraTime = 1.0f;
            isMovingCamera = false;
            transform.position = new Vector3(newCameraX, transform.position.y, transform.position.z);
            //isAnchorMovingRightActive = true;
            return;
        }

        
        transform.position = new Vector3(newCameraX, transform.position.y, transform.position.z);


    }
}
