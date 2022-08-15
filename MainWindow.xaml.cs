using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using MahApps.Metro.Controls;

namespace AutoViewFB
{
	// Token: 0x02000006 RID: 6
	public partial class MainWindow : MetroWindow
	{
		// Token: 0x06000055 RID: 85 RVA: 0x00002BE2 File Offset: 0x00000DE2
		public MainWindow()
		{
			this.InitializeComponent();
			MainWindow.Instance = this;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00002BF8 File Offset: 0x00000DF8
		public void UpdateControls()
		{
			if (!base.Dispatcher.CheckAccess())
			{
				base.Dispatcher.Invoke(new Action(this.UpdateControls));
				return;
			}
			this.lbStatus.Content = "Status: Stopped";
			int numCheckedAccounts = Checker.Items.Count((CheckerItem a) => a.Status != CheckerItem.State.Unchecked && a.Status != CheckerItem.State.Checking);
			int totalAccount = Checker.Items.Count;
			if (numCheckedAccounts >= totalAccount && totalAccount > 0)
			{
				this.lbStatus.Content = "Status: Finished!";
				Checker.IsChecking = false;
			}
			else if (Checker.IsChecking)
			{
				this.lbStatus.Content = "Status: Checking...";
			}
			this.mainProgressBar.Value = (double)(Checker.Items.Any<CheckerItem>() ? ((float)numCheckedAccounts * 100f / (float)totalAccount) : 0f);
			this.btnLoad.IsEnabled = !Checker.IsChecking;
			this.btnStart.IsEnabled = numCheckedAccounts < totalAccount;
			this.btnStart.Content = (Checker.IsChecking ? "Stop" : "Start");
			this.lbChecked.Content = string.Format("Checked: {0}/{1}", numCheckedAccounts, totalAccount);
			this.dtgMain.ItemsSource = Checker.Items;
			this.dtgMain.Items.Refresh();
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00002D51 File Offset: 0x00000F51
		private void BtnLoad_Click(object sender, RoutedEventArgs e)
		{
			if (Checker.LoadFromIndex == 0)
			{
				Checker.LoadDataFromFile();
				return;
			}
			if (Checker.LoadFromIndex == 1)
			{
				Checker.LoadDataFromServer();
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00002D6E File Offset: 0x00000F6E
		private void BtnStart_Click(object sender, RoutedEventArgs e)
		{
			if (Checker.IsChecking)
			{
				Checker.Stop();
			}
			else
			{
				Checker.Start();
			}
			this.UpdateControls();
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00002D89 File Offset: 0x00000F89
		private void BtnCloseChrome_Click(object sender, RoutedEventArgs e)
		{
			Checker.CloseChrome();
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00002D90 File Offset: 0x00000F90
		private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
		{
			Checker.RunOnWindowsStartup();
			Checker.WaitSignal();
			try
			{
				this.chkOffImage.IsChecked = new bool?((Checker.ReadSetting("IsTurnOffImage") == string.Empty) ? Checker.IsTurnOffImage : bool.Parse(Checker.ReadSetting("IsTurnOffImage")));
			}
			catch
			{
			}
			try
			{
				this.chkOffProxy.IsChecked = new bool?((Checker.ReadSetting("IsTurnOffProxy") == string.Empty) ? Checker.IsTurnOffProxy : bool.Parse(Checker.ReadSetting("IsTurnOffProxy")));
			}
			catch
			{
			}
			try
			{
				this.chkFakeUA.IsChecked = new bool?((Checker.ReadSetting("FakeUserAgent") == string.Empty) ? Checker.FakeUserAgent : bool.Parse(Checker.ReadSetting("FakeUserAgent")));
			}
			catch
			{
			}
			try
			{
				this.chkLoginMBasic.IsChecked = new bool?((Checker.ReadSetting("LoginMBasic") == string.Empty) ? Checker.LoginMBasic : bool.Parse(Checker.ReadSetting("LoginMBasic")));
			}
			catch
			{
			}
			this.txbLinkFanpage.Text = Checker.ReadSetting("LinkFanpage");
			this.txbContent.Text = Checker.ReadSetting("Content");
			this.txbTotalThreads.Text = ((Checker.ReadSetting("TotalThreads") == string.Empty) ? Checker.TotalTabs.ToString() : Checker.ReadSetting("TotalThreads"));
			this.txbTotalTabs.Text = ((Checker.ReadSetting("TotalTabs") == string.Empty) ? Checker.TotalTabs.ToString() : Checker.ReadSetting("TotalTabs"));
			this.txbCloseChromeAfterMinute.Text = ((Checker.ReadSetting("CloseChromeAfterMinute") == string.Empty) ? Checker.CloseChromeAfterMinute.ToString() : Checker.ReadSetting("CloseChromeAfterMinute"));
			this.txbRepeat.Text = ((Checker.ReadSetting("Repeat") == string.Empty) ? Checker.Repeat.ToString() : Checker.ReadSetting("Repeat"));
			int loadFromIndex;
			if (!int.TryParse(Checker.ReadSetting("LoadFromIndex"), out loadFromIndex))
			{
				loadFromIndex = 0;
			}
			this.cbLoadFrom.SelectedIndex = loadFromIndex;
			this.txbFromAcc.Text = Checker.ReadSetting("FromAcc");
			this.txbToAcc.Text = Checker.ReadSetting("ToAcc");
			this.txbDelayThread.Text = Checker.ReadSetting("DelayThread");
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00003054 File Offset: 0x00001254
		private void DtgMain_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			Checker.LoadDataFromClipboard();
		}

		// Token: 0x0600005C RID: 92 RVA: 0x0000305C File Offset: 0x0000125C
		private void MenuIemCPUid_Click(object sender, RoutedEventArgs e)
		{
			List<CheckerItem> list = this.dtgMain.SelectedItems.Cast<CheckerItem>().ToList<CheckerItem>();
			string result = "";
			foreach (CheckerItem item in list)
			{
				result = result + item.Uid + "\n";
			}
			result = result.TrimEnd(new char[] { '\n' });
			Clipboard.SetDataObject(result);
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000030E8 File Offset: 0x000012E8
		private void MenuIemCPProxy_Click(object sender, RoutedEventArgs e)
		{
			List<CheckerItem> list = this.dtgMain.SelectedItems.Cast<CheckerItem>().ToList<CheckerItem>();
			string result = "";
			foreach (CheckerItem item in list)
			{
				result = string.Concat(new string[]
				{
					result,
					item.Proxy.IP,
					":",
					item.Proxy.Port.ToString(),
					":",
					item.Proxy.Username,
					"|",
					item.Proxy.Password,
					"\n"
				});
			}
			result = result.TrimEnd(new char[] { '\n' });
			Clipboard.SetDataObject(result);
		}

		// Token: 0x0600005E RID: 94 RVA: 0x000031D8 File Offset: 0x000013D8
		private void MenuIemCPAll_Click(object sender, RoutedEventArgs e)
		{
			List<CheckerItem> list = this.dtgMain.SelectedItems.Cast<CheckerItem>().ToList<CheckerItem>();
			string result = "";
			foreach (CheckerItem item in list)
			{
				if (string.IsNullOrEmpty(item.Cookie))
				{
					result = string.Concat(new string[]
					{
						result,
						item.Uid,
						"|",
						item.Password,
						"|",
						item.SecretKey,
						"|",
						item.Proxy.IP,
						":",
						item.Proxy.Port.ToString(),
						":",
						item.Proxy.Username,
						":",
						item.Proxy.Password,
						"\n"
					});
				}
				else
				{
					result = string.Concat(new string[]
					{
						result,
						item.Cookie,
						"|",
						item.Proxy.IP,
						":",
						item.Proxy.Port.ToString(),
						":",
						item.Proxy.Username,
						":",
						item.Proxy.Password,
						"\n"
					});
				}
			}
			result = result.TrimEnd(new char[] { '\n' });
			Clipboard.SetDataObject(result);
		}

		// Token: 0x0600005F RID: 95 RVA: 0x000033A8 File Offset: 0x000015A8
		private void MenuIemSaveData_Click(object sender, RoutedEventArgs e)
		{
			string data = string.Empty;
			foreach (CheckerItem item in Checker.Items)
			{
				if (string.IsNullOrEmpty(item.Cookie))
				{
					data = string.Concat(new string[]
					{
						data,
						item.Uid,
						"|",
						item.Password,
						"|",
						item.SecretKey,
						"|",
						item.Proxy.IP,
						":",
						item.Proxy.Port.ToString(),
						":",
						item.Proxy.Username,
						":",
						item.Proxy.Password,
						"\n"
					});
				}
				else
				{
					data = string.Concat(new string[]
					{
						data,
						item.Cookie,
						"|",
						item.Proxy.IP,
						":",
						item.Proxy.Port.ToString(),
						":",
						item.Proxy.Username,
						":",
						item.Proxy.Password,
						"\n"
					});
				}
			}
			File.WriteAllText("data.txt", data);
		}

		// Token: 0x06000060 RID: 96 RVA: 0x0000355C File Offset: 0x0000175C
		private void ChkFakeUA_Click(object sender, RoutedEventArgs e)
		{
			Checker.FakeUserAgent = this.chkFakeUA.IsChecked.GetValueOrDefault();
			Checker.AddUpdateAppSettings("FakeUserAgent", this.chkFakeUA.IsChecked.ToString());
		}

		// Token: 0x06000061 RID: 97 RVA: 0x000035A4 File Offset: 0x000017A4
		private void ChkOffImage_Click(object sender, RoutedEventArgs e)
		{
			Checker.IsTurnOffImage = this.chkOffImage.IsChecked.GetValueOrDefault();
			Checker.AddUpdateAppSettings("IsTurnOffImage", this.chkOffImage.IsChecked.ToString());
		}

		// Token: 0x06000062 RID: 98 RVA: 0x000035EC File Offset: 0x000017EC
		private void ChkOffProxy_Click(object sender, RoutedEventArgs e)
		{
			Checker.IsTurnOffProxy = this.chkOffProxy.IsChecked.GetValueOrDefault();
			Checker.AddUpdateAppSettings("IsTurnOffProxy", this.chkOffProxy.IsChecked.ToString());
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00003634 File Offset: 0x00001834
		private void ChkLoginMBasic_Click(object sender, RoutedEventArgs e)
		{
			Checker.LoginMBasic = this.chkLoginMBasic.IsChecked.GetValueOrDefault();
			Checker.AddUpdateAppSettings("LoginMBasic", this.chkLoginMBasic.IsChecked.ToString());
		}

		// Token: 0x06000064 RID: 100 RVA: 0x0000367C File Offset: 0x0000187C
		private void TxbTotalThreads_TextChanged(object sender, TextChangedEventArgs e)
		{
			int totalThreads;
			if (!int.TryParse(this.txbTotalThreads.Text, out totalThreads))
			{
				totalThreads = 1;
			}
			Checker.TotalThreads = totalThreads;
			Checker.AddUpdateAppSettings("TotalThreads", this.txbTotalThreads.Text);
		}

		// Token: 0x06000065 RID: 101 RVA: 0x000036BC File Offset: 0x000018BC
		private void TxbTotalTabs_TextChanged(object sender, TextChangedEventArgs e)
		{
			int totalTabs;
			if (!int.TryParse(this.txbTotalTabs.Text, out totalTabs))
			{
				totalTabs = 1;
			}
			Checker.TotalTabs = totalTabs;
			Checker.AddUpdateAppSettings("TotalTabs", this.txbTotalTabs.Text);
		}

		// Token: 0x06000066 RID: 102 RVA: 0x000036FA File Offset: 0x000018FA
		private void TxbLinkFanpage_TextChanged(object sender, TextChangedEventArgs e)
		{
			Checker.LinkFanpage = this.txbLinkFanpage.Text;
			Checker.AddUpdateAppSettings("LinkFanpage", this.txbLinkFanpage.Text);
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00003721 File Offset: 0x00001921
		private void TxbContent_TextChanged(object sender, TextChangedEventArgs e)
		{
			Checker.Content = this.txbContent.Text;
			Checker.AddUpdateAppSettings("Content", this.txbContent.Text);
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00003748 File Offset: 0x00001948
		private void TxbCloseChromeAfterMinute_TextChanged(object sender, TextChangedEventArgs e)
		{
			int closeChromeAfterMinute;
			if (!int.TryParse(this.txbCloseChromeAfterMinute.Text, out closeChromeAfterMinute))
			{
				closeChromeAfterMinute = 60;
			}
			Checker.CloseChromeAfterMinute = closeChromeAfterMinute;
			Checker.AddUpdateAppSettings("CloseChromeAfterMinute", this.txbCloseChromeAfterMinute.Text);
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00003788 File Offset: 0x00001988
		private void TxbRepeat_TextChanged(object sender, TextChangedEventArgs e)
		{
			int repeat;
			if (!int.TryParse(this.txbRepeat.Text, out repeat))
			{
				repeat = 0;
			}
			Checker.Repeat = repeat;
			Checker.AddUpdateAppSettings("Repeat", this.txbRepeat.Text);
		}

		// Token: 0x0600006A RID: 106 RVA: 0x000037C8 File Offset: 0x000019C8
		private void TxbFromAcc_TextChanged(object sender, TextChangedEventArgs e)
		{
			int fromAcc;
			if (!int.TryParse(this.txbFromAcc.Text, out fromAcc))
			{
				fromAcc = 0;
			}
			Checker.FromAcc = fromAcc;
			Checker.AddUpdateAppSettings("FromAcc", this.txbFromAcc.Text);
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00003808 File Offset: 0x00001A08
		private void TxbToAcc_TextChanged(object sender, TextChangedEventArgs e)
		{
			int toAcc;
			if (!int.TryParse(this.txbToAcc.Text, out toAcc))
			{
				toAcc = 0;
			}
			Checker.ToAcc = toAcc;
			Checker.AddUpdateAppSettings("ToAcc", this.txbToAcc.Text);
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00003848 File Offset: 0x00001A48
		private void TxbDelayThread_TextChanged(object sender, TextChangedEventArgs e)
		{
			int delayThread;
			if (!int.TryParse(this.txbDelayThread.Text, out delayThread))
			{
				delayThread = 100;
			}
			Checker.DelayThread = delayThread;
			Checker.AddUpdateAppSettings("DelayThread", this.txbDelayThread.Text);
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00003888 File Offset: 0x00001A88
		private void CbLoadFrom_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Checker.LoadFromIndex = this.cbLoadFrom.SelectedIndex;
			Checker.AddUpdateAppSettings("LoadFromIndex", this.cbLoadFrom.SelectedIndex.ToString());
			if (this.txbFromAcc == null || this.txbToAcc == null)
			{
				return;
			}
			if (this.cbLoadFrom.SelectedIndex == 0)
			{
				this.txbFromAcc.Visibility = Visibility.Collapsed;
				this.txbToAcc.Visibility = Visibility.Collapsed;
				return;
			}
			this.txbFromAcc.Visibility = Visibility.Visible;
			this.txbToAcc.Visibility = Visibility.Visible;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00003911 File Offset: 0x00001B11
		private void BtnLoadSettings_Click(object sender, RoutedEventArgs e)
		{
			Checker.LoadSettings();
		}

		// Token: 0x04000022 RID: 34
		public static MainWindow Instance;
	}
}
