using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

//IA2-P2
//esta en varios lados pero te podes fijar por los usages y eso.
public class SpatialGridManager : MonoBehaviour
{
    public static SpatialGridManager i;
    public Queries queryPrefab;
    public SpatialGrid grid;

    private void Awake()
    {
        i = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            grid.Refresh();
        }
    }

    public void AttachEntity(GridEntity entity)
    {
        entity.transform.parent = grid.transform;
        grid.Refresh();
    }

    public List<GridEntity> Query(Vector3 point, float radius)
    {
        Queries query = Instantiate(queryPrefab, transform);
        query.targetGrid = grid;
        query.transform.position = point;
        query.radius = radius;
        List<GridEntity> result = query.Query().ToList();
        
        Destroy(query.gameObject);
        return result;
    }
}
