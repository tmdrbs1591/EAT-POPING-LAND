using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongStart : MonoBehaviour
{



    private void Start()
    {
    }
    public void SongStarts()
    {
        SongManager.instance.SongChange(2);
    }
    public void LodingSongStarts()
    {
        SongManager.instance.SongChange(4);
    }
    public void SongChanges(int index)
    {
        SongManager.instance.SongChange(index);
    }
}
