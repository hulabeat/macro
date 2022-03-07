namespace macro
{
  internal class Service
  {
    private string userId;
    private string userPw;
    private string userSkPayPw;
    private string userApiKey;
    private string userProductLink;
    private int userTimeSleep;
    private bool userOptionSave;
    private bool userInfoSave;
    private bool userLogSave;

    public string id
    {
      get => this.userId;
      set => this.userId = value;
    }

    public string pw
    {
      get => this.userPw;
      set => this.userPw = value;
    }

    public string skpayPw
    {
      get => this.userSkPayPw;
      set => this.userSkPayPw = value;
    }

    public string productLink
    {
      get => this.userProductLink;
      set => this.userProductLink = value;
    }

    public string apiKey
    {
      get => this.userApiKey;
      set => this.userApiKey = value;
    }

    public int sleepTime
    {
      get => this.userTimeSleep;
      set => this.userTimeSleep = value;
    }

    public bool optionSave
    {
      get => this.userOptionSave;
      set => this.userOptionSave = value;
    }

    public bool infoSave
    {
      get => this.userInfoSave;
      set => this.userInfoSave = value;
    }

    public bool logSave
    {
      get => this.userLogSave;
      set => this.userLogSave = value;
    }
  }
}
