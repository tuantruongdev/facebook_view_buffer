using System;

namespace AutoViewFB
{
	// Token: 0x02000002 RID: 2
	internal class ProxyInfo
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		// (set) Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
		public string IP { get; set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000003 RID: 3 RVA: 0x00002061 File Offset: 0x00000261
		// (set) Token: 0x06000004 RID: 4 RVA: 0x00002069 File Offset: 0x00000269
		public int Port { get; set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000005 RID: 5 RVA: 0x00002072 File Offset: 0x00000272
		// (set) Token: 0x06000006 RID: 6 RVA: 0x0000207A File Offset: 0x0000027A
		public string Username { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000007 RID: 7 RVA: 0x00002083 File Offset: 0x00000283
		// (set) Token: 0x06000008 RID: 8 RVA: 0x0000208B File Offset: 0x0000028B
		public string Password { get; set; }

		// Token: 0x06000009 RID: 9 RVA: 0x00002094 File Offset: 0x00000294
		public ProxyInfo(string proxy)
		{
			string[] proxyInfo = proxy.Split(new char[] { ':' });
			this.IP = proxyInfo[0];
			this.Port = int.Parse(proxyInfo[1]);
			this.Username = proxyInfo[2];
			this.Password = proxyInfo[3];
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000020E4 File Offset: 0x000002E4
		public override string ToString()
		{
			return this.IP + ":" + this.Port.ToString();
		}
	}
}
