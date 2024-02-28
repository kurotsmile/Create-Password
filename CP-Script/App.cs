
using System;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class App : MonoBehaviour
{
    public Carrot.Carrot carrot;
    public GameObject panel_add;
    public GameObject button_RemoveAds;
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
    public AudioSource SoundClick;
    public AudioSource SoundSuccess;
    private Item_encode item_encode_temp;

    [Header("Ads")]
    public string id_ads_app_vungle;
    public string id_ads_trunggiang_vungle;

#if UNITY_IOS
	private string gameId = "3398022";
#elif UNITY_ANDROID
    private string gameId = "3398023";
#endif
    private int count_click_ads = 0;

    void Start()
    {
        this.carrot.Load_Carrot(check_exit_app);
        this.carrot.shop.onCarrotPaySuccess = this.On_Buy_Success_Carrot_Pay;
        this.carrot.shop.onCarrotRestoreSuccess = this.On_Restore_Carrot_Pay;
        this.carrot.act_after_close_all_box = this.check_list_online_password;
        this.panel_add.SetActive(false);
        this.panel_m5d.SetActive(false);
        this.GetComponent<Data_Password>().load_data();
        this.check_list_online_password();
#if UNITY_WSA
        Vungle.init(this.id_ads_app_vungle);
        Vungle.loadAd(this.id_ads_trunggiang_vungle);
#elif UNITY_ANDROID
        if (Advertisement.isSupported) Advertisement.Initialize(gameId);
#endif

        this.check_inapp_remove_ads();
    }



    private void check_inapp_remove_ads()
    {
        if (PlayerPrefs.GetInt("is_buy_ads", 0) == 0)
            this.button_RemoveAds.SetActive(true);
        else
            this.button_RemoveAds.SetActive(false);
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
        this.create_pass();
        this.inp_password_tag.text = "Password " + this.GetComponent<Data_Password>().get_length();
        this.panel_add.SetActive(true);
    }

    public void create_pass()
    {
        this.SoundClick.Play();
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
        this.SoundSuccess.Play();
        this.check_show_ads();
        this.GetComponent<Data_Password>().add(this.txt_password.text, this.inp_password_tag.text, DateTime.Today.ToString(),0);
        this.panel_add.SetActive(false);
    }

    public void add_md5()
    {
        this.SoundSuccess.Play();
        this.check_show_ads();
        this.GetComponent<Data_Password>().add(this.txt_m5d_show.text, this.inp_string_m5d.text, DateTime.Today.ToString(), 1);
        this.panel_m5d.SetActive(false);
    }

    public void show_app_more()
    {
        this.SoundClick.Play();
        this.carrot.show_list_carrot_app();
    }

    public void show_m5d()
    {
        this.SoundClick.Play();
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
        this.SoundClick.Play();
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

    public void rate_app()
    {
        this.SoundClick.Play();
        this.carrot.show_rate();
    }

    public void app_share()
    {
        this.SoundClick.Play();
        this.carrot.show_share();
    }

    public void check_show_ads()
    {
        this.carrot.ads.show_ads_Interstitial();
    }

    public void reset_app()
    {
        this.SoundClick.Play();
        this.carrot.delete_all_data();
        this.check_inapp_remove_ads();
    }

    public void copy_text_password()
    {
        carrot.play_sound_click();
        carrot.show_input(PlayerPrefs.GetString("copy_success", "Has been copied!"), PlayerPrefs.GetString("copy_success", "Has been copied!"), txt_password.text, Carrot.Window_Input_value_Type.input_field);
    }

    public void copy_text_m5d()
    {
        carrot.play_sound_click();
        carrot.show_input(PlayerPrefs.GetString("copy_success", "Has been copied!"), PlayerPrefs.GetString("copy_success", "Has been copied!"), txt_m5d_show.text, Carrot.Window_Input_value_Type.input_field);
    }

    public void set_length_by_inp()
    {
        this.slider_password.value =int.Parse(this.inp_password_length.text);
    }

    public void buy_success(Product product)
    {
        this.On_Buy_Success_Carrot_Pay(product.definition.id);
    }

    public void buy_product(int index_product)
    {
        this.carrot.buy_product(index_product);
    }

    private void act_inapp_removeAds()
    {
        PlayerPrefs.SetInt("is_buy_ads", 1);
        this.button_RemoveAds.SetActive(false);
    }

    private void On_Buy_Success_Carrot_Pay(string id_product)
    {
        if (id_product == this.carrot.shop.get_id_by_index(0))
        {
            this.carrot.show_msg(PlayerPrefs.GetString("Shop", "shop"), "Successful removal of advertising! Thank you for your purchase of the app's items", Carrot.Msg_Icon.Success);
            this.act_inapp_removeAds();
        }
    }

    private void On_Restore_Carrot_Pay(string[] arr_id)
    {
        for(int i = 0; i < arr_id.Length; i++)
        {
            string id_p = arr_id[i];
            if (id_p == this.carrot.shop.get_id_by_index(0)) this.act_inapp_removeAds();
        }
    }

    public void Restore_Product()
    {
        this.SoundClick.Play();
        this.carrot.restore_product();
    }

    public void btn_user()
    {
        this.SoundClick.Play();
        this.carrot.show_login();
    }

    public void btn_sel_lang()
    {
        this.SoundClick.Play();
        this.carrot.show_list_lang();
    }

    public void On_After_Login()
    {
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

    }
}
