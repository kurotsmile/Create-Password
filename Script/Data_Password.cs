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

    private Carrot_Box box_list = null;

    public void Load_data()
    {
        string s_id_user_login = app.carrot.user.get_id_user_login();
        app.carrot.clear_contain(this.area_body_home);
        this.length = PlayerPrefs.GetInt("length_pass", 0);
        int count_item = 0;
        for (int i = this.length; i >= 0; i--)
        {
            string s_data = PlayerPrefs.GetString("data_p_" + i, "");
            if (s_data!= "")
            {
                var index = i;
                IDictionary data_pass = (IDictionary) Carrot.Json.Deserialize(s_data);
                GameObject item_password = Instantiate(this.prefab_password);
                item_password.transform.SetParent(this.area_body_home);
                item_password.transform.localPosition = new Vector3(item_password.transform.localPosition.x, item_password.transform.localPosition.y, 0f);
                item_password.transform.localScale = new Vector3(1f, 1f, 1f);
                Panel_item item_p=item_password.GetComponent<Panel_item>();
                item_p.txt_password.text = data_pass["pass_password"].ToString();
                item_p.txt_date.text = data_pass["pass_date"].ToString();
                item_p.txt_tag.text = data_pass["pass_tag"].ToString();
                item_p.s_type = data_pass["pass_type"].ToString();
                item_p.s_id = data_pass["pass_id"].ToString();

                item_p.index = i;
                item_p.button_download.SetActive(false);
 
                if (item_p.s_type=="1")
                    item_p.icon.sprite = icon_m5d;
                else
                    item_p.icon.sprite = icon_password;

                if (s_id_user_login == "")
                   item_p.button_upload.SetActive(false);
                else
                   item_p.button_upload.SetActive(true);

                item_p.Set_act_click(() => this.app.copy(data_pass["pass_password"].ToString()));
                item_p.Set_act_delete(() => Delete_pass(index));
                item_p.Set_act_upload(() => Upload_password(index));

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

    public void Add(IDictionary obj_json)
    {
        string s_pass = UnityEngine.Purchasing.MiniJSON.Json.Serialize(obj_json);
        Debug.Log(s_pass);
        PlayerPrefs.SetString("data_p_"+this.length,s_pass);
        this.length++;
        PlayerPrefs.SetInt("length_pass", this.length);
        this.Load_data();
    }

    public int Get_length()
    {
        return this.length;
    }

    public void Delete_pass(int index_delete)
    {
        PlayerPrefs.DeleteKey("data_p_" + index_delete);
        app.carrot.show_msg(PlayerPrefs.GetString("del_success", "Delete selected data successfully!"));
        this.Load_data();
    }

    public void Upload_password(int index)
    {
        string s_user_id_login = app.carrot.user.get_id_user_login();
        if (s_user_id_login != "")
        {
            this.app.carrot.show_loading();
            string s_data = PlayerPrefs.GetString("data_p_" + index, "");
            IDictionary data_pass = (IDictionary)Carrot.Json.Deserialize(s_data);

            data_pass["user_id"] = app.carrot.user.get_id_user_login();
            data_pass["user_lang"] = app.carrot.user.get_lang_user_login();

            string s_json = app.carrot.server.Convert_IDictionary_to_json(data_pass);
            app.carrot.server.Add_Document_To_Collection(this.app.carrot.Carrotstore_AppId, data_pass["pass_id"].ToString(), s_json, Act_done_upload_password_done, Act_done_upload_password_fail);
        }
    }

    private void Act_done_upload_password_done(string s_data)
    {
        app.carrot.play_vibrate();
        app.carrot.hide_loading();
        app.carrot.show_msg(PlayerPrefs.GetString("backup", "Backup by account"), PlayerPrefs.GetString("backup_success", "Backup this data item successfully!"));
        app.carrot.ads.show_ads_Interstitial();
    }

    private void Act_done_upload_password_fail(string s_data)
    {
        app.carrot.hide_loading();
        app.carrot.show_msg(PlayerPrefs.GetString("backup", "Backup by account"), PlayerPrefs.GetString("backup_fail", "Backup failed"));
    }

    public void Show_list_password_online()
    {
        app.carrot.ads.show_ads_Interstitial();
        app.carrot.play_sound_click();
        this.Get_list_pass_online();
    }

    private void Get_list_pass_online()
    {
        if (this.box_list != null) this.box_list.close();
        this.app.carrot.show_loading();
        StructuredQuery q = new(app.carrot.Carrotstore_AppId);
        q.Add_where("user_id",Query_OP.EQUAL, app.carrot.user.get_id_user_login());
        app.carrot.server.Get_doc(q.ToJson(), Act_show_list_password_online_done, Act_show_list_password_online_fail);
    }

    private void Act_show_list_password_online_done(string s_data)
    {
        this.app.carrot.hide_loading();
        Fire_Collection fc = new(s_data);
        if (!fc.is_null)
        {
            this.box_list = app.carrot.Create_Box(PlayerPrefs.GetString("list_pass_online", "Backup data"), this.icon_list_password_online);
            for(int i = 0; i < fc.fire_document.Length; i++)
            {
                var data_pass = fc.fire_document[i].Get_IDictionary();
                Carrot_Box_Item item_pass = box_list.create_item("item_" + i);
                item_pass.set_title(data_pass["pass_password"].ToString());
                item_pass.set_tip(data_pass["pass_tag"].ToString() + " - " + data_pass["pass_date"].ToString());

                if (data_pass["pass_type"].ToString() == "1")
                    item_pass.set_icon(icon_m5d);
                else
                    item_pass.set_icon(icon_password);

                item_pass.set_act(() => this.app.copy(data_pass["pass_password"].ToString()));

                Carrot_Box_Btn_Item btn_download = item_pass.create_item();
                btn_download.set_icon(app.carrot.icon_carrot_download);
                btn_download.set_act(() => Act_download_pass(data_pass));

                var pass_id = data_pass["pass_id"].ToString();
                Carrot_Box_Btn_Item btn_del = item_pass.create_item();
                btn_del.set_icon(app.carrot.sp_icon_del_data);
                btn_del.set_act(() => delete_pass_online(pass_id));

            }
        }
        else
        {
            app.carrot.show_msg(PlayerPrefs.GetString("list_pass_online", "Backup data"),PlayerPrefs.GetString("no_item", "No items have been archived yet"),Carrot.Msg_Icon.Alert);
        }
    }

    private void Act_show_list_password_online_fail(string s_error)
    {
        this.app.carrot.hide_loading();
        this.app.carrot.show_msg(PlayerPrefs.GetString("list_pass_online", "Backup data"), s_error, Msg_Icon.Error);
    }

    public void delete_pass_online(string s_id)
    {
        app.carrot.server.Delete_Doc(app.carrot.Carrotstore_AppId, s_id, Act_delete_pass);
    }

    private void Act_delete_pass(string s_data)
    {
        app.carrot.play_vibrate();
        app.carrot.show_msg(PlayerPrefs.GetString("list_pass_online", "Backup data"), PlayerPrefs.GetString("del_success", "Delete selected data successfully!"), Msg_Icon.Success);
        Get_list_pass_online();
    }

    private void Act_download_pass(IDictionary data_pass)
    {
        app.carrot.play_vibrate();
        Add(data_pass);
        app.carrot.show_msg(PlayerPrefs.GetString("list_pass_online", "Backup data"), PlayerPrefs.GetString("download_success", "Data download successful!"), Carrot.Msg_Icon.Success);
    }
}
