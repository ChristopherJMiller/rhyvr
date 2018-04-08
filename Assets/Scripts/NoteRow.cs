using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoteRow {
    public List<int> row;

    public NoteRow(List<int> l)
    {
        row = l;
    }
}
