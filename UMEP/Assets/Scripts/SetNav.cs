using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SetNav : MonoBehaviour
{
    [SerializeField]
    private GameObject NavTarget;

    private NavMeshPath path;
    private LineRenderer line;

    public QrCodeRecenter QrCode;

    // Start is called before the first frame update
    void Start()
    {
        path = new NavMeshPath();
        line = transform.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        NavMesh.CalculatePath(transform.position, NavTarget.transform.position, NavMesh.AllAreas, path);
        line.positionCount = path.corners.Length;
        line.SetPositions(path.corners);

        if(QrCode.isActivated)
        {
            line.enabled = true;
        } 
        else
        {
            line.enabled = false;
        }
    }
}
