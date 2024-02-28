using System;
using UnityEngine;
using UnityEngine.UI;


public class App : MonoBehaviour
{
    public Carrot.Carrot carrot;
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
    public AudioSource[] sounds;
    public AudioClip SoundClicl_clip;

    private Item_encode item_encode_temp;

    void Start()
    {
        this.carrot.Load_Carrot(check_exit_app);
        this.carrot.act_after_close_all_box = this.check_list_online_password;
        this.carrot.change_sound_click(this.SoundClicl_clip);

        this.panel_add.SetActive(false);
        this.panel_m5d.SetActive(false);
        this.GetComponent<Data_Password>().load_data();
        this.check_list_online_password();
    }

    private void check_exit_app()
    {
        if (this.panel_add.activeInHierarchy)
        {
            this.panel_add.SetActive(false);
            this.carrot.set_no_check_exit_app();
        }else if (this.panel_m5d.activeInHierarchy)
        {
            this.panel_m5d.SetActive(false);
            this.carrot.set_no_check_exit_app();
        }
    }

    public void show_add()
    {
        this.carrot.ads.show_ads_Interstitial();
        this.create_pass();
        this.inp_password_tag.text = "Password " + this.GetComponent<Data_Password>().get_length();
        this.panel_add.SetActive(true);
    }

    public void create_pass()
    {
        this.carrot.play_sound_click();
        const int MAXIMUM_PASSWORD_ATTEMPTS = 10000;
        bool includeLowercase = this.Toggle_includeLowercase.isOn;
        bool includeUppercase = this.Toggle_includeUppercase.isOn;
        bool includeNumeric = this.Toggle_includeNumeric.isOn;
        bool includeSpecial = this.Toggle_includeSpecial.isOn;
        int lengthOfPassword =int.Parse(this.slider_password.value.ToString());

        PasswordGeneratorSettings settings = new PasswordGeneratorSettings(includeLowercase, includeUppercase, includeNumeric, includeSpecial, lengthOfPassword);
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
        this.txt_password.text = password;
        this.inp_password_length.text = this.slider_password.value.ToString();
    }

    public void add_password()
    {
        this.carrot.ads.show_ads_Interstitial();
        this.play_sound(0);
        this.carrot.ads.show_ads_Interstitial();
        this.GetComponent<Data_Password>().add(this.txt_password.text, this.inp_password_tag.text, DateTime.Today.ToString(),0);
        this.panel_add.SetActive(false);
    }

    public void add_md5()
    {
        this.play_sound(0);
        this.carrot.ads.show_ads_Interstitial();
        this.GetComponent<Data_Password>().add(this.txt_m5d_show.text, this.inp_string_m5d.text, DateTime.Today.ToString(), 1);
        this.panel_m5d.SetActive(false);
    }

    public void show_m5d()
    {
        this.carrot.ads.show_ads_Interstitial();
        this.carrot.play_sound_click();
        this.panel_m5d.SetActive(true);
        this.panel_m5d_menu.SetActive(true);
    }

    public void sel_md5_type(Item_encode encode_temp)
    {
        this.item_encode_temp = encode_temp;
        this.txt_m5d_type.text = encode_temp.txt_name.text;
        this.panel_m5d_menu.SetActive(false);
        this.create_m5d();
    }

    public void create_m5d()
    {
        this.carrot.play_sound_click();
        if(this.item_encode_temp.int_type==0)
            this.txt_m5d_show.text = PasswordGenerator.MD5Hash(this.inp_string_m5d.text);
        else if (this.item_encode_temp.int_type == 1)
            this.txt_m5d_show.text = PasswordGenerator.SHA512(this.inp_string_m5d.text);
        else if (this.item_encode_temp.int_type == 2)
            this.txt_m5d_show.text = PasswordGenerator.url_encode(this.inp_string_m5d.text);
        else if (this.item_encode_temp.int_type == 3)
            this.txt_m5d_show.text = PasswordGenerator.Base64(this.inp_string_m5d.text);
        else if (this.item_encode_temp.int_type == 4)
            this.txt_m5d_show.text = PasswordGenerator.SHA1(this.inp_string_m5d.text);
        else if (this.item_encode_temp.int_type == 5)
            this.txt_m5d_show.text = PasswordGenerator.SHA256(this.inp_string_m5d.text);
    }

    public void copy_text_password()
    {
        this.copy(this.txt_password.text);
    }

    public void copy_text_m5d()
    {
        this.copy(this.txt_m5d_show.text);
    }

    public void copy(string s_copy)
    {
        TextEditor txt_copy = new TextEditor();
        txt_copy.text = s_copy;
        txt_copy.SelectAll();
        txt_copy.Copy();
        this.carrot.show_input(PlayerPrefs.GetString("copy", "Copy"), PlayerPrefs.GetString("copy_success", "Copy successful !!!"), s_copy);
    }

    public void set_length_by_inp()
    {
        this.slider_password.value =int.Parse(this.inp_password_length.text);
    }

    public void btn_user()
    {
        this.carrot.play_sound_click();
        this.carrot.user.show_login(this.On_After_Login);
    }

    public void On_After_Login()
    {
        this.carrot.ads.show_ads_Interstitial();
        this.GetComponent<Data_Password>().load_data();
        this.check_list_online_password();
    }

    private void check_list_online_password()
    {
        if (this.carrot.user.get_id_user_login()== "")
            this.button_List_Online_Password.SetActive(false);
        else
            this.button_List_Online_Password.SetActive(true);
    }

    public void btn_encode_paste()
    {
        TextEditor txt_pase = new TextEditor();
        txt_pase.multiline = true;
        txt_pase.Paste();
        this.inp_string_m5d.text = txt_pase.text;

    }

    public void play_sound(int index)
    {
        if (this.carrot.get_status_sound()) this.sounds[index].Play();
    }

    public void btn_setting()
    {
        this.carrot.ads.show_ads_Interstitial();
        Carrot.Carrot_Box box_setting=this.carrot.Create_Setting();
        box_setting.set_act_before_closing(act_close_setting);
    }

    private void act_close_setting()
    {
        this.carrot.ads.show_ads_Interstitial();
        this.On_After_Login();
    }
}
