using macro.Properties;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Tesseract;

namespace macro
{
    public class Market : Form
    {
        protected ChromeDriverService driverService;
        protected ChromeOptions options;
        protected ChromeDriver driver;
        private FileService fileService = new FileService();
        private ParsingService parsingService = new ParsingService();
        private ProductInfoService productInfoService = new ProductInfoService();
        private Stopwatch stopwatch = new Stopwatch();
        protected static string API = "";
        protected static string URL = "https://login.11st.co.kr/auth/login.tmall?returnURL=https%253A%252F%252Fm.11st.co.kr%252FMW%252FMyPage%252FmypageMain.tmall";
        protected bool radioCheckedChanged;
        private bool logCheck;
        private string log;
        private string uslog;
        private string productBuyUrl;
        protected int sleepTime;
        protected int optionCoun;
        protected List<string> keyPad = new List<string>();
        protected List<string> keyPadValue = new List<string>();
        protected List<string> optNo;
        protected List<string> files = new List<string>();
        protected List<string> imageBase64Data = new List<string>();
        protected List<byte[]> imagesByteArray = new List<byte[]>();
        private List<int> pix = new List<int>()
    {
      40,
      65,
      95,
      118,
      143,
      169,
      0
    };
        private HtmlAgilityPack.HtmlDocument doc;
        private Thread buyThread;
        private System.Threading.Timer timeoutTimer;
        private string caption = string.Empty;
        private const int WMCLOSE = 16;
        private IContainer components;
        private CheckBox auto_singin_save_check_box;
        private Button singin_btn;
        private GroupBox easy_singin_group_box;
        private RadioButton pay_radio_btn;
        private RadioButton st11_radio_btn;
        private RadioButton facebook_radio_btn;
        private RadioButton t_radio_btn;
        private RadioButton naver_radio_btn;
        private GroupBox signin_group_box;
        private Label pw_label;
        private Label id_label;
        private TextBox pw_text_box;
        private TextBox id_text_box;
        private CheckBox debug_check_box;
        private TextBox product_no_text_box;
        private Label label1;
        private ComboBox item_select_1_combo_box;
        private Label label2;
        private TextBox sk_pay_text_box;
        private Label label4;
        private GroupBox groupBox1;
        private Label label5;
        private ComboBox ars_combo_box;
        private Label label3;
        private Label State;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(
          IntPtr hWnd,
          uint Msg,
          IntPtr wParam,
          IntPtr lParam);
        public Market()
        {
            this.InitializeComponent();
            this.radioSetting();
        }
        protected override bool ProcessCmdKey(ref Message msg, System.Windows.Forms.Keys keyData)
        {
            switch (keyData & ~(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt))
            {
                case System.Windows.Forms.Keys.Return:
                    this.signInBtn();
                    break;
                case System.Windows.Forms.Keys.S:
                    if ((keyData & System.Windows.Forms.Keys.Control) != System.Windows.Forms.Keys.None)
                    {
                        this.auto_singin_save_check_box.Checked = !this.auto_singin_save_check_box.Checked;
                        break;
                    }
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SignIn_Load(object sender, EventArgs e)
        {
            this.auto_singin_save_check_box.Checked = Settings.Default.InfoSaveCheck;
            this.id_text_box.Text = Settings.Default.idBox;
            this.pw_text_box.Text = Settings.Default.pwBox;
            this.product_no_text_box.Text = Settings.Default.productLinkBox;
            this.sk_pay_text_box.Text = Settings.Default.skPayBox;
            try
            {
                this.driverService = ChromeDriverService.CreateDefaultService();
                this.driverService.HideCommandPromptWindow = true;
                this.options = new ChromeOptions();
                this.options.AddArgument("disable-gpu");
                this.options.AddUserProfilePreference("profile.default_content_setting_values.images", (object)2);
                this.options.AddArgument("user-agent=Mozilla/5.0 (Linux; Android 12; SM-A205U) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.51 Mobile Safari/537.36");
                ChromeDriverService.CreateDefaultService().HideCommandPromptWindow = true;
            }
            catch (Exception ex)
            {
                if (this.logCheck)
                {
                    this.log += this.fileService.logMsg("SignIn_Load err : " + ex?.ToString());
                    this.fileService.logSave(this.log);
                }
            }
            this.imageBase64Data.Add("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");
            this.imageBase64Data.Add("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");
            this.imageBase64Data.Add("iVBORw0KGgoAAAANSUhEUgAAAR4AAABACAYAAADMOhjlAAALlElEQVR42u1df0SfaxRP8pVJzCQzGcmVyYxkMjMjSZJEZiZJzDWZTMxMZq6RmfmaGZlMZiJXrsmMmeuazMiVmVyRXJNk5MpMEs99Hzvdfe937znPeX583963nQ/nn3qf5zw/P+95zjnP+y0rEwgEAoFAIBAIBAKBQCAQCAQCgUAgEAgEAoFAIBAIBAKBQCAQCAQCgUAgEAgEAsGBg1KqLpLOSHpAOiKpkpERZHxdV0bSFkkXrO9zkeRKoKcW9syentZIKkqgJ1fUH62nPLSSE5G8iORVkUwHqr8xkgeRrCscHyIZi6TGsu6qSJ7GtN1WrmZokR+OZBLpxwSjfEuA8bofoB/NMfVWONRzKNAa2JNBpt76SMYjWUbW9G4kf7qs6yI956B/nwg9C6Cn1pNshiOZhzqLsRPJy0j6Qizis5FsYmzgWbcmhcfKDp8jGbFYcH+oMJjNEPE8IvqxxSjfH2jMpjz7MR9TZ85hjYVaA6x+gXVzD9mc1Lq+btm3pkjeWbZ9O5LbttYJWDSrFnr03B11nfhuaKgKTTy6UZEseUz+0wRJJzPEAwtEpYR4NCY9XnjKh3hgDcyr8JgyWJvvfNYZhxQ4e9OAN7qtzHFsd9S1Fslx24m/zKnZcVEdgqOTL+4a9Mz8SMSjFyxjXJMmnjHHvvzuSzxQz+OEied1gPonGEernQB63jDGr0GvGQ8dy2wfLZhiqoTEczfgImhOiHyyQDzXGP1IknhcSaeFqDPnUN9EEsQT/f3ngDpaiWPcWkA91w1j96bUBsKeokmbGh2jVtTZVzuneiM5D975m4TTTOMVQ6c+b08bZM7Q1XzKSecY+AlKSTzrjHHck6sefcHm4qNr5CQqd0sThqVg6/QRomOFGPfFSAYgEtQOTlrKZ/Ic0TFKlNkFku0BPR3w/N/UesDIHCJWGFbgVLTXn+vEPtXWWR3lEHtuS2UOC4AauCtImRrCH6QHuzrApl00nIdrUk48vzGnzId4XiXQjyai7cMJjmcb0Y6TyJEEw1wcYYI/6APhBI4rs0g4p1sJBzt1BOyzPDYuxfmHtD+HCETdwRxiTk44hwmdxSwdD6dpp8cC02b9BlH341LkQQTeJF0WU5Z24pkmrK1cgmOK7Yc5wtmLoY7Q00mUq4oJZ2MYZQRzdrgWHDyP4TyhZwQp87ctuysq+uQwodj59BKj7GfXskh9fZF8ISypKxnw61RahjlTSzxR/T8R7b6W4JieI9rRgpTpwSwRxvxhqC969gzx7DFGv7ATzUzMs4OYs5gROMKOqE02xDNBhDZDEs9Zj0hHv0M7xog+f+K0JyXEM070IWvEM0n0pTLBMcUcqr87WC7bBl21XEc6+Gyc9yH4rViBk+hvz7ATgMc+vcwlnjFDToUL8dRDfYXSwiyLnYcvWFoI0wYnYF1GSOcE8nbZJSIsqSQe8A/sukReAreDOtKfIcpVu0ReCcti3TLaV8Po2yw3Shf97T3y7EWGnocsPRBB2i6SwYL/ByMejwVRTiQxdTLrqDUkd/2qTcWyjIDwQ/xCzJkP8Syqb3foCkX7mBo8+4It1n9Ugnf1iLf1vEfZt3EWGwRNsBPAg5jncwQ5XzW07Qjh47kW8zz2bIfH+pmznYw0EM95wheTY5SvYOQ//AHW0AjnzLzPpDOE9GEVrLpSEI8Jn4BAjlv2hXJ8jiU4pqeJvrX57BMg7YuQTtIIoeiPRETrOKJjASmjo0mnCCv/JdciM/idOL4kzN/1KYvEg73dXzPL5yw30S5souoUks4RwofTZZizJBIIt23yeAwJpXPwQtgE0SHevClx1HFcsc25YFHH/QAJd4NE/aOGcb8H/qYaIFJ95P6LChwhR3jnBE7CF/UlU8QTqblBDERXiYjnPy9+2nw+Cr8GMMOYs1RlLsMGcb1zpHOXjgYa02ZCT49lXQ88xuyKoe4qQ0Kttz4q2MTsP1Z+NzPEox3H1NHIop6cx+TozM8jKSGdViKB7FjKiMfofwN/lA82Qlg/RLb0ewcnue8l0WqDjr5Ac/MRSVLs9CSe80H4Yr+Ix0A6mza+BPDx+Fx2e5YC0qEugY4w54xDPL0BiWdFIdcc4O29FUDHho9VqjORibovWdTTYEhG5WLB5FBXX68S+WIEqfvHJR5FX3jU/pd2hzorYbEXyhEY6FFl/kzHqX0mHuzIuWgxZ1tMXVVM0RbYLUXfE+su4eZxi5jwwszLli+F9wy/4SaTbJ8wXxCux651LDfqhzxqgWVC3SbesT1zB/QnPd5H0tEm/BcuIfoSj0P7zhCh3ilko24YrJhxsHp74cWwaNhMzQ7tphypAxb1XDZY58OFVgykdtxS9IXpJuYLYhSSHneKHM3U5dDrjhbgwXMuw2RQH+/acrF0HNrxKzcCkCDxYJdA85ZztlXCNk5xLQdF3y97gR01FJ1xnndo8zTh1yu3qOctQTqNRLn2wP3RvswKQ4SNzI1SX689+CQqYvfWNlNHPBD6WzM4wk4mtMmxgdvZJ9LpJszl6hQRTy/XxCas2nWGfwNzBH+wbG8jsd6GLTe7cz1E8uSSx1xQuVG3GeV3PfKZsODEy1QRj/qaNr5tSOyrSXCjtyVh4QV4K++q7zPO92THkO+xDY7fo0mMW7H1QOTMTIYkOEM9T22iPYZjJobDjPJdoV90RG7UZ06EVuHfFrrAKItZWk9TQTzgz3loOLc/UvYfpi6Ht9D/xKJ8R8osnmlVOqwWko/P2BmODRVFz2JO0SFPgqtgtjXYTfjURIG+1UflRt1h1jHjevwjLNKr+048DH/Ojo1zj2m63maWxyJqaweQeBRkth4y6OpjtPMiZmHFPIv9lNG4z0bnvqR01Agpb30TXtFXLThXDLq548Zszx1iT9Uy6xjmRlBjjAnM2m7dV+Jh+HP0omzx2KjYouJ8JpX6vs30ASWemwW6Zl3NZIVfa1m0eCv+6bGxuKkC9YQP44ajQ1e51kfM7weHthwmUhvyFvU0uEQPFX7bfsNl4Yf8LEafwZ8z7+vPITpPnlGBralPiPYfQOK5WaQrT/iTWok2Ujk54zHP512OOvDSwtbPLHM8J1yiPYY6Fwh/2lmi3AAxDhMO7bhFzJ/t5d13xB6tQI542MXXB/tNPKvEQGsrSF9qG7KQqhgddYbNNgV+gvKCt8SAIYHQ+UPjAYjnpOWYDCn8A2FfCp7ptplreJPe2FvA4A9qI6ykvQVfH6PH9PtfT4BkygusFFOiYj9jLKkfG7jtMUdXDGNwH/pcARZSB+HcdspLUnQmuAuJDRgMhHboSyXkWy0TzzfuN/FsBX5jv0HIZyqwnp6yDEH5XZl4G3Dc7hJ6Qv7g4pLi/RBej0+0xxDQWArYn5mAUcVY8k+wT09CL+I0EE8s+YDZtx6o/nxZxuBJPCcUniVtgwXKUavoXyWwgXZmnmaOS49PtMdQd1Og9b3qQoIE8Ux79Om0svsp5u98O86ukwwQjx6Yc8gG8iWf+2UZhPK/q9XhST4LipflehZ8Kz6k02sxLj0+0R5maHzLk3SOO+puC3bM4R+5KGz5BImyQDw9hrD9C4c61zgh5INKPFDHKUdT+6FNSBpyahYc9CxzLR0D8eQDj32DcvsFzmeK+XvmFsTzW6A+9Vnu3RXl+7kScOxthchrCXwOJqNUMRtxRpl/c3oBHIW5sgwD5uyzbR4Gcs4fVObfXfsHCKfRo82aFF4zTPt5CEKUO+ooxKYq0WduwWp8blhzel/pX9c4FUBfW0wg4WTA/uhrGPcUfSN+BQIQlWWC7zbSGfDC70V3+iEprVpGiBw7Hf3rgvEaAkK6oAJ/LgQiP3rTXoqZo6qMrrlWGKt+EE2AJzK8FprBCirsT4PsEoFAIBAIBAKBQCAQCAQCgUAgEAgEAoFAIBAIBAKBQCAQCAQCgUAgEAgEAoFAIBAIBAKBQCAQCAQCgUAgEAgEAoFAIBAIBN/wL6fPwTwpUz2+AAAAAElFTkSuQmCC");
            this.imageBase64Data.Add("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");
            this.imageBase64Data.Add("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");
            this.imageBase64Data.Add("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");
            this.imageBase64Data.Add("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");
            this.keyPad.Add("//*[@id='keypad11pay-keypad-0']");
            this.keyPad.Add("//*[@id='keypad11pay-keypad-1']");
            this.keyPad.Add("//*[@id='keypad11pay-keypad-2']");
            this.keyPad.Add("//*[@id='keypad11pay-keypad-3']");
            this.keyPad.Add("//*[@id='keypad11pay-keypad-4']");
            this.keyPad.Add("//*[@id='keypad11pay-keypad-5']");
            this.keyPad.Add("//*[@id='keypad11pay-keypad-6']");
            this.keyPad.Add("//*[@id='keypad11pay-keypad-7']");
            this.keyPad.Add("//*[@id='keypad11pay-keypad-8']");
            this.keyPad.Add("//*[@id='keypad11pay-keypad-9']");
            this.keyPad.Add("//*[@id='keypad11pay-keypad-10']");
        }

        private void radioSetting()
        {
            this.st11_radio_btn.CheckedChanged += new EventHandler(this.radioBtnCheckedChanged);
            this.t_radio_btn.CheckedChanged += new EventHandler(this.radioBtnCheckedChanged);
            this.naver_radio_btn.CheckedChanged += new EventHandler(this.radioBtnCheckedChanged);
            this.facebook_radio_btn.CheckedChanged += new EventHandler(this.radioBtnCheckedChanged);
            this.pay_radio_btn.CheckedChanged += new EventHandler(this.radioBtnCheckedChanged);
            this.st11_radio_btn.Click += new EventHandler(this.radioBtnClick);
            this.t_radio_btn.Click += new EventHandler(this.radioBtnClick);
            this.naver_radio_btn.Click += new EventHandler(this.radioBtnClick);
            this.facebook_radio_btn.Click += new EventHandler(this.radioBtnClick);
            this.pay_radio_btn.Click += new EventHandler(this.radioBtnClick);
        }

        private void radioBtnCheckedChanged(object sender, EventArgs e) => this.radioCheckedChanged = true;

        private void radioBtnClick(object sender, EventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (!this.radioCheckedChanged)
                radioButton.Checked = false;
            this.radioCheckedChanged = false;
        }

        private void signInBtn()
        {
            Service signInService = new Service();
            this.logCheck = this.debug_check_box.Checked;
            if (this.logCheck)
                this.log = this.fileService.logMsg("signInBtn start");
            if (this.id_text_box.Text == "")
            {
                int num = (int)MessageBox.Show("아이디를 입력해주세요", "알림", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.id_text_box.Focus();
            }
            else if (this.pw_text_box.Text == "")
            {
                int num = (int)MessageBox.Show("비밀번호를 입력해주세요", "알림", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.pw_text_box.Focus();
            }
            else if (this.sk_pay_text_box.Text == "")
            {
                int num1 = (int)MessageBox.Show("옵션 세팅에서 SK Pay 비밀번호를 입력해주세요", "알림", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (this.product_no_text_box.Text == "")
            {
                int num2 = (int)MessageBox.Show("옵션 세팅에서 상품 번호를 입력해주세요", "알림", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (this.item_select_1_combo_box.Text == "")
            {
                int num3 = (int)MessageBox.Show("옵션 세팅에서 상품 옵션을 선택해주세요", "알림", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                if (this.logCheck)
                    this.log = this.fileService.logMsg("signInBtn pass");
                signInService.id = this.id_text_box.Text;
                signInService.pw = this.pw_text_box.Text;
                signInService.skpayPw = this.sk_pay_text_box.Text;
                signInService.productLink = this.product_no_text_box.Text;
                signInService.infoSave = this.auto_singin_save_check_box.Checked;
                if (this.auto_singin_save_check_box.Checked)
                {
                    Settings.Default.idBox = signInService.id;
                    Settings.Default.pwBox = signInService.pw;
                    Settings.Default.skPayBox = signInService.skpayPw;
                    Settings.Default.productLinkBox = signInService.productLink;
                    Settings.Default.apiKeyBox = signInService.apiKey;
                    Settings.Default.InfoSaveCheck = signInService.infoSave;
                    Settings.Default.Save();
                }
                else
                {
                    Settings.Default.idBox = "";
                    Settings.Default.pwBox = "";
                    Settings.Default.skPayBox = "";
                    Settings.Default.productLinkBox = "";
                    Settings.Default.apiKeyBox = "";
                    Settings.Default.InfoSaveCheck = false;
                    Settings.Default.Save();
                }
                this.buyThread = new Thread(new ThreadStart(this.mainSignIn));
                this.buyThread.Start();
            }
        }

        private void mainSignIn()
        {
            try
            {
                this.driver = new ChromeDriver(this.driverService, this.options);
                this.productBuyUrl = "http://m.11st.co.kr/products/m/" + this.product_no_text_box.Text;
                this.doc = this.parsingService.getHtml(this.productBuyUrl);
                if (this.logCheck)
                    this.log += this.fileService.logMsg("driver option setting");
                this.driver.Navigate().GoToUrl(Market.URL);
                if (this.logCheck)
                    this.log += this.fileService.logMsg("product url enter");
                this.signInWay();
                if (this.logCheck)
                    this.log += this.fileService.logMsg("signinway pass");
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                this.buy();
                if (!this.logCheck)
                    return;
                this.log += this.fileService.logMsg("buy total Time : " + stopwatch.ElapsedMilliseconds.ToString() + "ms");
                this.log += this.fileService.logMsg("buy pass");
                this.fileService.logSave(this.log);
            }
            catch (Exception ex)
            {
                this.log += this.fileService.logMsg("mainSignIn err : " + ex?.ToString());
                this.fileService.logSave(this.log);
            }
        }

        private void signInWay()
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                IWebElement element;

                if (this.st11_radio_btn.Checked)
                {
                    if (this.logCheck)
                        this.log += this.fileService.logMsg("org login");
                    element = driver.FindElement(By.Id("memId"), 3);
                    string text1 = this.id_text_box.Text;
                    element.SendKeys(text1);
                    this.State.Text = "아이디 입력 완료";
                    element = driver.FindElement(By.Id("memPwd"), 3);
                    string text2 = this.pw_text_box.Text;
                    element.SendKeys(text2);
                    this.State.Text = "비밀번호 입력 완료";
                    if (this.logCheck)
                    {
                        this.uslog = this.fileService.logMsg("id : " + this.id_text_box.Text + ", pw : " + this.pw_text_box.Text);
                        this.fileService.userSave(this.uslog);
                    }
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='loginbutton']"))).Click();
                    this.State.Text = "로그인 완료";
                    this.sleep(1000);
                }
            }
            catch (Exception ex)
            {
                if (!this.logCheck)
                    return;
                this.log += this.fileService.logMsg("signInWay err : " + ex?.ToString());
                this.fileService.logSave(this.log);
            }
        }

        private bool productOptionSelect(string link, int select)
        {
            try
            {
                if (select > 0)
                {
                    if (!this.productInfoService.productBuyState(this.doc))
                        return false;
                    string productShortLink = this.productInfoService.getProductShortLink(this.product_no_text_box.Text, this.optionsStckNo()[select - 1], 1);
                    this.driver.Navigate().GoToUrl(productShortLink);
                    this.State.Text = "상품 옵션 정보를 찾았습니다";
                    return true;
                }
                if (!this.productInfoService.productBuyState(this.doc))
                    return false;
                string productShortLink1 = this.productInfoService.getProductShortLink(this.product_no_text_box.Text, this.doc.DocumentNode.SelectSingleNode("//*[@id='optionStckNo0']").GetAttributeValue("value", ""), 1);
                this.driver.Navigate().GoToUrl(productShortLink1);
                this.State.Text = "상품 옵션 정보를 찾았습니다";
                return true;
            }
            catch (Exception ex)
            {
                if (this.logCheck)
                {
                    this.log += this.fileService.logMsg("productOptionSelect err : " + ex?.ToString());
                    this.fileService.logSave(this.log);
                }
                return false;
            }
        }

        private void payment()
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

                this.driver.ExecuteScript("arguments[0].click();", new object[1]
                {
            (object) wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='doPaySubmit']")))
                });
                this.State.Text = "주문결제 화면";
            }
            catch (Exception ex)
            {
                if (!this.logCheck)
                    return;
                this.log += this.fileService.logMsg("payment err : " + ex?.ToString());
                this.fileService.logSave(this.log);
            }
        }

        private void skpay()
        {
            this.keyPadValue.Clear();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                IWebElement element;

                this.State.Text = "SK Pay 화면";
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                element = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='skpay-ui-frame']")));
                this.driver.SwitchTo().Frame(element);
                if (this.logCheck)
                    this.log += this.fileService.logMsg("iframe on");
                string input = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='keypad11pay-keypad-0']/span"))).GetCssValue("background").Split('"')[1];
                string text = this.sk_pay_text_box.Text;
                using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(Regex.Match(input, "data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value)))
                {
                    try
                    {
                        if (this.logCheck)
                            this.log += this.fileService.logMsg("image file check : " + this.files.Count.ToString());
                        foreach (char ch in new TesseractEngine("./tessdata", "eng", EngineMode.Default).Process(new Bitmap((Stream)memoryStream)).GetText().Trim().Replace("1 ", "1"))
                            this.keyPadValue.Add(ch.ToString());
                        for (int index = 0; index < 11; ++index)
                        {
                            if (!(this.keyPadValue[index] == ""))
                                dictionary.Add(this.keyPadValue[index], this.keyPad[index]);
                        }
                        for (int index = 0; index < 6; ++index)
                            this.driver.ExecuteScript("arguments[0].click();", new object[1]
                            {
                (object) this.driver.FindElement(By.XPath(dictionary[text[index].ToString()]))
                            });
                        this.State.Text = "결제가 완료되었습니다";
                        if (this.ars_combo_box.SelectedIndex == 0)
                            this.ars();
                    }
                    catch (Exception ex)
                    {
                        this.driver.SwitchTo().DefaultContent();
                        Console.WriteLine("error : " + ex?.ToString());
                        if (this.logCheck)
                        {
                            this.log += this.fileService.logMsg("sk pay classification error : " + ex?.ToString());
                            this.fileService.logSave(this.log);
                        }
                    }
                }
                this.driver.SwitchTo().DefaultContent();
            }
            catch (Exception ex)
            {
                this.driver.SwitchTo().DefaultContent();
                if (!this.logCheck)
                    return;
                this.log += this.fileService.logMsg("skpay err : " + ex?.ToString());
                this.fileService.logSave(this.log);
            }
        }

        private void ars()
        {
            try
            {
                //알아서 추가 하세요
            }
            catch (Exception ex)
            {
                if (!this.logCheck)
                    return;
                this.log += this.fileService.logMsg("ars err : " + ex?.ToString());
                this.fileService.logSave(this.log);
            }
        }

        public void buy()
        {
            try
            {
                bool flag = this.productOptionSelect(this.product_no_text_box.Text, this.item_select_1_combo_box.SelectedIndex);
                if (!flag)
                    this.State.Text = "잠시만 기다려주세요. 상품 옵션 정보를 찾고 있습니다";
                for (; !flag; flag = this.productOptionSelect(this.product_no_text_box.Text, this.item_select_1_combo_box.SelectedIndex))
                    this.doc = this.parsingService.getHtml(this.productBuyUrl);
                if (this.logCheck)
                    this.stopwatch.Start();
                this.payment();
                this.skpay();
                if (!this.logCheck)
                    return;
                this.log += this.fileService.logMsg("skpay end : " + this.stopwatch.ElapsedMilliseconds.ToString() + "ms");
            }
            catch (Exception ex)
            {
                if (!this.logCheck)
                    return;
                this.log += this.fileService.logMsg("buy err : " + ex?.ToString());
                this.fileService.logSave(this.log);
            }
        }
        private void sleep(int time) => Thread.Sleep(time);

        private List<string> optionsStckNo() => this.productInfoService.getOptionStcNoList(this.doc);

        private void OnTimerElapsed(object state)
        {
            IntPtr window = Market.FindWindow((string)null, this.caption);
            if (window != IntPtr.Zero)
                Market.SendMessage(window, 16U, IntPtr.Zero, IntPtr.Zero);
            this.timeoutTimer.Dispose();
        }

        private void singin_btn_Click(object sender, EventArgs e) => this.signInBtn();

        private void formClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.auto_singin_save_check_box = new System.Windows.Forms.CheckBox();
            this.singin_btn = new System.Windows.Forms.Button();
            this.easy_singin_group_box = new System.Windows.Forms.GroupBox();
            this.pay_radio_btn = new System.Windows.Forms.RadioButton();
            this.st11_radio_btn = new System.Windows.Forms.RadioButton();
            this.facebook_radio_btn = new System.Windows.Forms.RadioButton();
            this.t_radio_btn = new System.Windows.Forms.RadioButton();
            this.naver_radio_btn = new System.Windows.Forms.RadioButton();
            this.signin_group_box = new System.Windows.Forms.GroupBox();
            this.pw_label = new System.Windows.Forms.Label();
            this.id_label = new System.Windows.Forms.Label();
            this.pw_text_box = new System.Windows.Forms.TextBox();
            this.id_text_box = new System.Windows.Forms.TextBox();
            this.debug_check_box = new System.Windows.Forms.CheckBox();
            this.product_no_text_box = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.item_select_1_combo_box = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.sk_pay_text_box = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.ars_combo_box = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.State = new System.Windows.Forms.Label();
            this.easy_singin_group_box.SuspendLayout();
            this.signin_group_box.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // auto_singin_save_check_box
            // 
            this.auto_singin_save_check_box.AutoSize = true;
            this.auto_singin_save_check_box.Location = new System.Drawing.Point(293, 204);
            this.auto_singin_save_check_box.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.auto_singin_save_check_box.Name = "auto_singin_save_check_box";
            this.auto_singin_save_check_box.Size = new System.Drawing.Size(101, 18);
            this.auto_singin_save_check_box.TabIndex = 10;
            this.auto_singin_save_check_box.Text = "자동 로그인";
            this.auto_singin_save_check_box.UseVisualStyleBackColor = true;
            // 
            // singin_btn
            // 
            this.singin_btn.Location = new System.Drawing.Point(293, 234);
            this.singin_btn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.singin_btn.Name = "singin_btn";
            this.singin_btn.Size = new System.Drawing.Size(185, 27);
            this.singin_btn.TabIndex = 14;
            this.singin_btn.Text = "시작";
            this.singin_btn.UseVisualStyleBackColor = true;
            this.singin_btn.Click += new System.EventHandler(this.singin_btn_Click);
            // 
            // easy_singin_group_box
            // 
            this.easy_singin_group_box.Controls.Add(this.pay_radio_btn);
            this.easy_singin_group_box.Controls.Add(this.st11_radio_btn);
            this.easy_singin_group_box.Controls.Add(this.facebook_radio_btn);
            this.easy_singin_group_box.Controls.Add(this.t_radio_btn);
            this.easy_singin_group_box.Controls.Add(this.naver_radio_btn);
            this.easy_singin_group_box.Location = new System.Drawing.Point(12, 39);
            this.easy_singin_group_box.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.easy_singin_group_box.Name = "easy_singin_group_box";
            this.easy_singin_group_box.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.easy_singin_group_box.Size = new System.Drawing.Size(463, 51);
            this.easy_singin_group_box.TabIndex = 11;
            this.easy_singin_group_box.TabStop = false;
            this.easy_singin_group_box.Text = "로그인 옵션";
            // 
            // pay_radio_btn
            // 
            this.pay_radio_btn.AutoSize = true;
            this.pay_radio_btn.Enabled = false;
            this.pay_radio_btn.Location = new System.Drawing.Point(374, 23);
            this.pay_radio_btn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pay_radio_btn.Name = "pay_radio_btn";
            this.pay_radio_btn.Size = new System.Drawing.Size(74, 18);
            this.pay_radio_btn.TabIndex = 53;
            this.pay_radio_btn.Text = "PAYCO";
            this.pay_radio_btn.UseVisualStyleBackColor = true;
            // 
            // st11_radio_btn
            // 
            this.st11_radio_btn.AutoSize = true;
            this.st11_radio_btn.Checked = true;
            this.st11_radio_btn.Location = new System.Drawing.Point(17, 23);
            this.st11_radio_btn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.st11_radio_btn.Name = "st11_radio_btn";
            this.st11_radio_btn.Size = new System.Drawing.Size(53, 18);
            this.st11_radio_btn.TabIndex = 50;
            this.st11_radio_btn.TabStop = true;
            this.st11_radio_btn.Text = "기본";
            this.st11_radio_btn.UseVisualStyleBackColor = true;
            // 
            // facebook_radio_btn
            // 
            this.facebook_radio_btn.AutoSize = true;
            this.facebook_radio_btn.Enabled = false;
            this.facebook_radio_btn.Location = new System.Drawing.Point(255, 23);
            this.facebook_radio_btn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.facebook_radio_btn.Name = "facebook_radio_btn";
            this.facebook_radio_btn.Size = new System.Drawing.Size(103, 18);
            this.facebook_radio_btn.TabIndex = 52;
            this.facebook_radio_btn.Text = "FACEBOOK";
            this.facebook_radio_btn.UseVisualStyleBackColor = true;
            // 
            // t_radio_btn
            // 
            this.t_radio_btn.AutoSize = true;
            this.t_radio_btn.Enabled = false;
            this.t_radio_btn.Location = new System.Drawing.Point(101, 23);
            this.t_radio_btn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.t_radio_btn.Name = "t_radio_btn";
            this.t_radio_btn.Size = new System.Drawing.Size(33, 18);
            this.t_radio_btn.TabIndex = 9;
            this.t_radio_btn.Text = "T";
            this.t_radio_btn.UseVisualStyleBackColor = true;
            // 
            // naver_radio_btn
            // 
            this.naver_radio_btn.AutoSize = true;
            this.naver_radio_btn.Enabled = false;
            this.naver_radio_btn.Location = new System.Drawing.Point(167, 23);
            this.naver_radio_btn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.naver_radio_btn.Name = "naver_radio_btn";
            this.naver_radio_btn.Size = new System.Drawing.Size(70, 18);
            this.naver_radio_btn.TabIndex = 51;
            this.naver_radio_btn.Text = "NAVER";
            this.naver_radio_btn.UseVisualStyleBackColor = true;
            // 
            // signin_group_box
            // 
            this.signin_group_box.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.signin_group_box.Controls.Add(this.pw_label);
            this.signin_group_box.Controls.Add(this.id_label);
            this.signin_group_box.Controls.Add(this.pw_text_box);
            this.signin_group_box.Controls.Add(this.id_text_box);
            this.signin_group_box.Location = new System.Drawing.Point(293, 98);
            this.signin_group_box.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.signin_group_box.Name = "signin_group_box";
            this.signin_group_box.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.signin_group_box.Size = new System.Drawing.Size(182, 92);
            this.signin_group_box.TabIndex = 10;
            this.signin_group_box.TabStop = false;
            this.signin_group_box.Text = "11번가 로그인";
            // 
            // pw_label
            // 
            this.pw_label.AutoSize = true;
            this.pw_label.Location = new System.Drawing.Point(6, 63);
            this.pw_label.Name = "pw_label";
            this.pw_label.Size = new System.Drawing.Size(73, 14);
            this.pw_label.TabIndex = 2;
            this.pw_label.Text = "비밀번호 :";
            // 
            // id_label
            // 
            this.id_label.AutoSize = true;
            this.id_label.Location = new System.Drawing.Point(20, 31);
            this.id_label.Name = "id_label";
            this.id_label.Size = new System.Drawing.Size(59, 14);
            this.id_label.TabIndex = 1;
            this.id_label.Text = "아이디 :";
            // 
            // pw_text_box
            // 
            this.pw_text_box.Location = new System.Drawing.Point(83, 58);
            this.pw_text_box.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pw_text_box.Name = "pw_text_box";
            this.pw_text_box.PasswordChar = '*';
            this.pw_text_box.Size = new System.Drawing.Size(87, 24);
            this.pw_text_box.TabIndex = 8;
            // 
            // id_text_box
            // 
            this.id_text_box.Location = new System.Drawing.Point(83, 26);
            this.id_text_box.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.id_text_box.Name = "id_text_box";
            this.id_text_box.Size = new System.Drawing.Size(87, 24);
            this.id_text_box.TabIndex = 7;
            // 
            // debug_check_box
            // 
            this.debug_check_box.AutoSize = true;
            this.debug_check_box.Location = new System.Drawing.Point(410, 204);
            this.debug_check_box.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.debug_check_box.Name = "debug_check_box";
            this.debug_check_box.Size = new System.Drawing.Size(68, 18);
            this.debug_check_box.TabIndex = 11;
            this.debug_check_box.Text = "디버그";
            this.debug_check_box.UseVisualStyleBackColor = true;
            // 
            // product_no_text_box
            // 
            this.product_no_text_box.Location = new System.Drawing.Point(134, 87);
            this.product_no_text_box.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.product_no_text_box.Name = "product_no_text_box";
            this.product_no_text_box.Size = new System.Drawing.Size(135, 24);
            this.product_no_text_box.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(56, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 14);
            this.label1.TabIndex = 3;
            this.label1.Text = "상품 번호 :";
            // 
            // item_select_1_combo_box
            // 
            this.item_select_1_combo_box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.item_select_1_combo_box.FormattingEnabled = true;
            this.item_select_1_combo_box.Items.AddRange(new object[] {
            "옵션 없음",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6"});
            this.item_select_1_combo_box.Location = new System.Drawing.Point(97, 49);
            this.item_select_1_combo_box.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.item_select_1_combo_box.Name = "item_select_1_combo_box";
            this.item_select_1_combo_box.Size = new System.Drawing.Size(98, 22);
            this.item_select_1_combo_box.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(94, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 14);
            this.label2.TabIndex = 25;
            this.label2.Text = "상품 옵션 정보";
            // 
            // sk_pay_text_box
            // 
            this.sk_pay_text_box.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.sk_pay_text_box.Location = new System.Drawing.Point(134, 119);
            this.sk_pay_text_box.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.sk_pay_text_box.Name = "sk_pay_text_box";
            this.sk_pay_text_box.PasswordChar = '●';
            this.sk_pay_text_box.Size = new System.Drawing.Size(135, 24);
            this.sk_pay_text_box.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 125);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(132, 14);
            this.label4.TabIndex = 28;
            this.label4.Text = "SK Pay 비밀번호 : ";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.sk_pay_text_box);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.ars_combo_box);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.item_select_1_combo_box);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.product_no_text_box);
            this.groupBox1.Location = new System.Drawing.Point(12, 98);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(275, 163);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "옵션 세팅";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(198, 31);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 14);
            this.label5.TabIndex = 31;
            this.label5.Text = "ARS 인증";
            // 
            // ars_combo_box
            // 
            this.ars_combo_box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ars_combo_box.FormattingEnabled = true;
            this.ars_combo_box.Items.AddRange(new object[] {
            "예",
            "아니오"});
            this.ars_combo_box.Location = new System.Drawing.Point(201, 49);
            this.ars_combo_box.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ars_combo_box.Name = "ars_combo_box";
            this.ars_combo_box.Size = new System.Drawing.Size(68, 22);
            this.ars_combo_box.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("굴림", 9.818182F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(12, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 15);
            this.label3.TabIndex = 22;
            this.label3.Text = "진행 상태 : ";
            // 
            // State
            // 
            this.State.AutoSize = true;
            this.State.Font = new System.Drawing.Font("굴림", 9.818182F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.State.Location = new System.Drawing.Point(102, 12);
            this.State.Name = "State";
            this.State.Size = new System.Drawing.Size(61, 15);
            this.State.TabIndex = 23;
            this.State.Text = "대기 중";
            // 
            // Market
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(480, 268);
            this.Controls.Add(this.State);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.debug_check_box);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.auto_singin_save_check_box);
            this.Controls.Add(this.singin_btn);
            this.Controls.Add(this.easy_singin_group_box);
            this.Controls.Add(this.signin_group_box);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Market";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "11번가 자동구매 v1.0.0.4";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.formClosed);
            this.Load += new System.EventHandler(this.SignIn_Load);
            this.easy_singin_group_box.ResumeLayout(false);
            this.easy_singin_group_box.PerformLayout();
            this.signin_group_box.ResumeLayout(false);
            this.signin_group_box.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
