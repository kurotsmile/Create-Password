using UnityEngine;
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
    public bool is_online = false;

    public void click()
    {
        GameObject.Find("App").GetComponent<App>().SoundClick.Play();
        GameObject.Find("App").GetComponent<App>().carrot.show_input(PlayerPrefs.GetString("copy_success", "Has been copied!"), PlayerPrefs.GetString("copy_success", "Has been copied!"), txt_password.text, Carrot.Window_Input_value_Type.input_field);
    }

    public void delete()
    {
        if (this.is_online)
        {
            GameObject.Find("App").GetComponent<Data_Password>().delete_pass_online(this.s_id);
        }
        else
        {
            GameObject.Find("App").GetComponent<Data_Password>().delete_pass(this.index);
            GameObject.Find("App").GetComponent<App>().carrot.show_msg(PlayerPrefs.GetString("del_success", "Delete selected data successfully!"));
        }
        GameObject.Find("App").GetComponent<App>().SoundClick.Play();
    }

    public void copy()
    {
        GameObject.Find("App").GetComponent<App>().SoundClick.Play();
        GameObject.Find("App").GetComponent<App>().carrot.show_input(PlayerPrefs.GetString("copy_success", "Has been copied!"), PlayerPrefs.GetString("copy_success", "Has been copied!"), txt_password.text, Carrot.Window_Input_value_Type.input_field);
    }

    public void upload_password()
    {
        GameObject.Find("App").GetComponent<Data_Password>().upload_password(this);
    }

    public void download_password()
    {
        this.GetComponent<App>().carrot.show_msg(PlayerPrefs.GetString("list_pass_online", "Backup data"), PlayerPrefs.GetString("download_success", "Data download successful!"), Carrot.Msg_Icon.Success);
        GameObject.Find("App").GetComponent<Data_Password>().add(this.txt_password.text, this.txt_tag.text, System.DateTime.Today.ToString(), int.Parse(this.s_type));
    }
}
