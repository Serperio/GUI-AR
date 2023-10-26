using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Record : MonoBehaviour
{

    /*private List<string> searchHistory = new List<string>();
    int n;

    void Start()
    {
        GameObject buttonTemplate = transform.GetChild(0).gameObject;
        GameObject g;

        n = searchHistory.Count; //3?

        for (int i = 0; i < n; i++)
        {
            g = Instantiate(buttonTemplate, transform);

            g.transform.GetChild(1).GetComponent<Text>().text = searchHistory[i];

            g.GetComponent<Button>().AddListener(i, Buscador.FindPointData(name));

            g.GetComponent<Button>().AddEventListener(i, DeleteRecord(i));
        }

        Destroy(buttonTemplate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*private void UpdateSearchHistoryUI(string text)
    {

        if (string.NotNullOrEmpty(inputText))
        {
            if (searchHistory.Contains(inputText))
            {
                searchHistory.Remove(inputText);
            }
            searchHistory.Insert(0, inputText);
            inputText = string.Empty;
        }
    }

    public void DeleteRecord(int N)
    {   
        searchHistory.RemoveAt(N);
    }

    public void ClearSearchHistory()
    {
        searchHistory.Clear();
        //UpdateSearchHistoryUI();
    }
    */
}
