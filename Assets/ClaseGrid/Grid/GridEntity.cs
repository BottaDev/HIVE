using System;
using UnityEngine;

//[ExecuteInEditMode]
public class GridEntity : MonoBehaviour
{
	public event Action<GridEntity> OnMove = delegate {};
    public bool onGrid;

    void Update() {
        
	    OnMove(this);
	}
}
