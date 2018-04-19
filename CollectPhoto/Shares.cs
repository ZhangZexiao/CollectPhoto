using System;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
namespace Trinet.Networking
{
#region Share Type
	[Flags]
	public enum ShareType
	{
		Disk=0,Printer=1,Device=2,IPC=3,Special= -2147483648,
	}
#endregion
#region Share
	public class Share
	{
#region Private data
		private string _server;
		private string _netName;
		private string _path;
		private ShareType _shareType;
		private string _remark;
#endregion
#region Constructor
		public Share(string server,string netName,string path,ShareType shareType,string remark)
		{
			if(ShareType.Special==shareType&&"IPC$"==netName)
			{
				shareType|=ShareType.IPC;
			}
			_server=server;
			_netName=netName;
			_path=path;
			_shareType=shareType;
			_remark=remark;
		}
#endregion
#region Properties
		public string Server
		{
			get
			{
				return _server;
			}
		}
		public string NetName
		{
			get
			{
				return _netName;
			}
		}
		public string Path
		{
			get
			{
				return _path;
			}
		}
		public ShareType ShareType
		{
			get
			{
				return _shareType;
			}
		}
		public string Remark
		{
			get
			{
				return _remark;
			}
		}
		public bool IsFileSystem
		{
			get
			{
				if(0!=(_shareType&ShareType.Device))
				{
					return false;
				}
				if(0!=(_shareType&ShareType.IPC))
				{
					return false;
				}
				if(0!=(_shareType&ShareType.Printer))
				{
					return false;
				}
				if(0==(_shareType&ShareType.Special))
				{
					return true;
				}
				if(ShareType.Special==_shareType&&null!=_netName&&0!=_netName.Length)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		public DirectoryInfo Root
		{
			get
			{
				if(IsFileSystem)
				{
					if(null==_server||0==_server.Length)
					{
						if(null==_path||0==_path.Length)
						{
							return new DirectoryInfo(ToString());
						}
						else
						{
							return new DirectoryInfo(_path);
						}
					}
					else
					{
						return new DirectoryInfo(ToString());
					}
				}
				else
				{
					return null;
				}
			}
		}
#endregion
		public override string ToString()
		{
			if(null==_server||0==_server.Length)
			{
				return string.Format(@"\\{0}\{1}",Environment.MachineName,_netName);
			}
			else
			{
				return string.Format(@"\\{0}\{1}",_server,_netName);
			}
		}
		public bool MatchesPath(string path)
		{
			if(!IsFileSystem)
			{
				return false;
			}
			if(null==path||0==path.Length)
			{
				return true;
			}
			return path.ToLower().StartsWith(_path.ToLower());
		}
	}
#endregion
#region Share Collection
	public class ShareCollection:
	ReadOnlyCollectionBase
	{
#region Platform
		protected static bool IsNT
		{
			get
			{
				return(PlatformID.Win32NT==Environment.OSVersion.Platform);
			}
		}
		protected static bool IsW2KUp
		{
			get
			{
				OperatingSystem os=Environment.OSVersion;
				if(PlatformID.Win32NT==os.Platform&&os.Version.Major>=5)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
#endregion
#region Interop
#region Constants
		protected const int MAX_PATH=260;
		protected const int NO_ERROR=0;
		protected const int ERROR_ACCESS_DENIED=5;
		protected const int ERROR_WRONG_LEVEL=124;
		protected const int ERROR_MORE_DATA=234;
		protected const int ERROR_NOT_CONNECTED=2250;
		protected const int UNIVERSAL_NAME_INFO_LEVEL=1;
		protected const int MAX_SI50_ENTRIES=20;
#endregion
#region Structures
		[StructLayout(LayoutKind.Sequential,CharSet=CharSet.Auto)]
		protected struct UNIVERSAL_NAME_INFO
		{
			[MarshalAs(UnmanagedType.LPTStr)]
			public string lpUniversalName;
		}
		[StructLayout(LayoutKind.Sequential,CharSet=CharSet.Unicode)]
		protected struct SHARE_INFO_2
		{
			[MarshalAs(UnmanagedType.LPWStr)]
			public string NetName;
			public ShareType ShareType;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string Remark;
			public int Permissions;
			public int MaxUsers;
			public int CurrentUsers;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string Path;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string Password;
		}
		[StructLayout(LayoutKind.Sequential,CharSet=CharSet.Unicode)]
		protected struct SHARE_INFO_1
		{
			[MarshalAs(UnmanagedType.LPWStr)]
			public string NetName;
			public ShareType ShareType;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string Remark;
		}
		[StructLayout(LayoutKind.Sequential,CharSet=CharSet.Ansi,Pack=1)]
		protected struct SHARE_INFO_50
		{
			[MarshalAs(UnmanagedType.ByValTStr,SizeConst=13)]
			public string NetName;
			public byte bShareType;
			public ushort Flags;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string Remark;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string Path;
			[MarshalAs(UnmanagedType.ByValTStr,SizeConst=9)]
			public string PasswordRW;
			[MarshalAs(UnmanagedType.ByValTStr,SizeConst=9)]
			public string PasswordRO;
			public ShareType ShareType
			{
				get
				{
					return(ShareType)((int)bShareType&0x7F);
				}
			}
		}
		[StructLayout(LayoutKind.Sequential,CharSet=CharSet.Ansi,Pack=1)]
		protected struct SHARE_INFO_1_9x
		{
			[MarshalAs(UnmanagedType.ByValTStr,SizeConst=13)]
			public string NetName;
			public byte Padding;
			public ushort bShareType;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string Remark;
			public ShareType ShareType
			{
				get
				{
					return(ShareType)((int)bShareType&0x7FFF);
				}
			}
		}
#endregion
#region Functions
		[DllImport("mpr",CharSet=CharSet.Auto)]
		protected static extern int WNetGetUniversalName(string lpLocalPath,int dwInfoLevel,ref UNIVERSAL_NAME_INFO lpBuffer,ref int lpBufferSize);
		[DllImport("mpr",CharSet=CharSet.Auto)]
		protected static extern int WNetGetUniversalName(string lpLocalPath,int dwInfoLevel,IntPtr lpBuffer,ref int lpBufferSize);
		[DllImport("netapi32",CharSet=CharSet.Unicode)]
		protected static extern int NetShareEnum(string lpServerName,int dwLevel,out IntPtr lpBuffer,int dwPrefMaxLen,out int entriesRead,out int totalEntries,ref int hResume);
		[DllImport("svrapi",CharSet=CharSet.Ansi)]
		protected static extern int NetShareEnum([MarshalAs(UnmanagedType.LPTStr)]
		string lpServerName,int dwLevel,IntPtr lpBuffer,ushort cbBuffer,out ushort entriesRead,out ushort totalEntries);
		[DllImport("netapi32")]
		protected static extern int NetApiBufferFree(IntPtr lpBuffer);
#endregion
#region Enumerate shares
		protected static void EnumerateSharesNT(string server,ShareCollection shares)
		{
			int level=2;
			int entriesRead,totalEntries,nRet,hResume=0;
			IntPtr pBuffer=IntPtr.Zero;
			try
			{
				nRet=NetShareEnum(server,level,out pBuffer,-1,out entriesRead,out totalEntries,ref hResume);
				if(ERROR_ACCESS_DENIED==nRet)
				{
					level=1;
					nRet=NetShareEnum(server,level,out pBuffer,-1,out entriesRead,out totalEntries,ref hResume);
				}
				if(NO_ERROR==nRet&&entriesRead>0)
				{
					Type t=(2==level)?typeof(SHARE_INFO_2):typeof(SHARE_INFO_1);
					int offset=Marshal.SizeOf(t);
					for(int i=0,lpItem=pBuffer.ToInt32();i<entriesRead;i++,lpItem+=offset)
					{
						IntPtr pItem=new IntPtr(lpItem);
						if(1==level)
						{
							SHARE_INFO_1 si=(SHARE_INFO_1)Marshal.PtrToStructure(pItem,t);
							shares.Add(si.NetName,string.Empty,si.ShareType,si.Remark);
						}
						else
						{
							SHARE_INFO_2 si=(SHARE_INFO_2)Marshal.PtrToStructure(pItem,t);
							shares.Add(si.NetName,si.Path,si.ShareType,si.Remark);
						}
					}
				}
			}
			finally
			{
				if(IntPtr.Zero!=pBuffer)
				{
					NetApiBufferFree(pBuffer);
				}
			}
		}
		protected static void EnumerateShares9x(string server,ShareCollection shares)
		{
			int level=50;
			int nRet=0;
			ushort entriesRead,totalEntries;
			Type t=typeof(SHARE_INFO_50);
			int size=Marshal.SizeOf(t);
			ushort cbBuffer=(ushort)(MAX_SI50_ENTRIES*size);
			IntPtr pBuffer=Marshal.AllocHGlobal(cbBuffer);
			try
			{
				nRet=NetShareEnum(server,level,pBuffer,cbBuffer,out entriesRead,out totalEntries);
				if(ERROR_WRONG_LEVEL==nRet)
				{
					level=1;
					t=typeof(SHARE_INFO_1_9x);
					size=Marshal.SizeOf(t);
					nRet=NetShareEnum(server,level,pBuffer,cbBuffer,out entriesRead,out totalEntries);
				}
				if(NO_ERROR==nRet||ERROR_MORE_DATA==nRet)
				{
					for(int i=0,lpItem=pBuffer.ToInt32();i<entriesRead;i++,lpItem+=size)
					{
						IntPtr pItem=new IntPtr(lpItem);
						if(1==level)
						{
							SHARE_INFO_1_9x si=(SHARE_INFO_1_9x)Marshal.PtrToStructure(pItem,t);
							shares.Add(si.NetName,string.Empty,si.ShareType,si.Remark);
						}
						else
						{
							SHARE_INFO_50 si=(SHARE_INFO_50)Marshal.PtrToStructure(pItem,t);
							shares.Add(si.NetName,si.Path,si.ShareType,si.Remark);
						}
					}
				}
				else
				{
					Console.WriteLine(nRet);
				}
			}
			finally
			{
				Marshal.FreeHGlobal(pBuffer);
			}
		}
		protected static void EnumerateShares(string server,ShareCollection shares)
		{
			if(null!=server&&0!=server.Length&& !IsW2KUp)
			{
				server=server.ToUpper();
				if(!('\\'==server[0]&&'\\'==server[1]))
				{
					server= @"\\" +server;
				}
			}
			if(IsNT)
			{
				EnumerateSharesNT(server,shares);
			}
			else
			{
				EnumerateShares9x(server,shares);
			}
		}
#endregion
#endregion
#region Static methods
		public static bool IsValidFilePath(string fileName)
		{
			if(null==fileName||0==fileName.Length)
			{
				return false;
			}
			char drive=char.ToUpper(fileName[0]);
			if('A'>drive||drive>'Z')
			{
				return false;
			}
			else if(Path.VolumeSeparatorChar!=fileName[1])
			{
				return false;
			}
			else if(Path.DirectorySeparatorChar!=fileName[2])
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		public static string PathToUnc(string fileName)
		{
			if(null==fileName||0==fileName.Length)
			{
				return string.Empty;
			}
			fileName=Path.GetFullPath(fileName);
			if(!IsValidFilePath(fileName))
			{
				return fileName;
			}
			int nRet=0;
			UNIVERSAL_NAME_INFO rni=new UNIVERSAL_NAME_INFO();
			int bufferSize=Marshal.SizeOf(rni);
			nRet=WNetGetUniversalName(fileName,UNIVERSAL_NAME_INFO_LEVEL,ref rni,ref bufferSize);
			if(ERROR_MORE_DATA==nRet)
			{
				IntPtr pBuffer=Marshal.AllocHGlobal(bufferSize);
				try
				{
					nRet=WNetGetUniversalName(fileName,UNIVERSAL_NAME_INFO_LEVEL,pBuffer,ref bufferSize);
					if(NO_ERROR==nRet)
					{
						rni=(UNIVERSAL_NAME_INFO)Marshal.PtrToStructure(pBuffer,typeof(UNIVERSAL_NAME_INFO));
					}
				}
				finally
				{
					Marshal.FreeHGlobal(pBuffer);
				}
			}
			switch(nRet)
			{
			case NO_ERROR:
				return rni.lpUniversalName;
			case ERROR_NOT_CONNECTED:
				ShareCollection shi=LocalShares;
				if(null!=shi)
				{
					Share share=shi[fileName];
					if(null!=share)
					{
						string path=share.Path;
						if(null!=path&&0!=path.Length)
						{
							int index=path.Length;
							if(Path.DirectorySeparatorChar!=path[path.Length-1])
							{
								index++;
							}
							if(index<fileName.Length)
							{
								fileName=fileName.Substring(index);
							}
							else
							{
								fileName=string.Empty;
							}
							fileName=Path.Combine(share.ToString(),fileName);
						}
					}
				}
				return fileName;
			default:
				Console.WriteLine("Unknown return value: {0}",nRet);
				return string.Empty;
			}
		}
		public static Share PathToShare(string fileName)
		{
			if(null==fileName||0==fileName.Length)
			{
				return null;
			}
			fileName=Path.GetFullPath(fileName);
			if(!IsValidFilePath(fileName))
			{
				return null;
			}
			ShareCollection shi=LocalShares;
			if(null==shi)
			{
				return null;
			}
			else
			{
				return shi[fileName];
			}
		}
#endregion
#region Local shares
		private static ShareCollection _local=null;
		public static ShareCollection LocalShares
		{
			get
			{
				if(null==_local)
				{
					_local=new ShareCollection();
				}
				return _local;
			}
		}
		public static ShareCollection GetShares(string server)
		{
			return new ShareCollection(server);
		}
#endregion
#region Private Data
		private string _server;
#endregion
#region Constructor
		public ShareCollection()
		{
			_server=string.Empty;
			EnumerateShares(_server,this);
		}
		public ShareCollection(string server)
		{
			_server=server;
			EnumerateShares(_server,this);
		}
#endregion
#region Add
		protected void Add(Share share)
		{
			InnerList.Add(share);
		}
		protected void Add(string netName,string path,ShareType shareType,string remark)
		{
			InnerList.Add(new Share(_server,netName,path,shareType,remark));
		}
#endregion
#region Properties
		public string Server
		{
			get
			{
				return _server;
			}
		}
		public Share this[int index]
		{
			get
			{
				return(Share)InnerList[index];
			}
		}
		public Share this[string path]
		{
			get
			{
				if(null==path||0==path.Length)
				{
					return null;
				}
				path=Path.GetFullPath(path);
				if(!IsValidFilePath(path))
				{
					return null;
				}
				Share match=null;
				for(int i=0;i<InnerList.Count;i++)
				{
					Share s=(Share)InnerList[i];
					if(s.IsFileSystem&&s.MatchesPath(path))
					{
						if(null==match)
						{
							match=s;
						}
						else if(match.Path.Length<s.Path.Length)
						{
							if(ShareType.Disk==s.ShareType||ShareType.Disk!=match.ShareType)
							{
								match=s;
							}
						}
					}
				}
				return match;
			}
		}
#endregion
#region Implementation of ICollection
		public void CopyTo(Share[]array,int index)
		{
			InnerList.CopyTo(array,index);
		}
#endregion
	}
#endregion
}
