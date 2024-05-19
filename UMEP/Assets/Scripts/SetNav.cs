using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Unity.AI.Navigation;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class SetNav : MonoBehaviour
{
    [SerializeField] private GameObject NavTarget;
    [SerializeField] private QrCodeRecenter QrCode;
    [SerializeField] private Button reroute;
    [SerializeField] private NavMeshSurface navSurface;
    [SerializeField] private GameObject[] cubes;
    [SerializeField] private GameObject capsule;
    [SerializeField] private TextMeshProUGUI Distance;

    private NavMeshPath path;
    private LineRenderer line;
    private GameObject closestCube; // Store the reference to the closest cube
    private GameObject previousClosestCube; // Store the reference to the previous closest cube
    private List<GameObject> sortedCubes = new List<GameObject>(); // Store the cubes sorted by distance
    private int cubeIndex = 0;

    void Start()
    {
        path = new NavMeshPath();
        line = transform.GetComponent<LineRenderer>();
        reroute.onClick.AddListener(Reroute);
    }

    void Reroute()
    {
        cubeIndex++;
        if (cubeIndex >= sortedCubes.Count)
        {
            // Handle the case when cubeIndex reaches the size of sortedCubes
            cubeIndex = 0; // For example, reset cubeIndex to 0
            //Debug.Log("cubeIndex reset to 0");
        }
        //Debug.Log("Current cubeIndex: " + cubeIndex);
    }

    void Update()
    {
        reroute.gameObject.SetActive(QrCode.isActivated);
        Distance.gameObject.SetActive(QrCode.isActivated);

        if (QrCode.isActivated)
        {
            if (cubeIndex == 0)
            { 
               FindAndSortCubesByDistance();
            }
            

            if (sortedCubes.Count > 0)
            {
                closestCube = sortedCubes[cubeIndex];
              
                ActivateCubes();

                if (previousClosestCube != closestCube)
                {
                    BakeNavMesh();
                    previousClosestCube = closestCube;
                }

                DrawPathToTarget(NavTarget.transform.position);
                DisplayDistanceToNavTarget();
            }
        }
    }

    void FindAndSortCubesByDistance()
    {
        sortedCubes = cubes.OrderBy(cube => Vector3.Distance(capsule.transform.position, cube.transform.position)).ToList();
    }

    void ActivateCubes()
    {
        for (int i = 0; i < sortedCubes.Count; i++)
        {
            sortedCubes[i].SetActive(i != cubeIndex); // Activate all cubes except the closest one
        }
    }

    void BakeNavMesh()
    {
        if (navSurface != null)
        {
            navSurface.BuildNavMesh();
        }
    }

    void DrawPathToTarget(Vector3 targetPosition)
    {
        if (NavMesh.CalculatePath(capsule.transform.position, targetPosition, NavMesh.AllAreas, path))
        {
            line.positionCount = path.corners.Length;
            line.SetPositions(path.corners);
            line.enabled = QrCode.isActivated;
        }
    }

    void DisplayDistanceToNavTarget()
    {
        // Calculate the distance using the path corners
        float pathDistance = 0f;
        if (path.corners.Length > 1)
        {
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                pathDistance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
        }

        int roundedDistance = Mathf.RoundToInt(pathDistance);
        // Debug.Log("Distance to NavTarget: " + roundedDistance);

        Distance.text = roundedDistance.ToString() + " m";
    }
}
