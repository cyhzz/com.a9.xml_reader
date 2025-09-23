using System.Collections;
using System.Collections.Generic;
using System.Data;
using Com.A9.FileReader;
using UnityEngine;

[System.Serializable]
public class RwoCol
{
    public List<List<string>> a;
}
public class ExcelTest : MonoBehaviour
{
#if UNITY_EDITOR_OSX
    [ContextMenu("Test")]
    public void Test()
    {
        DataTable dt = new DataTable();
        dt.Clear();
        dt.Columns.Add("Name");
        dt.Columns.Add("Marks");
        DataRow _ravi = dt.NewRow();
        _ravi["Name"] = "ravi";
        _ravi["Marks"] = "500";
        dt.Rows.Add(_ravi);
        xmlReader.SaveAsExcel<List<List<string>>>("test.xlsx", dt);
    }
#endif
}
