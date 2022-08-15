using System;

namespace AutoViewFB
{
	// Token: 0x02000005 RID: 5
	internal class CheckerItem
	{
		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000045 RID: 69 RVA: 0x00002B19 File Offset: 0x00000D19
		// (set) Token: 0x06000046 RID: 70 RVA: 0x00002B21 File Offset: 0x00000D21
		public string Cookie { get; set; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000047 RID: 71 RVA: 0x00002B2A File Offset: 0x00000D2A
		// (set) Token: 0x06000048 RID: 72 RVA: 0x00002B32 File Offset: 0x00000D32
		public string Uid { get; set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000049 RID: 73 RVA: 0x00002B3B File Offset: 0x00000D3B
		// (set) Token: 0x0600004A RID: 74 RVA: 0x00002B43 File Offset: 0x00000D43
		public string Password { get; set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600004B RID: 75 RVA: 0x00002B4C File Offset: 0x00000D4C
		// (set) Token: 0x0600004C RID: 76 RVA: 0x00002B54 File Offset: 0x00000D54
		public string SecretKey { get; set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600004D RID: 77 RVA: 0x00002B5D File Offset: 0x00000D5D
		// (set) Token: 0x0600004E RID: 78 RVA: 0x00002B65 File Offset: 0x00000D65
		public ProxyInfo Proxy { get; set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x0600004F RID: 79 RVA: 0x00002B6E File Offset: 0x00000D6E
		// (set) Token: 0x06000050 RID: 80 RVA: 0x00002B76 File Offset: 0x00000D76
		public string ErrorMessage { get; set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000051 RID: 81 RVA: 0x00002B7F File Offset: 0x00000D7F
		// (set) Token: 0x06000052 RID: 82 RVA: 0x00002B87 File Offset: 0x00000D87
		public CheckerItem.State Status { get; set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000053 RID: 83 RVA: 0x00002B90 File Offset: 0x00000D90
		public string StatusDisplay
		{
			get
			{
				switch (this.Status)
				{
				case CheckerItem.State.Error:
					return this.ErrorMessage;
				case CheckerItem.State.Success:
					return "Hoàn thành";
				case CheckerItem.State.Checking:
					return "Đang chạy...";
				case CheckerItem.State.Unchecked:
					return "Đang chờ";
				default:
					return string.Empty;
				}
			}
		}

		// Token: 0x0200000C RID: 12
		public enum State
		{
			// Token: 0x04000055 RID: 85
			Error,
			// Token: 0x04000056 RID: 86
			Success,
			// Token: 0x04000057 RID: 87
			Checking,
			// Token: 0x04000058 RID: 88
			Unchecked
		}
	}
}
