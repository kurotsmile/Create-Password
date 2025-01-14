using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Panel_item : MonoBehaviour
{
    public string s_id;
    public string s_type;
    public Image icon;
    public Text txt_tag;
    public Text txt_date;
    public Text txt_password;
    public int index;
    public GameObject button_upload;
    public GameObject button_download;

    private UnityAction act_click = null;
    private UnityAction act_delete=null;
    private UnityAction act_download=null;
    private UnityAction act_upload=null;

    public void click()
    {
        act_click?.Invoke();
    }

    public void Set_act_click(UnityAction act_click)
    {
        this.act_click = act_click;
    }

    public void Set_act_delete(UnityAction act_delete)
    {
        this.act_delete = act_delete;
    }

    public void Set_act_upload(UnityAction act_upload)
    {
        this.act_upload= act_upload;
    }

    public void Set_act_download(UnityAction act_download)
    {
        this.act_download=act_download;
    }

    public void delete()
    {
        act_delete?.Invoke();
    }

    public void upload_password()
    {
        act_upload?.Invoke();
    }

    public void download_password()
    {
        act_download?.Invoke();
    }
}
