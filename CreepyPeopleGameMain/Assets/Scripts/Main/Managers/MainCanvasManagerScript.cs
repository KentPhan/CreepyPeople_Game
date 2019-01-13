using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainCanvasManagerScript : MonoBehaviour
{
    public static MainCanvasManagerScript Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetConnectionStatusText()
    {

    }
}