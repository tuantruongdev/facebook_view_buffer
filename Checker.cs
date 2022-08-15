using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Chrome.ChromeDriverExtensions;
using OtpNet;

namespace AutoViewFB
{
	// Token: 0x02000004 RID: 4
	internal static class Checker
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000E RID: 14 RVA: 0x0000216C File Offset: 0x0000036C
		// (set) Token: 0x0600000F RID: 15 RVA: 0x00002173 File Offset: 0x00000373
		public static bool IsChecking { get; set; } = false;

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000010 RID: 16 RVA: 0x0000217B File Offset: 0x0000037B
		// (set) Token: 0x06000011 RID: 17 RVA: 0x00002182 File Offset: 0x00000382
		public static List<CheckerItem> Items { get; set; } = new List<CheckerItem>();

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000012 RID: 18 RVA: 0x0000218A File Offset: 0x0000038A
		// (set) Token: 0x06000013 RID: 19 RVA: 0x00002191 File Offset: 0x00000391
		private static List<Thread> Threads { get; set; } = new List<Thread>();

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000014 RID: 20 RVA: 0x00002199 File Offset: 0x00000399
		// (set) Token: 0x06000015 RID: 21 RVA: 0x000021A0 File Offset: 0x000003A0
		private static Thread LoopThread { get; set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000016 RID: 22 RVA: 0x000021A8 File Offset: 0x000003A8
		// (set) Token: 0x06000017 RID: 23 RVA: 0x000021AF File Offset: 0x000003AF
		private static int NumThreads { get; set; } = 0;

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000018 RID: 24 RVA: 0x000021B7 File Offset: 0x000003B7
		// (set) Token: 0x06000019 RID: 25 RVA: 0x000021BE File Offset: 0x000003BE
		public static int TotalThreads { get; set; } = 1;

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600001A RID: 26 RVA: 0x000021C6 File Offset: 0x000003C6
		// (set) Token: 0x0600001B RID: 27 RVA: 0x000021CD File Offset: 0x000003CD
		public static int TotalTabs { get; set; } = 1;

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600001C RID: 28 RVA: 0x000021D5 File Offset: 0x000003D5
		// (set) Token: 0x0600001D RID: 29 RVA: 0x000021DC File Offset: 0x000003DC
		private static int RepeatCounter { get; set; } = 0;

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600001E RID: 30 RVA: 0x000021E4 File Offset: 0x000003E4
		// (set) Token: 0x0600001F RID: 31 RVA: 0x000021EB File Offset: 0x000003EB
		public static int Repeat { get; set; } = 0;

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000020 RID: 32 RVA: 0x000021F3 File Offset: 0x000003F3
		// (set) Token: 0x06000021 RID: 33 RVA: 0x000021FA File Offset: 0x000003FA
		public static int CloseChromeAfterMinute { get; set; } = 60;

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000022 RID: 34 RVA: 0x00002202 File Offset: 0x00000402
		// (set) Token: 0x06000023 RID: 35 RVA: 0x00002209 File Offset: 0x00000409
		public static int LoadFromIndex { get; set; } = 0;

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000024 RID: 36 RVA: 0x00002211 File Offset: 0x00000411
		// (set) Token: 0x06000025 RID: 37 RVA: 0x00002218 File Offset: 0x00000418
		public static int FromAcc { get; set; } = 0;

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000026 RID: 38 RVA: 0x00002220 File Offset: 0x00000420
		// (set) Token: 0x06000027 RID: 39 RVA: 0x00002227 File Offset: 0x00000427
		public static int ToAcc { get; set; } = 0;

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000028 RID: 40 RVA: 0x0000222F File Offset: 0x0000042F
		// (set) Token: 0x06000029 RID: 41 RVA: 0x00002236 File Offset: 0x00000436
		public static int DelayThread { get; set; } = 100;

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600002A RID: 42 RVA: 0x0000223E File Offset: 0x0000043E
		// (set) Token: 0x0600002B RID: 43 RVA: 0x00002245 File Offset: 0x00000445
		public static bool FakeUserAgent { get; set; } = false;

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600002C RID: 44 RVA: 0x0000224D File Offset: 0x0000044D
		// (set) Token: 0x0600002D RID: 45 RVA: 0x00002254 File Offset: 0x00000454
		public static bool LoginMBasic { get; set; } = false;

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600002E RID: 46 RVA: 0x0000225C File Offset: 0x0000045C
		// (set) Token: 0x0600002F RID: 47 RVA: 0x00002263 File Offset: 0x00000463
		public static bool IsTurnOffImage { get; set; } = false;

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000030 RID: 48 RVA: 0x0000226B File Offset: 0x0000046B
		// (set) Token: 0x06000031 RID: 49 RVA: 0x00002272 File Offset: 0x00000472
		public static bool IsTurnOffProxy { get; set; } = false;

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000032 RID: 50 RVA: 0x0000227A File Offset: 0x0000047A
		// (set) Token: 0x06000033 RID: 51 RVA: 0x00002281 File Offset: 0x00000481
		public static string LinkFanpage { get; set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000034 RID: 52 RVA: 0x00002289 File Offset: 0x00000489
		// (set) Token: 0x06000035 RID: 53 RVA: 0x00002290 File Offset: 0x00000490
		public static string Content { get; set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000036 RID: 54 RVA: 0x00002298 File Offset: 0x00000498
		// (set) Token: 0x06000037 RID: 55 RVA: 0x0000229F File Offset: 0x0000049F
		public static long Signal { get; set; }

		// Token: 0x06000039 RID: 57 RVA: 0x00002325 File Offset: 0x00000525
		public static void RunOnWindowsStartup()
		{
			Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true).SetValue("AutoViewFB", Assembly.GetExecutingAssembly().Location);
		}

		// Token: 0x0600003A RID: 58 RVA: 0x0000234B File Offset: 0x0000054B
		public static void WaitSignal()
		{
			new Thread(delegate()
			{
				for (;;)
				{
					try
					{
						using (WebClient client = new WebClient())
						{
							long signal = Checker.Signal;
							Checker.Signal = long.Parse(client.DownloadString("http://viewadbreak.fun/server.php?action=getSignal"));
							if (signal != Checker.Signal)
							{
								if (Checker.Signal == -1L)
								{
									Checker.Stop();
								}
								else if (Checker.Signal == -2L)
								{
									Checker.CloseChrome();
								}
								else if (Checker.Signal > DateTimeOffset.UtcNow.ToUnixTimeSeconds())
								{
									Checker.Stop();
									Checker.LoadSettings();
									if (Checker.LoadFromIndex == 0)
									{
										Checker.LoadDataFromFile();
									}
									else if (Checker.LoadFromIndex == 1)
									{
										Checker.LoadDataFromServer().Join();
									}
									Checker.Start();
								}
								MainWindow.Instance.Dispatcher.Invoke(delegate()
								{
									MainWindow.Instance.UpdateControls();
								});
							}
						}
					}
					catch
					{
					}
					finally
					{
						Thread.Sleep(15000);
					}
				}
			})
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00002380 File Offset: 0x00000580
		public static string ReadSetting(string key)
		{
			try
			{
				return ConfigurationManager.AppSettings[key] ?? string.Empty;
			}
			catch
			{
			}
			return string.Empty;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x000023C0 File Offset: 0x000005C0
		public static void AddUpdateAppSettings(string key, string value)
		{
			try
			{
				Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
				KeyValueConfigurationCollection settings = configuration.AppSettings.Settings;
				if (settings[key] == null)
				{
					settings.Add(key, value);
				}
				else
				{
					settings[key].Value = value;
				}
				configuration.Save(ConfigurationSaveMode.Modified);
				ConfigurationManager.RefreshSection(configuration.AppSettings.SectionInformation.Name);
			}
			catch
			{
			}
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00002430 File Offset: 0x00000630
		public static void CloseChrome()
		{
			try
			{
				Process[] array = Process.GetProcessesByName("chrome");
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Kill();
				}
				array = Process.GetProcessesByName("chromedriver");
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Kill();
				}
			}
			catch
			{
			}
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00002494 File Offset: 0x00000694
		public static void LoadDataFromFile()
		{
			Checker.Items = new List<CheckerItem>();
			if (!File.Exists("data.txt"))
			{
				return;
			}
			foreach (string line in (from x in File.ReadAllLines("data.txt").Distinct<string>().ToArray<string>()
				where !string.IsNullOrEmpty(x)
				select x).ToArray<string>())
			{
				if (!(line == string.Empty))
				{
					string[] arrInfo = line.Split(new char[] { '|' });
					if (arrInfo.Length == 2)
					{
						if (arrInfo[1].Split(new char[] { ':' }).Length >= 4)
						{
							Match matchUid = Regex.Match(arrInfo[0], "c_user=(?<uid>.+?);");
							if (matchUid != Match.Empty)
							{
								CheckerItem newCheckerInfo = new CheckerItem
								{
									Cookie = arrInfo[0],
									Uid = matchUid.Groups["uid"].ToString(),
									Proxy = new ProxyInfo(arrInfo[1]),
									Status = CheckerItem.State.Unchecked
								};
								Checker.Items.Add(newCheckerInfo);
							}
						}
					}
					else if (arrInfo.Length == 4 && arrInfo[3].Split(new char[] { ':' }).Length >= 4)
					{
						CheckerItem newCheckerInfo2 = new CheckerItem
						{
							Uid = arrInfo[0],
							Password = arrInfo[1],
							SecretKey = arrInfo[2],
							Proxy = new ProxyInfo(arrInfo[3]),
							Status = CheckerItem.State.Unchecked
						};
						Checker.Items.Add(newCheckerInfo2);
					}
				}
			}
			MainWindow.Instance.UpdateControls();
		}

		// Token: 0x0600003F RID: 63 RVA: 0x0000262C File Offset: 0x0000082C
		public static void LoadDataFromClipboard()
		{
			Checker.Items = new List<CheckerItem>();
			foreach (string line in (from x in Clipboard.GetText(TextDataFormat.UnicodeText).Split(new char[] { '\n' }).Distinct<string>()
					.ToArray<string>()
				where !string.IsNullOrEmpty(x)
				select x).ToArray<string>())
			{
				if (!(line == string.Empty))
				{
					string[] arrInfo = line.Split(new char[] { '|' });
					if (arrInfo.Length == 2)
					{
						if (arrInfo[1].Split(new char[] { ':' }).Length >= 4)
						{
							Match matchUid = Regex.Match(arrInfo[0], "c_user=(?<uid>.+?);");
							if (matchUid != Match.Empty)
							{
								CheckerItem newCheckerInfo = new CheckerItem
								{
									Cookie = arrInfo[0],
									Uid = matchUid.Groups["uid"].ToString(),
									Proxy = new ProxyInfo(arrInfo[1]),
									Status = CheckerItem.State.Unchecked
								};
								Checker.Items.Add(newCheckerInfo);
							}
						}
					}
					else if (arrInfo.Length == 4 && arrInfo[3].Split(new char[] { ':' }).Length >= 4)
					{
						CheckerItem newCheckerInfo2 = new CheckerItem
						{
							Uid = arrInfo[0],
							Password = arrInfo[1],
							SecretKey = arrInfo[2],
							Proxy = new ProxyInfo(arrInfo[3]),
							Status = CheckerItem.State.Unchecked
						};
						Checker.Items.Add(newCheckerInfo2);
					}
				}
			}
			MainWindow.Instance.UpdateControls();
		}

		// Token: 0x06000040 RID: 64 RVA: 0x000027C0 File Offset: 0x000009C0
		public static Thread LoadDataFromServer()
		{
			Checker.Items = new List<CheckerItem>();
			MainWindow.Instance.Dispatcher.Invoke(delegate()
			{
				MainWindow.Instance.btnLoad.IsEnabled = false;
				MainWindow.Instance.btnStart.IsEnabled = false;
			});
			Thread thread = new Thread(delegate()
			{
				try
				{
					using (WebClient client = new WebClient())
					{
						foreach (string line in (from x in client.DownloadString(string.Format("http://viewadbreak.fun/server.php?action=get&from={0}&to={1}", Checker.FromAcc, Checker.ToAcc)).Split(new char[] { '\n' }).Distinct<string>()
								.ToArray<string>()
							where !string.IsNullOrEmpty(x)
							select x).ToArray<string>())
						{
							if (!(line == string.Empty))
							{
								string[] arrInfo = line.Split(new char[] { '|' });
								if (arrInfo.Length == 2)
								{
									if (arrInfo[1].Split(new char[] { ':' }).Length >= 4)
									{
										Match matchUid = Regex.Match(arrInfo[0], "c_user=(?<uid>.+?);");
										if (matchUid != Match.Empty)
										{
											CheckerItem newCheckerInfo = new CheckerItem
											{
												Cookie = arrInfo[0],
												Uid = matchUid.Groups["uid"].ToString(),
												Proxy = new ProxyInfo(arrInfo[1]),
												Status = CheckerItem.State.Unchecked
											};
											Checker.Items.Add(newCheckerInfo);
										}
									}
								}
								else if (arrInfo.Length == 4 && arrInfo[3].Split(new char[] { ':' }).Length >= 4)
								{
									CheckerItem newCheckerInfo2 = new CheckerItem
									{
										Uid = arrInfo[0],
										Password = arrInfo[1],
										SecretKey = arrInfo[2],
										Proxy = new ProxyInfo(arrInfo[3]),
										Status = CheckerItem.State.Unchecked
									};
									Checker.Items.Add(newCheckerInfo2);
								}
							}
						}
					}
				}
				catch
				{
				}
				finally
				{
					MainWindow.Instance.Dispatcher.Invoke(delegate()
					{
						MainWindow.Instance.UpdateControls();
					});
				}
			});
			thread.IsBackground = true;
			thread.Start();
			return thread;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00002838 File Offset: 0x00000A38
		public static void LoadSettings()
		{
			MainWindow.Instance.Dispatcher.Invoke(delegate()
			{
				MainWindow.Instance.btnLoadSettings.IsEnabled = false;
			});
			new Thread(delegate()
			{
				try
				{
					using (WebClient client = new WebClient())
					{
						JObject json = JObject.Parse(client.DownloadString("http://viewadbreak.fun/server.php?action=loadSettings"));
						MainWindow.Instance.Dispatcher.Invoke(delegate()
						{
							MainWindow.Instance.txbTotalThreads.Text = ((string)json["totalThreads"]) ?? string.Empty;
							MainWindow.Instance.txbTotalTabs.Text = ((string)json["totalTabs"]) ?? string.Empty;
							MainWindow.Instance.txbLinkFanpage.Text = ((string)json["linkFanpage"]) ?? string.Empty;
							MainWindow.Instance.txbContent.Text = ((string)json["content"]) ?? string.Empty;
							MainWindow.Instance.txbCloseChromeAfterMinute.Text = ((string)json["closeChromeAfterMinute"]) ?? string.Empty;
							MainWindow.Instance.txbRepeat.Text = ((string)json["repeat"]) ?? string.Empty;
							MainWindow.Instance.txbDelayThread.Text = ((string)json["delayThread"]) ?? string.Empty;
							MainWindow.Instance.chkFakeUA.IsChecked = new bool?(((bool?)json["fakeUA"]).GetValueOrDefault());
							MainWindow.Instance.chkOffImage.IsChecked = new bool?(((bool?)json["turnOffImage"]).GetValueOrDefault());
							MainWindow.Instance.chkOffProxy.IsChecked = new bool?(((bool?)json["chkOffProxy"]).GetValueOrDefault());
							MainWindow.Instance.chkLoginMBasic.IsChecked = new bool?(((bool?)json["mBasic"]).GetValueOrDefault());
						});
					}
				}
				catch
				{
				}
				finally
				{
					MainWindow.Instance.Dispatcher.Invoke(delegate()
					{
						MainWindow.Instance.btnLoadSettings.IsEnabled = true;
					});
				}
			})
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x06000042 RID: 66 RVA: 0x000028A4 File Offset: 0x00000AA4
		public static void Start()
		{
			if (Checker.IsChecking)
			{
				return;
			}
			Checker.IsChecking = true;
			Checker.CloseChrome();
			Checker.NumThreads = 0;
			Checker.RepeatCounter = 0;
			Checker.LoopThread = new Thread(new ThreadStart(Checker.Loop))
			{
				IsBackground = true
			};
			Checker.LoopThread.Start();
		}

		// Token: 0x06000043 RID: 67 RVA: 0x000028F7 File Offset: 0x00000AF7
		public static void Stop()
		{
			if (!Checker.IsChecking)
			{
				return;
			}
			Checker.IsChecking = false;
			new Thread(delegate()
			{
				try
				{
					Checker.CloseChrome();
					foreach (Thread thread in Checker.Threads)
					{
						thread.Abort();
					}
					Checker.Threads.Clear();
					Checker.LoopThread.Abort();
					Checker.Items.Where((CheckerItem a) => a.Status == CheckerItem.State.Checking).Select(delegate(CheckerItem a)
					{
						a.Status = CheckerItem.State.Unchecked;
						return a;
					}).ToList<CheckerItem>();
					MainWindow.Instance.UpdateControls();
				}
				catch
				{
				}
			})
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00002938 File Offset: 0x00000B38
		private static void Loop()
		{
			for (;;)
			{
				if (Checker.Items.Any((CheckerItem a) => a.Status == CheckerItem.State.Unchecked))
				{
					if (Checker.NumThreads < Checker.TotalThreads)
					{
						CheckerItem item = Checker.Items.FirstOrDefault((CheckerItem a) => a.Status == CheckerItem.State.Unchecked);
						if (item == null)
						{
							goto IL_131;
						}
						Checker.NumThreads++;
						int i = (string.IsNullOrEmpty(item.Cookie) ? Checker.Items.FindIndex((CheckerItem a) => a.Uid == item.Uid && a.Password == item.Password && a.SecretKey == item.SecretKey && a.Proxy == item.Proxy) : Checker.Items.FindIndex((CheckerItem a) => a.Cookie == item.Cookie));
						Checker.Items[i].Status = CheckerItem.State.Checking;
						MainWindow.Instance.UpdateControls();
						Thread tr = new Thread(delegate()
						{
							try
							{
								ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
								chromeDriverService.HideCommandPromptWindow = true;
								ChromeOptions chromeOptions = new ChromeOptions();
								if (Checker.IsTurnOffImage)
								{
									chromeOptions.AddArgument("--blink-settings=imagesEnabled=False");
								}
								int i;
								if (!Checker.IsTurnOffProxy)
								{
									ProxyInfo proxyInfo = Checker.Items[i].Proxy;
									chromeOptions.AddHttpProxy(proxyInfo.IP, proxyInfo.Port, proxyInfo.Username, proxyInfo.Password);
								}
								chromeOptions.AddArguments(new string[]
								{
									"--disable-application-cache", "--disable-notifications", "--disable-popup-blocking", "--hide-scrollbars", "--ignore-certificate-errors", "--allow-running-insecure-content", "--no-sandbox", "--disable-gpu", "--disable-dev-shm-usage", "--disable-web-security",
									"--disable-rtc-smoothness-algorithm", "--disable-webrtc-hw-decoding", "--disable-webrtc-hw-encoding", "--disable-webrtc-multiple-routes", "--disable-webrtc-hw-vp8-encoding", "--enforce-webrtc-ip-permission-check", "--force-webrtc-ip-handling-policy", "--disable-infobars", "--disable-blink-features", "--disable-blink-features=BlockCredentialedSubresources",
									"--disable-blink-features=AutomationControlled", "--autoplay-policy=no-user-gesture-required"
								});
								if ((i + 1) % 4 == 0)
								{
									chromeOptions.AddArgument("--window-position=560,400");
								}
								else if ((i + 1) % 3 == 0)
								{
									chromeOptions.AddArgument("--window-position=0,400");
								}
								else if ((i + 1) % 2 == 0)
								{
									chromeOptions.AddArgument("--window-position=560,0");
								}
								else
								{
									chromeOptions.AddArgument("--window-position=0,0");
								}
								if (Checker.FakeUserAgent && File.Exists("UserAgent.txt"))
								{
									string[] UAs = File.ReadAllLines("UserAgent.txt");
									UAs = UAs.Distinct<string>().ToArray<string>();
									UAs = UAs.Where((string x) => !string.IsNullOrEmpty(x)).ToArray<string>();
									if (UAs.Length != 0)
									{
										Random random = new Random();
										string userAgent = UAs[random.Next(0, UAs.Length)];
										chromeOptions.AddArgument("--user-agent=" + userAgent);
									}
								}
								chromeOptions.AddUserProfilePreference("credentials_enable_service", false);
								chromeOptions.AddUserProfilePreference("profile.password_manager_enabled", false);
								chromeOptions.AddArgument("--disable-blink-features=AutomationControlled");
								chromeOptions.AddArgument("--window-size=560,548");
								chromeOptions.AddExcludedArgument("enable-automation");
								chromeOptions.AddExcludedArgument("disable-popup-blocking");
								chromeOptions.AddExtension("AlwaysActiveWindow.crx");
								chromeOptions.BinaryLocation = "GoogleChromePortable\\App\\Chrome-bin\\chrome.exe";
								ChromeDriver chromeDriver = new ChromeDriver(chromeDriverService, chromeOptions);
								for (i = 0; i < Checker.TotalTabs; i++)
								{
									Thread.Sleep(2000);
									if (i == 0)
									{
										if (string.IsNullOrEmpty(Checker.Items[i].Cookie))
										{
											if (Checker.LoginMBasic)
											{
												chromeDriver.Navigate().GoToUrl("https://mbasic.facebook.com/");
												chromeDriver.FindElement(By.Name("email")).SendKeys(Checker.Items[i].Uid);
												chromeDriver.FindElement(By.Name("pass")).SendKeys(Checker.Items[i].Password);
												chromeDriver.FindElement(By.Name("login")).Click();
											}
											else
											{
												chromeDriver.Navigate().GoToUrl("https://www.facebook.com/");
												chromeDriver.FindElement(By.Id("email")).SendKeys(Checker.Items[i].Uid);
												chromeDriver.FindElement(By.Id("pass")).SendKeys(Checker.Items[i].Password);
												chromeDriver.FindElement(By.Name("login")).Click();
											}
											Thread.Sleep(1000);
											if (chromeDriver.Url.Contains("/login/"))
											{
												Checker.Items[i].Status = CheckerItem.State.Error;
												Checker.Items[i].ErrorMessage = "Invalid username or password";
												chromeDriver.Quit();
												return;
											}
											if (chromeDriver.PageSource.Contains("id=\"approvals_code\""))
											{
												Totp otp = new Totp(Base32Encoding.ToBytes(Checker.Items[i].SecretKey), 30, OtpHashMode.Sha1, 6, null);
												chromeDriver.FindElement(By.Id("approvals_code")).SendKeys(otp.ComputeTotp());
												Thread.Sleep(1000);
												chromeDriver.FindElement(By.Id("checkpointSubmitButton")).Click();
												Thread.Sleep(1000);
												chromeDriver.FindElement(By.Id("checkpointSubmitButton")).Click();
												Thread.Sleep(1000);
												if (chromeDriver.Url.Contains("checkpoint"))
												{
													Checker.Items[i].Status = CheckerItem.State.Error;
													Checker.Items[i].ErrorMessage = "Lỗi đăng nhập";
													chromeDriver.Quit();
													return;
												}
											}
											else if (chromeDriver.Url.Contains("checkpoint"))
											{
												Checker.Items[i].Status = CheckerItem.State.Error;
												Checker.Items[i].ErrorMessage = "Checkpoint";
												chromeDriver.Quit();
												return;
											}
										}
										else
										{
											chromeDriver.Url = "https://www.facebook.com/";
											string[] array = Checker.Items[i].Cookie.Replace("; ", ";").Split(new char[] { ';' });
											for (int k = 0; k < array.Length; k++)
											{
												string[] cookieInfo = array[k].Split(new char[] { '=' });
												if (cookieInfo.Length >= 2)
												{
													chromeDriver.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie(cookieInfo[0], cookieInfo[1], ".facebook.com", "/", new DateTime?(DateTime.Now.AddDays(10.0))));
												}
											}
										}
										Thread.Sleep(1000);
										chromeDriver.Navigate().GoToUrl("https://www.facebook.com/profile.php");
										if (chromeDriver.Url.Contains("checkpoint"))
										{
											Checker.Items[i].Status = CheckerItem.State.Error;
											Checker.Items[i].ErrorMessage = "Checkpoint";
											chromeDriver.Quit();
											return;
										}
										if (chromeDriver.Url == "https://www.facebook.com/profile.php")
										{
											Checker.Items[i].Status = CheckerItem.State.Error;
											Checker.Items[i].ErrorMessage = "Cookie Die";
											chromeDriver.Quit();
											return;
										}
										chromeDriver.Navigate().GoToUrl(Checker.LinkFanpage);
									}
									else
									{
										chromeDriver.ExecuteScript("window.open('" + Checker.LinkFanpage + "', '_blank');", Array.Empty<object>());
										chromeDriver.SwitchTo().Window(chromeDriver.WindowHandles.Last<string>());
									}
									for (int j = 0; j < 5; j++)
									{
										try
										{
											chromeDriver.ExecuteScript("window.scrollBy(0, 1000)", Array.Empty<object>());
										}
										catch
										{
										}
										Thread.Sleep(2000);
									}
									Thread.Sleep(3000);
									foreach (IWebElement element in chromeDriver.FindElements(By.XPath("//*[contains(@style,'text-align: start;')]")))
									{
										if (element.Text == Checker.Content)
										{
											chromeDriver.ExecuteScript("arguments[0].scrollIntoView();", new object[] { element });
											Thread.Sleep(1000);
										}
									}
								}
								Checker.Items[i].Status = CheckerItem.State.Success;
							}
							catch (Exception ex)
							{
								int i;
								Checker.Items[i].Status = CheckerItem.State.Error;
								Checker.Items[i].ErrorMessage = ex.Message;
							}
							finally
							{
								Checker.NumThreads--;
								MainWindow.Instance.UpdateControls();
							}
						})
						{
							IsBackground = true
						};
						tr.Start();
						Checker.Threads.Add(tr);
					}
					Thread.Sleep(Checker.DelayThread);
					continue;
				}
				for (;;)
				{
					IL_131:
					if (Checker.Items.Count((CheckerItem a) => a.Status == CheckerItem.State.Checking) <= 0)
					{
						break;
					}
					Thread.Sleep(100);
				}
				Thread.Sleep(TimeSpan.FromMinutes((double)Checker.CloseChromeAfterMinute));
				Checker.CloseChrome();
				if (Checker.RepeatCounter >= Checker.Repeat)
				{
					break;
				}
				Checker.RepeatCounter++;
				Checker.NumThreads = 0;
				Checker.IsChecking = true;
				Checker.Items.Select(delegate(CheckerItem a)
				{
					a.Status = CheckerItem.State.Unchecked;
					a.ErrorMessage = string.Empty;
					return a;
				}).ToList<CheckerItem>();
				MainWindow.Instance.UpdateControls();
			}
		}
	}
}
