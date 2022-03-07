using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace macro.Properties
{
  [CompilerGenerated]
  [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.0.3.0")]
  internal sealed class Settings : ApplicationSettingsBase
  {
    private static Settings defaultInstance = (Settings) SettingsBase.Synchronized((SettingsBase) new Settings());

    public static Settings Default => Settings.defaultInstance;

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("")]
    public string idBox
    {
      get => (string) this[nameof (idBox)];
      set => this[nameof (idBox)] = (object) value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("")]
    public string pwBox
    {
      get => (string) this[nameof (pwBox)];
      set => this[nameof (pwBox)] = (object) value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("")]
    public string apiKeyBox
    {
      get => (string) this[nameof (apiKeyBox)];
      set => this[nameof (apiKeyBox)] = (object) value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("")]
    public string skPayBox
    {
      get => (string) this[nameof (skPayBox)];
      set => this[nameof (skPayBox)] = (object) value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("")]
    public string productLinkBox
    {
      get => (string) this[nameof (productLinkBox)];
      set => this[nameof (productLinkBox)] = (object) value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("300")]
    public int sleepTimeBox
    {
      get => (int) this[nameof (sleepTimeBox)];
      set => this[nameof (sleepTimeBox)] = (object) value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("False")]
    public bool optionSaveCheckBox
    {
      get => (bool) this[nameof (optionSaveCheckBox)];
      set => this[nameof (optionSaveCheckBox)] = (object) value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("False")]
    public bool InfoSaveCheck
    {
      get => (bool) this[nameof (InfoSaveCheck)];
      set => this[nameof (InfoSaveCheck)] = (object) value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("False")]
    public bool logSaveCheck
    {
      get => (bool) this[nameof (logSaveCheck)];
      set => this[nameof (logSaveCheck)] = (object) value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("0")]
    public byte 설정
    {
      get => (byte) this[nameof (설정)];
      set => this[nameof (설정)] = (object) value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("")]
    public string signin_idBox
    {
      get => (string) this[nameof (signin_idBox)];
      set => this[nameof (signin_idBox)] = (object) value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("")]
    public string signin_pwBox
    {
      get => (string) this[nameof (signin_pwBox)];
      set => this[nameof (signin_pwBox)] = (object) value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("False")]
    public bool signin_InfoSaveCheck
    {
      get => (bool) this[nameof (signin_InfoSaveCheck)];
      set => this[nameof (signin_InfoSaveCheck)] = (object) value;
    }
  }
}
