using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_encode : MonoBehaviour
{
    public int int_type;
    public Text txt_name;
    public void click()
    {
        GameObject.Find("App").GetComponent<App>().sel_md5_type(this);
    }
}
