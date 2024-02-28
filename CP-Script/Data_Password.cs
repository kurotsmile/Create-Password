using Carrot;
using System.Collections;
using UnityEngine;

public class Data_Password : MonoBehaviour
{
    int length;
    public GameObject prefab_password;
    public Transform area_body_home;
    public Sprite icon_password;
    public Sprite icon_m5d;
    public Sprite icon_list_password_online;
    public GameObject panel_no_item;

    public void load_data()
    {
        string s_id_user_login = this.GetComponent<App>().carrot.user.get_id_user_login();
        this.GetComponent<App>().carrot.clear_contain(this.area_body_home);
        this.length = PlayerPrefs.GetInt("length", 0);
        int count_item = 0;
        for (int i = this.length; i >= 0; i--)
        {
            if (PlayerPrefs.GetString("data_pass_" + i, "") != "")
            {
                GameObject item_password = Instantiate(this.prefab_password);
                item_password.transform.SetParent(this.area_body_home);
                item_password.transform.localPosition = new Vector3(item_password.transform.localPosition.x, item_password.transform.localPosition.y, 0f);
                item_password.transform.localScale = new Vector3(1f, 1f, 1f);
                item_password.GetComponent<Panel_item>().txt_password.text = PlayerPrefs.GetString("data_pass_" + i);
                item_password.GetComponent<Panel_item>().txt_date.text = PlayerPrefs.GetString("data_date_" + i);
                item_password.GetComponent<Panel_item>().txt_tag.text = PlayerPrefs.GetString("data_tag_" + i);
                item_password.GetComponent<Panel_item>().index = i;
                item_password.GetComponent<Panel_item>().button_download.SetActive(false);
                item_password.GetComponent<Panel_item>().is_online = false;
                item_password.GetComponent<Panel_item>().s_type = PlayerPrefs.GetInt("data_type_" + i).ToString();
                if (PlayerPrefs.GetInt("data_type_" + i)==1)
                    item_password.GetComponent<Panel_item>().icon.sprite = icon_m5d;
                else
                    item_password.GetComponent<Panel_item>().icon.sprite = icon_password;

                if (s_id_user_login == "")
                   item_password.GetComponent<Panel_item>().button_upload.SetActive(false);
                else
                   item_password.GetComponent<Panel_item>().button_upload.SetActive(true);

                count_item++;
            }
        }

        if (count_item > 0)
        {
            this.panel_no_item.SetActive(false);
        }
        else
        {
            this.panel_no_item.SetActive(true);
        }
    }

    public void add(string s_pass,string s_tag,string s_date,int types)
    {
        PlayerPrefs.SetString("data_pass_"+this.length,s_pass);
        PlayerPrefs.SetString("data_tag_" + this.length,s_tag);
        PlayerPrefs.SetString("data_date_" + this.length, s_date);
        PlayerPrefs.SetInt("data_type_" + this.length, types);
        this.length++;
        PlayerPrefs.SetInt("length",this.length);
        this.load_data();
    }

    public int get_length()
    {
        return this.length;
    }

    public void delete_pass(int index_delete)
    {
        PlayerPrefs.DeleteKey("data_pass_" + index_delete);
        PlayerPrefs.DeleteKey("data_tag_" + index_delete);
        PlayerPrefs.DeleteKey("data_date_" + index_delete);
        PlayerPrefs.DeleteKey("data_type_" + index_delete);
        this.load_data();
    }

    public void upload_password(Panel_item data_password_item)
    {
        string s_user_id_login = this.GetComponent<App>().carrot.user.get_id_user_login();
        if (s_user_id_login != "")
        {
            IDictionary data_pass = (IDictionary)Json.Deserialize("{}");
            data_pass["user_id"] = s_user_id_login;
            data_pass["user_lang"] = this.GetComponent<App>().carrot.user.get_lang_user_login();
            data_pass["pass_tag"] = data_password_item.txt_tag.text;
            data_pass["pass_password"] = data_password_item.txt_password.text;
            data_pass["pass_date"] = data_password_item.txt_password.text;
            data_pass["pass_type"] = data_password_item.s_type;

            string s_json = this.GetComponent<App>().carrot.server.Convert_IDictionary_to_json(data_pass);
            this.GetComponent<App>().carrot.server.Add_Document_To_Collection("app",this.GetComponent<App>().carrot.Carrotstore_AppId,s_json, Act_done_upload_password_done); 
        }
    }

    private void Act_done_upload_password_done(string s_data)
    {
        IDictionary data_password = (IDictionary) Carrot.Json.Deserialize(s_data);
        if (data_password["error"].ToString() == "2")
            this.GetComponent<App>().carrot.show_msg(PlayerPrefs.GetString("backup", "Backup by account"), PlayerPrefs.GetString("backup_existed", "This data item already exists on the online archive!"));
        else if (data_password["error"].ToString() == "0")
            this.GetComponent<App>().carrot.show_msg(PlayerPrefs.GetString("backup", "Backup by account"), PlayerPrefs.GetString("backup_success", "Backup this data item successfully!"));
        else
            this.GetComponent<App>().carrot.show_msg(PlayerPrefs.GetString("backup", "Backup by account"), PlayerPrefs.GetString("backup_fail", "Backup failed"));
    }

    public void show_list_password_online()
    {
        this.GetComponent<App>().SoundClick.Play();
        this.get_list_pass_online();
    }

    private void get_list_pass_online()
    {
        /*
        WWWForm frm_list_password = this.GetComponent<App>().carrot.frm_act("list_password");
        frm_list_password.AddField("user_id", this.GetComponent<App>().carrot.get_id_user_login());
        frm_list_password.AddField("user_lang", this.GetComponent<App>().carrot.get_lang_user_login());
        this.GetComponent<App>().carrot.send(frm_list_password, act_show_list_password_online);
        */
    }

    private void act_show_list_password_online(string s_data)
    {
        IList list_pass = (IList)Carrot.Json.Deserialize(s_data);
        if (list_pass.Count > 0)
        {
            Carrot_Box box_list_pass=this.GetComponent<App>().carrot.Create_Box(PlayerPrefs.GetString("list_pass_online", "Backup data"), this.icon_list_password_online);
            for(int i = 0; i < list_pass.Count; i++)
            {
                IDictionary data_pass = (IDictionary)list_pass[i];
                GameObject item_password = Instantiate(this.prefab_password);
                item_password.GetComponent<Panel_item>().txt_password.text = data_pass["password"].ToString();
                item_password.GetComponent<Panel_item>().txt_date.text = data_pass["date"].ToString();
                item_password.GetComponent<Panel_item>().txt_tag.text = data_pass["tag"].ToString();
                item_password.GetComponent<Panel_item>().s_id = data_pass["id"].ToString();
                item_password.GetComponent<Panel_item>().s_type= data_pass["type"].ToString();
                item_password.GetComponent<Panel_item>().button_download.SetActive(true);
                item_password.GetComponent<Panel_item>().button_upload.SetActive(false);
                item_password.GetComponent<Panel_item>().is_online = true;
                box_list_pass.add_item(item_password);

                if (data_pass["type"].ToString()=="1")
                    item_password.GetComponent<Panel_item>().icon.sprite = icon_m5d;
                else
                    item_password.GetComponent<Panel_item>().icon.sprite = icon_password;
            }
        }
        else
        {
            this.GetComponent<App>().carrot.show_msg(PlayerPrefs.GetString("list_pass_online", "Backup data"),PlayerPrefs.GetString("no_item", "No items have been archived yet"),Carrot.Msg_Icon.Alert);
        }
    }

    public void delete_pass_online(string s_id)
    {
        /*
        WWWForm frm_del = this.GetComponent<App>().carrot.frm_act("del_password");
        frm_del.AddField("id_del", s_id);
        frm_del.AddField("lang_del", this.GetComponent<App>().carrot.get_lang_user_login());
        this.GetComponent<App>().carrot.send(frm_del, act_delete_pass);
        */
    }

    private void act_delete_pass(string s_data)
    {
        this.GetComponent<App>().carrot.show_msg(PlayerPrefs.GetString("list_pass_online", "Backup data"), PlayerPrefs.GetString("del_success", "Delete selected data successfully!"), Carrot.Msg_Icon.Success);
        this.get_list_pass_online();
    }
}
