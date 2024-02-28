using Carrot;
using System.Collections;
using UnityEngine;

public class Data_Password : MonoBehaviour
{
    [Header("Main Obj")]
    public App app;

    [Header("Data Obj")]
    int length;
    public GameObject prefab_password;
    public Transform area_body_home;
    public Sprite icon_password;
    public Sprite icon_m5d;
    public Sprite icon_list_password_online;
    public GameObject panel_no_item;

    public void load_data()
    {
        string s_id_user_login = app.carrot.user.get_id_user_login();
        app.carrot.clear_contain(this.area_body_home);
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
        string s_user_id_login = app.carrot.user.get_id_user_login();
        if (s_user_id_login != "")
        {
            string s_id= "pass" + app.carrot.generateID();
            IDictionary data_pass = (IDictionary)Json.Deserialize("{}");
            data_pass["pass_id"] = s_id;
            data_pass["user_id"] = app.carrot.user.get_id_user_login();
            data_pass["user_lang"] = app.carrot.user.get_lang_user_login();
            data_pass["pass_tag"] = data_password_item.txt_tag.text;
            data_pass["pass_password"] = data_password_item.txt_password.text;
            data_pass["pass_date"] = data_password_item.txt_date.text;
            data_pass["pass_type"] = data_password_item.s_type;

            string s_json = app.carrot.server.Convert_IDictionary_to_json(data_pass);
            app.carrot.server.Add_Document_To_Collection(this.app.carrot.Carrotstore_AppId, s_id, s_json,Act_done_upload_password_done, Act_done_upload_password_fail);
        }
    }

    private void Act_done_upload_password_done(string s_data)
    {
        app.carrot.show_msg(PlayerPrefs.GetString("backup", "Backup by account"), PlayerPrefs.GetString("backup_success", "Backup this data item successfully!"));
        app.carrot.ads.show_ads_Interstitial();
    }

    private void Act_done_upload_password_fail(string s_data)
    {
        app.carrot.show_msg(PlayerPrefs.GetString("backup", "Backup by account"), PlayerPrefs.GetString("backup_fail", "Backup failed"));
    }

    public void show_list_password_online()
    {
        GameObject.Find("App").GetComponent<App>().carrot.play_sound_click();
        this.get_list_pass_online();
    }

    private void get_list_pass_online()
    {
        StructuredQuery q = new(app.carrot.Carrotstore_AppId);
        q.Add_where("user_id",Query_OP.EQUAL, app.carrot.user.get_id_user_login());
        q.Add_where("user_lang", Query_OP.EQUAL, app.carrot.user.get_lang_user_login());
        app.carrot.server.Get_doc(q.ToJson(), Act_show_list_password_online_done);
    }

    private void Act_show_list_password_online_done(string s_data)
    {
        Fire_Collection fc = new(s_data);
        if (!fc.is_null)
        {
            Carrot.Carrot_Box box_list_pass_online=app.carrot.Create_Box(PlayerPrefs.GetString("list_pass_online", "Backup data"), this.icon_list_password_online);
            for(int i = 0; i < fc.fire_document.Length; i++)
            {
                IDictionary data_pass = fc.fire_document[i].Get_IDictionary();
                GameObject item_password = Instantiate(this.prefab_password);
                item_password.transform.SetParent(box_list_pass_online.area_all_item);
                item_password.transform.localPosition = new Vector3(item_password.transform.localPosition.x, item_password.transform.localPosition.y, 0f);
                item_password.transform.localScale = new Vector3(1f, 1f, 1f);
                item_password.GetComponent<Panel_item>().txt_password.text = data_pass["password"].ToString();
                item_password.GetComponent<Panel_item>().txt_date.text = data_pass["date"].ToString();
                item_password.GetComponent<Panel_item>().txt_tag.text = data_pass["tag"].ToString();
                item_password.GetComponent<Panel_item>().s_id = data_pass["id"].ToString();
                item_password.GetComponent<Panel_item>().s_type= data_pass["type"].ToString();
                item_password.GetComponent<Panel_item>().button_download.SetActive(true);
                item_password.GetComponent<Panel_item>().button_upload.SetActive(false);
                item_password.GetComponent<Panel_item>().is_online = true;

                if (data_pass["type"].ToString()=="1")
                    item_password.GetComponent<Panel_item>().icon.sprite = icon_m5d;
                else
                    item_password.GetComponent<Panel_item>().icon.sprite = icon_password;
            }
        }
        else
        {
            app.carrot.show_msg(PlayerPrefs.GetString("list_pass_online", "Backup data"),PlayerPrefs.GetString("no_item", "No items have been archived yet"),Carrot.Msg_Icon.Alert);
        }
    }

    public void delete_pass_online(string s_id)
    {
        app.carrot.server.Delete_Doc(app.carrot.Carrotstore_AppId, s_id, act_delete_pass);
    }

    private void act_delete_pass(string s_data)
    {
        app.carrot.show_msg(PlayerPrefs.GetString("list_pass_online", "Backup data"), PlayerPrefs.GetString("del_success", "Delete selected data successfully!"), Carrot.Msg_Icon.Success);
        this.get_list_pass_online();
    }
}
