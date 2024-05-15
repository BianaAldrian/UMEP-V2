using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Unity.AI.Navigation;
using System.Collections.Generic;
using System.Linq;

public class SetNav : MonoBehaviour
{
    [SerializeField] private GameObject NavTarget;
    [SerializeField] private QrCodeRecenter QrCode;
    [SerializeField] private Button reroute;
    [SerializeField] private NavMeshSurface navSurface;
    [SerializeField] private GameObject[] cubes;
    [SerializeField] private GameObject capsule;

    private NavMeshPath path;
    private LineRenderer line;
    private GameObject closestCube; // Store the reference to the closest cube
    private GameObject secondClosestCube; // Store the reference to the second closest cube
    private GameObject thirdClosestCube; // Store the reference to the third closest cube
    private GameObject previousClosestCube; // Store the reference to the previous closest cube
    private List<GameObject> sortedCubes = new List<GameObject>(); // Store the cubes sorted by distance

    void Start()
    {
        path = new NavMeshPath();
        line = transform.GetComponent<LineRenderer>();
        reroute.onClick.AddListener(BakeNavMesh);
    }

    void Reroute()
    {
    }

    void Update()
    {
        if (QrCode.isActivated)
        {
            FindAndSortCubesByDistance();

            if (sortedCubes.Count > 0)
            {
                closestCube = sortedCubes[0];
                secondClosestCube = sortedCubes.Count > 1 ? sortedCubes[1] : null;
                thirdClosestCube = sortedCubes.Count > 2 ? sortedCubes[2] : null;

                Debug.Log("closestCube" + closestCube.name);
                Debug.Log("secondClosestCube" + secondClosestCube.name);
                Debug.Log("thirdClosestCube" + thirdClosestCube.name);

                ActivateCubes();

                if (previousClosestCube != closestCube)
                {
                    BakeNavMesh();
                    previousClosestCube = closestCube;
                }

                DrawPathToTarget(NavTarget.transform.position);
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
            sortedCubes[i].SetActive(i != 0); // Activate all cubes except the closest one
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
}
