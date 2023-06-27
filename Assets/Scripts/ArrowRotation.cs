using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowRotation : MonoBehaviour
{
    [SerializeField]
    GameObject[] point;
    int startNumber;
    [SerializeField]
    string tagPoint;
    int maxPoints;
    // Start is called before the first frame update
    void Start()
    {
        startNumber = 0;
        maxPoints = point.Length;
        print(maxPoints);
    }

    // Update is called once per frame
    void Update()
    {
        //float angle = Mathf.Atan2( gameObject.transform.position.x- point.position.x, 0)*Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(new Vector3(-90f,-20f , angle));
        transform.LookAt(point[startNumber].transform);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(tagPoint))
        {
            print("Next Point");
            print(startNumber);
            print(maxPoints);
            point[startNumber].SetActive(false);
            if (maxPoints-1 > startNumber)
            { 
                startNumber ++;
            }
        }
    }
}
