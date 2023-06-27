using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowRotation : MonoBehaviour
{
    [SerializeField]
    List<GameObject> point;
    int startNumber;
    [SerializeField]
    string tagPoint;
    // Start is called before the first frame update
    void Start()
    {
        startNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //float angle = Mathf.Atan2( gameObject.transform.position.x- point.position.x, 0)*Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(new Vector3(-90f,-20f , angle));

        if(point.Count != 0)
        {
        transform.LookAt(point[startNumber].transform);
        }
    }
    public void NextPoint()
    {
        startNumber++;
        if(startNumber < point.Count)
        {
            startNumber++;
        }
    }
    public void SetPoint(GameObject obj)
    {
        point.Add(obj);
    }
}
