using Carrot;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class App : MonoBehaviour
{
    [Header("Main Obj")]
    public Carrot.Carrot carrot;
    public Data_Password pass;

    [Header("Add Obj")]
    public GameObject panel_add;
    public GameObject button_List_Online_Password;

    [Header("Add Password")]
    public Text txt_password;
    public Slider slider_password;
    public InputField inp_password_tag;
    public InputField inp_password_length;
    public Toggle Toggle_includeLowercase;
    public Toggle Toggle_includeUppercase;
    public Toggle Toggle_includeNumeric;
    public Toggle Toggle_includeSpecial;

    [Header("MD5")]
    public GameObject panel_m5d;
    public GameObject panel_m5d_menu;
    public InputField inp_string_m5d;
    public Text txt_m5d_show;
    public Text txt_m5d_type;

    [Header("Other")]
    public AudioSource Sound_success;
    public AudioClip SoundClicl_clip;
    public AudioSource bk_music;

    private Item_encode item_encode_temp;

    void Start()
    {
        carrot.Load_Carrot(Check_exit_app);
        carrot.act_after_close_all_box = check_list_online_password;
        carrot.game.load_bk_music(this.bk_music);
        carrot.change_sound_click(SoundClicl_clip);

        panel_add.SetActive(false);
        panel_m5d.SetActive(false);
        pass.Load_data();
        check_list_online_password();
    }

    private void Check_exit_app()
    {
        if (panel_add.activeInHierarchy)
        {
            panel_add.SetActive(false);
            carrot.set_no_check_exit_app();
        }else if (panel_m5d.activeInHierarchy)
        {
            panel_m5d.SetActive(false);
            carrot.set_no_check_exit_app();
        }
    }

    public void show_add()
    {
        carrot.ads.show_ads_Interstitial();
        create_pass();
        inp_password_tag.text = "Password " + pass.Get_length();
        panel_add.SetActive(true);
    }

    public void create_pass()
    {
        carrot.play_sound_click();
        const int MAXIMUM_PASSWORD_ATTEMPTS = 10000;
        bool includeLowercase = Toggle_includeLowercase.isOn;
        bool includeUppercase = Toggle_includeUppercase.isOn;
        bool includeNumeric = Toggle_includeNumeric.isOn;
        bool includeSpecial = Toggle_includeSpecial.isOn;
        int lengthOfPassword =int.Parse(slider_password.value.ToString());

        PasswordGeneratorSettings settings = new(includeLowercase, includeUppercase, includeNumeric, includeSpecial, lengthOfPassword);
        string password;
        if (!settings.IsValidLength())
        {
            password = settings.LengthErrorMessage();
        }
        else
        {
            int passwordAttempts = 0;
            do
            {
                password = PasswordGenerator.GeneratePassword(settings);
                passwordAttempts++;
            }
            while (passwordAttempts < MAXIMUM_PASSWORD_ATTEMPTS && !PasswordGenerator.PasswordIsValid(settings, password));

            password = PasswordGenerator.PasswordIsValid(settings, password) ? password : "Try again";
        }
        txt_password.text = password;
        inp_password_length.text = slider_password.value.ToString();
    }

    public void add_password()
    {
        this.play_sound_success();
        IDictionary data_pass = new Dictionary<string, string>();
        data_pass["pass_id"] = "pass" + carrot.generateID() + UnityEngine.Random.Range(0, 20);
        if (carrot.user.get_id_user_login() != "")
        {
            data_pass["user_id"] = carrot.user.get_id_user_login();
            data_pass["user_lang"] = carrot.user.get_lang_user_login();
        }
        data_pass["pass_tag"] = inp_password_tag.text;
        data_pass["pass_password"] = txt_password.text;
        data_pass["pass_date"] = DateTime.Today.ToString();
        data_pass["pass_type"] = "0";

        pass.Add(data_pass);
        panel_add.SetActive(false);
        carrot.ads.show_ads_Interstitial();
    }

    public void add_md5()
    {
        this.play_sound_success();

        IDictionary data_pass = new Dictionary<string, string>();
        data_pass["pass_id"] = "pass" + carrot.generateID() + UnityEngine.Random.Range(0, 20);
        if (carrot.user.get_id_user_login() != "")
        {
            data_pass["user_id"] = carrot.user.get_id_user_login();
            data_pass["user_lang"] = carrot.user.get_lang_user_login();
        }
        data_pass["pass_tag"] = inp_string_m5d.text;
        data_pass["pass_password"] = txt_m5d_show.text;
        data_pass["pass_date"] = DateTime.Today.ToString();
        data_pass["pass_type"] = "1";

        pass.Add(data_pass);
        panel_m5d.SetActive(false);
        carrot.ads.show_ads_Interstitial();
    }

    public void show_m5d()
    {
        carrot.ads.show_ads_Interstitial();
        carrot.play_sound_click();
        panel_m5d.SetActive(true);
        panel_m5d_menu.SetActive(true);
    }

    public void sel_md5_type(Item_encode encode_temp)
    {
        item_encode_temp = encode_temp;
        txt_m5d_type.text = encode_temp.txt_name.text;
        panel_m5d_menu.SetActive(false);
        create_m5d();
    }

    public void create_m5d()
    {
        carrot.play_sound_click();
        if(item_encode_temp.int_type==0)
            txt_m5d_show.text = PasswordGenerator.MD5Hash(inp_string_m5d.text);
        else if (item_encode_temp.int_type == 1)
            txt_m5d_show.text = PasswordGenerator.SHA512(inp_string_m5d.text);
        else if (item_encode_temp.int_type == 2)
            txt_m5d_show.text = PasswordGenerator.url_encode(inp_string_m5d.text);
        else if (item_encode_temp.int_type == 3)
            txt_m5d_show.text = PasswordGenerator.Base64(inp_string_m5d.text);
        else if (item_encode_temp.int_type == 4)
            txt_m5d_show.text = PasswordGenerator.SHA1(inp_string_m5d.text);
        else if (item_encode_temp.int_type == 5)
            txt_m5d_show.text = PasswordGenerator.SHA256(inp_string_m5d.text);
    }

    public void copy_text_password()
    {
        copy(txt_password.text);
    }

    public void copy_text_m5d()
    {
        copy(txt_m5d_show.text);
    }

    public void copy(string s_copy)
    {
        carrot.play_sound_click();
        TextEditor txt_copy = new TextEditor();
        txt_copy.text = s_copy;
        txt_copy.SelectAll();
        txt_copy.Copy();
        carrot.show_input(PlayerPrefs.GetString("copy", "Copy"), PlayerPrefs.GetString("copy_tip", "You can copy the data created in the text input box below"), s_copy);
    }

    public void set_length_by_inp()
    {
        slider_password.value =int.Parse(inp_password_length.text);
    }

    public void btn_user()
    {
        carrot.play_sound_click();
        carrot.user.show_login(On_After_Login);
    }

    public void On_After_Login()
    {
        carrot.ads.show_ads_Interstitial();
        pass.Load_data();
        check_list_online_password();
    }

    private void check_list_online_password()
    {
        if (carrot.user.get_id_user_login()== "")
            button_List_Online_Password.SetActive(false);
        else
            button_List_Online_Password.SetActive(true);
    }

    public void btn_encode_paste()
    {
        TextEditor txt_pase = new()
        {
            multiline = true
        };
        txt_pase.Paste();
        inp_string_m5d.text = txt_pase.text;
    }

    public void play_sound_success()
    {
        if (carrot.get_status_sound()) this.Sound_success.Play();
    }

    public void btn_setting()
    {
        carrot.ads.show_ads_Interstitial();
        Carrot_Box box_setting =carrot.Create_Setting();
        box_setting.set_act_before_closing(Act_close_setting);
    }

    private void Act_close_setting()
    {
        carrot.ads.show_ads_Interstitial();
        On_After_Login();
    }
}
