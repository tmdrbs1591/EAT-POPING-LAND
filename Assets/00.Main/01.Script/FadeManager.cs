using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeManager : MonoBehaviour
{
    public static FadeManager instance;
    [SerializeField] private GameObject fadeIn;
    [SerializeField] private GameObject fadeOut;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        FadeOut();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void FadeIn()
    {
        fadeIn.SetActive(true);
    }
    public void FadeOut()
    {
        fadeOut.SetActive(true);
    }
}
