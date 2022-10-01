using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class MovementOnClick : MonoBehaviour
{
    [SerializeField][Range(0f,5f)] private float step = 1f;
    [SerializeField] private GameObject _pointOnField ;
    [SerializeField] private LineRenderer _line => GetComponent<LineRenderer>()  ;

    private List<GameObject> pointers = new List<GameObject>();
    private Vector3 _clickMouse ;
    private int point = 0;
    private bool _isDead = false;

    private const int NUMBER_PLAYER_FOR_LINERENDER = 0;
    private const int NUMBER_NEXT_DOT_FOR_LINERENDER = 1;
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            GenerateNewPointOnTheField();
        }
    }

    private void GenerateNewPointOnTheField()
    {
        _clickMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _clickMouse.z = transform.position.z;
        GameObject temp = Instantiate(_pointOnField,_clickMouse,Quaternion.identity);
        temp.transform.SetParent(transform.parent);
        pointers.Add(temp);
    }

    private void FixedUpdate()
    {
        if(_isDead != true)
        {
            MovePlayer();
        }
    }   

    private void MovePlayer()
    {
        if(pointers.Count > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position,pointers[point].transform.position,step * Time.deltaTime);

            LineRenderer();
            DeletePoint();          
            ResetPoint();
        } 
    }

    private void LineRenderer()
    {
        _line.SetPosition(NUMBER_PLAYER_FOR_LINERENDER,transform.position);
        _line.SetPosition(NUMBER_NEXT_DOT_FOR_LINERENDER,pointers[point].transform.position);
    }

    private void DeletePoint()
    {
        if (Vector3.Distance(transform.position,pointers[point].transform.position) < 0.1f)
        {
            Destroy(pointers[point].gameObject);
            pointers.Remove(pointers[point]);
        }
    }

    private void ResetPoint()
    {
        if(pointers.Count <= 0)
        { 
            point = 0;
        }    
    } 

    internal void StopPlayer()
    {
        _isDead = true;
    }
}

