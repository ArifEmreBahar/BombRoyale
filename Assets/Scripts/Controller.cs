using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{

    private bool isMoving;
    private Vector3 origPos, targetPos;
    public float timeToMove = 0.1f;
    public float moveHorizontal, moveVertical;
    public bool isCross;
    public bool hitLeft, hitRight, hitBack, hitForward;
    public bool hitForwardRight, hitForwardLeft, hitBackRight, hitBackLeft;


    PhotonView view;

    void Start()
    {

        view = GetComponent<PhotonView>();
        if (view.IsMine)
        {
            GameObject cameraObj = new GameObject();
            cameraObj.transform.parent = transform;
            Camera camera = cameraObj.AddComponent<Camera>();

            camera.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            camera.transform.localPosition = new Vector3(0, 10, 0);
        }


        Color color = new Color(
          Random.Range(0f, 1f),
          Random.Range(0f, 1f),
          Random.Range(0f, 1f)
            );
        GetComponent<Renderer>().material.color = color;

    }


    void Update()
    {
        if (view.IsMine)
        {
            moveHorizontal = Input.GetAxisRaw("Horizontal");
            moveVertical = Input.GetAxisRaw("Vertical");

            //Is?n Kontrol
            RaycastHit hitControl;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hitControl, 1f))
            {
                hitRight = true;
            }
            else { hitRight = false; }
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hitControl, 1f))
            {
                hitLeft = true;
            }
            else { hitLeft = false; }
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out hitControl, 1f))
            {
                hitBack = true;
            }
            else { hitBack = false; }
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitControl, 1f))
            {
                hitForward = true;
            }
            else { hitForward = false; }

            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward + Vector3.right), out hitControl, 1f))
            {
                hitForwardRight = true;
            }
            else { hitForwardRight = false; }

            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward + Vector3.left), out hitControl, 1f))
            {
                hitForwardLeft = true;
            }
            else { hitForwardLeft = false; }

            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back + Vector3.right), out hitControl, 1f))
            {
                hitBackRight = true;
            }
            else { hitBackRight = false; }

            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back + Vector3.left), out hitControl, 1f))
            {
                hitBackLeft = true;
            }
            else { hitBackLeft = false; }

            // Is?n Control Son..............................................................................................................

            //Hareket Control

            if (moveVertical == 1f && moveHorizontal == 1f && !isMoving && hitForwardRight == false && hitForward == false && hitRight == false)
            {
                isCross = true;
                StartCoroutine(MovePlayer(Vector3.forward + Vector3.right));
            }
            else
            {
                isCross = false;
            }

            if (moveVertical == 1f && moveHorizontal == -1f && !isMoving && hitForwardLeft == false && hitForward == false && hitLeft == false)
            {
                isCross = true;
                StartCoroutine(MovePlayer(Vector3.forward + Vector3.left));
            }
            else
            {
                isCross = false;
            }

            if (moveVertical == -1f && moveHorizontal == 1f && !isMoving && hitBackRight == false && hitBack == false && hitRight == false)
            {
                isCross = true;
                StartCoroutine(MovePlayer(Vector3.back + Vector3.right));
            }
            else
            {
                isCross = false;
            }

            if (moveVertical == -1f && moveHorizontal == -1f && !isMoving && hitBackLeft == false && hitBack == false && hitLeft == false)
            {
                isCross = true;
                StartCoroutine(MovePlayer(Vector3.back + Vector3.left));
            }
            else
            {
                isCross = false;
            }

            if (moveHorizontal == 1f && !isMoving && isCross == false && hitRight == false)
            {
                StartCoroutine(MovePlayer(Vector3.right));
            }

            if (moveHorizontal == -1f && !isMoving && isCross == false && hitLeft == false)
            {
                StartCoroutine(MovePlayer(Vector3.left));
            }

            if (moveVertical == 1f && !isMoving && isCross == false && hitForward == false)
            {
                StartCoroutine(MovePlayer(Vector3.forward));
            }
            if (moveVertical == -1f && !isMoving && isCross == false && hitBack == false)
            {
                StartCoroutine(MovePlayer(Vector3.back));
            }

            //Hareket Control Son......
        }
    }

    private IEnumerator MovePlayer(Vector3 direction)
    {
        isMoving = true;

        float elapsedTime = 0;

        origPos = transform.position;
        targetPos = origPos + direction;

        while (elapsedTime < timeToMove)
        {
            transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;

        isMoving = false;
    }


}