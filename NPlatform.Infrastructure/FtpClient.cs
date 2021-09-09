#region ˵��
/*
 * ʵ����FTP�����ļ����ϴ��ļ��ȹ���
 * Version��V1.0  ���ߣ�������   ����:2007-05-08
 * 
 * 
 * 2007-08-29 
 * ���ܻ���Ӱ��ļ���
 * Encoding�ı��뷽ʽASCII����Default
 * ReadLine���߳��ӳ�
 * ��ȡ�������result��ResultCode�Ĵ���
 * ��ȡ���������Ĵ������
 */
#endregion

using System;	
using System.Net;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
//using System.Runtime.Remoting;
//using System.Runtime.Remoting.Messaging;
using System.Threading;

#region ftpӦ���뼰������
/*
//--------------------------------------------------------------------------------
110 Restart marker reply.
�����������Ӧ��������������ı���ȷ���ģ��������ǣ�MARK yyyy=mmmm������yyyy 
���û�������������ǣ�mmmm�Ƿ�������ǡ� 

120 Service ready in nnn minutes.
������nnn������׼���� 

125 Data connection already open; transfer starting.
���������Ѵ򿪣�׼������ 

150 File status okay; about to open data connection.
�ļ�״̬���ã����������� 


200 Command okay.
����ɹ� 

202 Command not implemented, superfluous(��ʣ��,�����) at this site.
����δʵ�� 

211 System status, or system help reply.
ϵͳ״̬��ϵͳ������Ӧ 

212 Directory status.
Ŀ¼״̬ 

213 File status.
�ļ�״̬ 

214 Help message.
������Ϣ����Ϣ���������û����� 

215 NAME system type.
����ϵͳ���� 

220 Service ready for new user.
�����û�����׼���� 

221 Service closing control connection.
����رտ������ӣ������˳���¼ 

225 Data connection open; no transfer in progress.
�������Ӵ򿪣��޴������ڽ��� 

226 Closing data connection.


227 Entering Passive Mode (h1,h2,h3,h4,p1,p2).
���뱻��ģʽ 

230 User logged in, proceed.
�û���¼ 

250 Requested file action okay, completed.
������ļ��������
 

������ļ�������� 
257 "PATHNAME" created.
����"PATHNAME" 

331 User name okay, need password.
�û�����ȷ����Ҫ���� 

332 Need account for login.
��¼ʱ��Ҫ�ʻ���Ϣ 

350 Requested file action pending further information.
������ļ�������Ҫ��һ������ 

421 Service not available, closing control connection.
�����ṩ���񣬹رտ������� 

425 Can't open data connection.
���ܴ��������� 

426 Connection closed; transfer aborted.
�ر����ӣ���ֹ���� 

450 Requested file action not taken.
������ļ�����δִ�� 

451 Requested action aborted: local error in processing.
��ֹ����Ĳ������б��ش��� 

452 Requested action not taken.
δִ������Ĳ�����ϵͳ�洢�ռ䲻�� 

500 Syntax error, command unrecognized.
��ʽ���������ʶ�� 

501 Syntax error in parameters or arguments.
�����﷨���� 

502 Command not implemented.
����δʵ�� 

503 Bad sequence of commands.
����˳����� 

504 Command not implemented for that parameter.
�˲����µ������δʵ�� 

530 Not logged in.
δ��¼ 

532 Need account for storing files
�洢�ļ���Ҫ�ʻ���Ϣ 

550 Requested action not taken.
δִ������Ĳ��� 

551 Requested action aborted: page type unknown.
���������ֹ��ҳ����δ֪ 

552 Requested file action aborted.
������ļ�������ֹ���洢������� 

553 Requested action not taken.
δִ������Ĳ������ļ������Ϸ�
*/
#endregion

namespace ZJJWEPlatform.Infrastructure
{
    /// <summary>
    /// ftp�ͻ���
    /// </summary>
	public class FtpClient
	{

		public class FtpException : Exception
		{
			public FtpException(string message) : base(message)
			{
			}

			public FtpException(string message, Exception innerException) : base(message,innerException)
			{
			}
		}

		private static int BUFFER_SIZE = 512;
		private static Encoding ASCII = Encoding.Default;

		private bool verboseDebugging = true; //false;

		// defaults
		private string server = "localhost";
		private string remotePath = ".";
		private string username = "anonymous";
		private string password = "anonymous@anonymous.net";
		private string message = null;
		private string result = null;
		private int m_iServerOS=32;

		private int port = 21;
		private int bytes = 0;
		private int resultCode = 0;

		private bool loggedin = false;
		private bool binMode = false;

		private Byte[] buffer = new Byte[BUFFER_SIZE];
		private Byte[] receiveBuffer = new Byte[BUFFER_SIZE*2];
		private Socket clientSocket = null;

		private int timeoutSeconds = 10;



		#region ��������
		//����������
		public string Server
		{
			get
			{
				return this.server;
			}
			set
			{
				this.server = value;
			}
		}
		
		//�û���
		public string Username
		{
			get
			{
				return this.username;
			}
			set
			{
				this.username = value;
			}
		}
		
		//����
		public string Password
		{
			get
			{
				return this.password;
			}
			set
			{
				this.password = value;
			}
		}

		//�˿ںţ�ͨ����21
		public int Port
		{
			get
			{
				return this.port;
			}
			set
			{
				this.port = value;
			}
		}
		
		//��������ӳ� 
		public int Timeout
		{
			get
			{
				return this.timeoutSeconds;
			}
			set
			{
				this.timeoutSeconds = value;
			}
		}
		#endregion

		#region ��������
		/// <summary>
		/// Display all communications to the debug log
		/// </summary>
		public bool VerboseDebugging
		{
			get
			{
				return this.verboseDebugging;
			}
			set
			{
				this.verboseDebugging = value;
			}
		}

		/// <summary>
		/// Display Server OS
		/// </summary>
		public int ServerOS 
		{
			get { return m_iServerOS;}
		}

		//��¼��Ŀ¼
		public string RemotePath
		{
			get
			{
				return this.remotePath;
			}
			set
			{
				this.remotePath = value;
			}
		}
		
		//�����ļ�ʱ�Ƿ���ö����Ʒ�ʽ
		public bool BinaryMode
		{
			get
			{
				return this.binMode;
			}
			set
			{
				if ( this.binMode == value ) return;

				if ( value )
					sendCommand("TYPE I");

				else
					sendCommand("TYPE A");

				if ( this.resultCode != 200 ) 
				{
					FireException(result);
				}
			}
		}
		#endregion

		#region ���캯��
		//		/// <summary>
		//		/// Default contructor
		//		/// </summary>
		//		public FtpClient()
		//		{
		//		}
		//		/// <summary>
		//		/// 
		//		/// </summary>
		//		/// <param name="server"></param>
		//		/// <param name="username"></param>
		//		/// <param name="password"></param>
		//		public FtpClient(string server, string username, string password)
		//		{
		//			this.server = server;
		//			this.username = username;
		//			this.password = password;
		//		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="server"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="timeoutSeconds"></param>
		/// <param name="port"></param>
		public FtpClient(string server, string username, string password, int timeoutSeconds, int port)
		{
			this.server = server;
			this.username = username;
			this.password = password;
			this.timeoutSeconds = timeoutSeconds;
			this.port = port;

			string LogFilePath = @"FtpData\FtpLog.txt";
			if(File.Exists(LogFilePath))
			{
				FileInfo LogFinInfo = new FileInfo(LogFilePath);
				long Len = LogFinInfo.Length;
				//����ļ���С����1M,����ļ�����
				if(Len>16)
				{
					LogFinInfo.Delete();
				}
			}
		}
		#endregion


//-----------------------------------------------------------------------------//
		//��¼����
		#region ��¼FTP������
		public bool Login()
		{
			if (this.loggedin)
			{
				this.Close();
			}

			IPAddress addr = null;
			IPEndPoint ep = null;
			try
			{
				this.clientSocket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
				addr = Dns.Resolve(this.server).AddressList[0];
				ep = new IPEndPoint( addr, this.port );
				//��ʼ����
				this.clientSocket.Connect(ep);
			}
			catch(Exception ex)
			{
				if ( this.clientSocket != null && this.clientSocket.Connected ) this.clientSocket.Close();
				//���ӵ�ʱ�������쳣
				FireException("Couldn't connect to remote server",ex);
				return false; 
			}
			//��ȡ���Ӻ�ķ�����
			this.readResponse();

			//����ʱ��������220��ʾ����ɹ�
			if(this.resultCode != 220)
			{
				this.Close();
				if(this.result==null)
				{
					FireException("���ܺ�Ftp������������!");
				}
				else
				{
					FireException(this.result);
				}
				return false; 
			}

			/// <summary> 
			/// Sends a user name
			/// </summary>
			/// <remarks>Reply codes: 230 530 500 501 421 331 332</remarks>
			this.sendCommand( "USER " + username );

			if( !(this.resultCode == 331 || this.resultCode == 230) )
			{
				this.cleanup();
				FireException(this.result);
				return false; 
			}

			if( this.resultCode != 230 )
			{
				/// <summary>
				/// send the user's password
				/// </summary>
				/// <remarks>Reply codes: 230 202 530 500 501 503 421 332</remarks>
				this.sendCommand( "PASS " + password );

				if( !(this.resultCode == 230 || this.resultCode == 202) )
				{
					this.cleanup();
					FireException(this.result);
					return false; 
				}
			}

			this.loggedin = true;
            return true;
		}
		#endregion
	
		#region �رպ�FTP������
		public void Close()
		{
			if( this.clientSocket != null )
			{
				/// <summary>
				/// This command terminates a USER and if file transfer is not in progress, the server closes the control connection. 
				/// </summary>
				/// <remarks>Reply codes: 221 500</remarks>
				this.sendCommand("QUIT");
			}

			this.cleanup();
		}
		#endregion
//-----------------------------------------------------------------------------//



//-----------------------------------------------------------------------------//
		//PART1���ļ���������
		//1.�����ļ�
		#region ����FTPĿ¼�µ�ĳ���ļ�
		public void Download(string remFileName)
		{
			this.Download(remFileName,"",false);
		}
		#endregion
		
		#region �����ļ������أ�ȷ���Ƿ�ϵ�����
		public void Download(string remFileName,Boolean resume)
		{
			this.Download(remFileName,"",resume);
		}
		#endregion
		
		#region �����ļ������أ������ļ�Ŀ¼������ڣ��ļ�����������򴴽�
		public void Download(string remFileName,string locFileName)
		{
			this.Download(remFileName,locFileName,false);
		}
		#endregion

		#region �����ļ������أ������ļ�Ŀ¼������ڣ��ļ�����������򴴽�.����ʹ�öϵ�����
		public void Download(string remFileName,string locFileName,Boolean resume)
		{
			if (!this.loggedin) 
			{
				this.Login();
			}
			if (locFileName.Equals(""))
			{
				locFileName = remFileName;
			}

			FileStream output = null;

            if (!File.Exists(locFileName))
                output = File.Create(locFileName);

            else
                output = new FileStream(locFileName, FileMode.Create);
        

			Socket cSocket = createDataSocket();
			//�����������Ӻ�Ϊʲô��Ҫ�Է���������ж�
			//readResponse();

			long offset = 0;

			if ( resume )
			{
				offset = output.Length;

				if ( offset > 0 )
				{

					/// <summary>
					/// The argument field represents the server marker at which file transfer is to be
					/// restarted. This command does not cause file transfer but skips over the file to
					/// the specified data checkpoint.
					/// </summary>
					/// <remarks>Reply codes: 500 501 502 421 530 350</remarks>
					this.sendCommand( "REST " + offset );
					if ( this.resultCode != 350 )
					{
						//Server dosnt support resuming
						offset = 0;
                    }
					else
					{
						output.Seek( offset, SeekOrigin.Begin );
					}
				}
			}

			/// <summary>
			/// Causes the server-DTP to transfer a copy of the file, specified 
			/// in the pathname, to the server- or user-DTP at the other end of the data 
			/// connection. The status and contents of the file at the server site shall be unaffected.
			/// </summary>
			/// <remarks>Reply codes: 125 150 110 226 250 425 426 451 450 550 500 501 421 530</remarks>
			this.sendCommand("RETR " + remFileName);

			/*	125,150,110,250,226 (success) */
			if ( this.resultCode != 150 && this.resultCode != 125 && this.resultCode != 110 && this.resultCode != 250 && this.resultCode != 226 )
			{
				output.Close();

				if ( cSocket.Connected )
				{
					cSocket.Close();
				}
				FireException(this.result);
			}

			DateTime timeout = DateTime.Now.AddSeconds(this.timeoutSeconds);

			while ( timeout > DateTime.Now )
			{
				this.bytes = cSocket.Receive(buffer, buffer.Length, 0);
				output.Write(this.buffer,0,this.bytes);

				if ( this.bytes <= 0)
				{
					break;
				}
			}

			//close the iostream and datasocket
			output.Close();
			if ( cSocket.Connected )
			{
				cSocket.Close();
			}

			this.readResponse();

			if( this.resultCode != 226 && this.resultCode != 250 && this.result!=null)
			{
				//�������ϵ�һ�δ���������ȡ��ֵ����226����250���ٶ�ȡһ�Σ���
				//FireException(this.result.Substring(4));
//				FireException(this.result);
//				showMessage(this.result,true);
				if( this.resultCode != 226 && this.resultCode != 250 && this.result!=null)
				{
					FireException(this.result);
				}

			}
		}

		#endregion


		//2.�ϴ��ļ�
		#region �ϴ��ļ�
		public void Upload(string fileName)
		{
			this.Upload(fileName,false);
		}
		#endregion
		
		#region �ϴ��ļ�������ʹ�öϵ��ϴ�
		public void Upload(string fileName, bool resume)
		{
			if ( !this.loggedin ) 
			{
				this.Login();
			}

			Socket cSocket = null ;
			long offset = 0;

			if ( resume )
			{
				try
				{
					this.BinaryMode = true;

					offset = GetFileSize( Path.GetFileName(fileName) );
				}
				catch(Exception)
				{
					// file not exist
					offset = 0;
				}
			}

			// open stream to read file
			FileStream input = new FileStream(fileName, FileMode.Open, System.IO.FileAccess.Read);

			if ( resume && input.Length < offset )
			{
				// different file size
				offset = 0;
			}
			else if ( resume && input.Length == offset )
			{
				// file done
				input.Close();
                return;
			}

			// dont create untill we know that we need it
			cSocket = this.createDataSocket();

			if ( offset > 0 )
			{
				/// <summary>
				/// The argument field represents the server marker at which file transfer is to be
				/// restarted. This command does not cause file transfer but skips over the file to
				/// the specified data checkpoint.
				/// </summary>
				/// <remarks>Reply codes: 500 501 502 421 530 350</remarks>
				this.sendCommand( "REST " + offset );
				if ( this.resultCode != 350 )
				{
					offset = 0;
				}
			}

			/// <summary>
			/// Causes the server-DTP to accept the data transferred via the data connection 
			/// and to store the data as a file at the server site. If the file specified in 
			/// the pathname exists at the server site, then its contents shall be replaced 
			/// by the data being transferred. A new file is created at the server site if 
			/// the file specified in the pathname does not already exist.
			/// </summary>
			/// <remarks>Reply codes: 125 150 110 226 250 425 426 451 551 552 532 450 452 553 500 501 421 530</remarks>
			this.sendCommand( "STOR " + Path.GetFileName(fileName) );

			if ( this.resultCode != 125 && this.resultCode != 150 ) 
			{
				input.Close();
				if(result!=null)
				{
					FireException("After Store Command:"+result.ToString());
				}
				else
				{
					FireException("After Store Command:"+resultCode.ToString());
				}
			}

			if ( offset != 0 )
			{
				input.Seek(offset,SeekOrigin.Begin);
			}
			while ((bytes = input.Read(buffer,0,buffer.Length)) > 0)
			{
				cSocket.Send(buffer, bytes, 0);
			}
			
			input.Close();

			if (cSocket.Connected)
			{
				cSocket.Close();
			}

			this.readResponse();

			
			//2007-08-17 ���԰칫�ҵķ��������ַ�������150 
			if( this.resultCode != 226 && this.resultCode != 250 && this.resultCode != 150) 
			{
				if(result!=null)
				{
					FireException("After Data Uploaded:"+result.ToString());
				}
				else
				{
					FireException("After Data Uploaded:"+resultCode.ToString());
				}
			}
		}
		#endregion
		
		#region �ϴ�ĳ��Ŀ¼�µ��ļ�
		/// <summary>
		/// Upload a directory and its file contents
		/// </summary>
		/// <param name="path"></param>
		/// <param name="recurse">Whether to recurse sub directories</param>
		public void UploadDirectory(string path, bool recurse)
		{
			this.UploadDirectory(path,recurse,"*.*");
		}
		#endregion
		
		#region �ϴ�ĳ��Ŀ¼�µ��ļ�
		/// <summary>
		/// Upload a directory and its file contents
		/// </summary>
		/// <param name="path"></param>
		/// <param name="recurse">Whether to recurse sub directories</param>
		/// <param name="mask">Only upload files of the given mask - everything is '*.*'</param>
		public void UploadDirectory(string path, bool recurse, string mask)
		{
			string[] dirs = path.Replace("/",@"\").Split('\\');
			string rootDir = dirs[ dirs.Length - 1 ];

			// make the root dir if it doed not exist
			try
			{
				// try to retrieve files
				this.GetFileList(rootDir);
			}
			catch
			{
				// if receive an error
				this.MakeDir(rootDir);
			}

//			if ( this.GetFileList(rootDir).Length < 1 ) this.MakeDir(rootDir);

			this.ChangeDir(rootDir);

			foreach ( string file in Directory.GetFiles(path,mask) )
			{
				this.Upload(file,true);
			}
			if ( recurse )
			{
				foreach ( string directory in Directory.GetDirectories(path) )
				{
					this.UploadDirectory(directory,recurse,mask);
				}
			}

			this.ChangeDir("..");
		}
		#endregion

#region ���²���δʹ��
		//3.ɾ���ļ�
		#region ɾ��ĳ���ļ�
		/// <summary>
		/// Delete a file from the remote FTP server.
		/// </summary>
		/// <param name="fileName"></param>
		public void DeleteFile(string fileName)
		{
			if ( !this.loggedin ) 
			{
				this.Login();
			}

			/// <summary>
			/// Causes the file specified in the pathname to be deleted at the server site
			/// </summary>
			/// <remarks>Reply codes: 250 450 550 500 501 502 421 530</remarks>
			this.sendCommand( "DELE " + fileName );

			if ( this.resultCode != 250 )
			{
//				FireException(this.result.Substring(4));
				FireException(this.result);
			}
		}
		#endregion

		//4.�޸��ļ���
		#region ������FTP�ϵ�ĳ���ļ�
		/// <summary>
		/// Rename a file on the remote FTP server.
		/// </summary>
		/// <param name="oldFileName"></param>
		/// <param name="newFileName"></param>
		/// <param name="overwrite">setting to false will throw exception if it exists</param>
		public void RenameFile(string oldFileName,string newFileName, bool overwrite)
		{
			if ( !this.loggedin )
			{
				this.Login();
			}

			/// <summary>
			/// Specifies the old pathname of the file which is to be renamed. This 
			/// command must be immediately followed by a "rename to" command specifying the new 
			/// file pathname.
			/// </summary>
			/// <remarks>Reply codes: 450 550 500 501 502 421 530 350</remarks>
			this.sendCommand( "RNFR " + oldFileName );

			if ( this.resultCode != 350 ) 
			{
//				FireException(this.result.Substring(4));
				FireException(this.result);
			}

//			if ( !overwrite && this.GetFileList(newFileName).Length > 0 ) 
//				FireException("File already exists");

			// verify if newFileName exists
			if ( !overwrite)
			{
				this.sendCommand("SIZE " + newFileName);
				if ( this.resultCode == 213 )
					FireException("File already exists");
			}

			/// <summary>
			/// Specifies the new pathname of the file specified in the immediately 
			/// preceding "rename from" command. Together the two commands cause a file to be renamed.
			/// </summary>
			/// <remarks>Reply codes: 250 532 553 500 501 502 503 421 530</remarks>
			this.sendCommand( "RNTO " + newFileName );

			if ( this.resultCode != 250 ) 
			{
//				FireException(this.result.Substring(4));
				FireException(this.result);
			}
        }
		#endregion
//-----------------------------------------------------------------------------//





//-----------------------------------------------------------------------------//
		//PARTII:�ļ��в���

		#region ��FTP�ϴ���һ���ļ���
		/// <summary>
		/// Create a directory on the remote FTP server.
		/// </summary>
		/// <param name="dirName"></param>
		public void MakeDir(string dirName)
		{
			if (!this.loggedin ) 
			{
				this.Login();
			}

			/// <summary>
			/// Causes the directory specified in the pathname to be created as 
			/// a directory (if the pathname is absolute) or as a subdirectory of the current 
			/// working directory (if the pathname is relative).
			/// </summary>
			/// <remarks>Reply codes: 257 500 501 502 421 530 550</remarks>
			this.sendCommand( "MKD " + dirName );

			if ( this.resultCode != 250 && this.resultCode != 257 ) 
			{
//				FireException(this.result.Substring(4));
				FireException(this.result);
			}
		}
		#endregion

		#region ɾ��FTP�ϵ�һ���ļ���
		/// <summary>
		/// Delete a directory on the remote FTP server.
		/// </summary>
		/// <param name="dirName"></param>
		public void RemoveDir(string dirName)
		{
			if ( !this.loggedin ) 
			{
				this.Login();
			}

			/// <summary>
			/// Causes the directory specified in the pathname to be removed as 
			/// a directory (if the pathname is absolute) or as a subdirectory of the current 
			/// working directory (if the pathname is relative).
			/// </summary>
			/// <remarks>Reply codes: 250 500 501 502 421 530 550</remarks>
			this.sendCommand( "RMD " + dirName );

			if ( this.resultCode != 250 )
			{
//				FireException(this.result.Substring(4));
				FireException(this.result);
			}
		}
		#endregion

		#region �ı�ͻ���Ŀǰ�������FTP�ϵ�Ŀ¼���ļ��в�Σ�
		/// <summary>
		/// Change the current working directory on the remote FTP server.
		/// </summary>
		/// <param name="dirName"></param>
		public bool ChangeDir(string dirName)
		{
			if( dirName == null || dirName.Equals(".") || dirName.Length == 0 )
			{
				return false;
			}

			if ( !this.loggedin )
			{
				this.Login();
			}

			/// <summary>
			/// Changes the current directory
			/// </summary>
			/// <remarks>Reply codes: 250 500 501 502 421 530 550</remarks>
			this.sendCommand( "CWD " + dirName );

			//if ( ( this.resultCode != 250 ) && ( this.resultCode != 150 ) )

			if ( (this.resultCode == 500) || (this.resultCode == 501) || (this.resultCode == 502) || (this.resultCode == 421) || (this.resultCode == 530) || (this.resultCode == 550) )
			{
//				FireException(result.Substring(4));
				FireException(this.result);
				return false;
			}

			/// <summary>
			/// Causes the name of the current working directory to be returned in the reply.
			/// </summary>
			/// <remarks>Reply codes: 257 500 501 502 421 550</remarks>
			this.sendCommand( "PWD" );

			if ( (this.resultCode == 500) || (this.resultCode == 501) || (this.resultCode == 502) || (this.resultCode == 421) || (this.resultCode == 550) )
			{
//				FireException(result.Substring(4));
				FireException(this.result);
				return false;
			}

			//��ʱ��message������","
			return true;

			// gonna have to do better than this....
//			this.remotePath = this.message.Split('"')[1];
//
//			showMessage( "Current directory is " + this.remotePath, false );
//			return this.remotePath;
		}
		#endregion
//-----------------------------------------------------------------------------//





//-----------------------------------------------------------------------------//	
		//PARTIII:�鿴��Ϣ
		//1.�鿴�ļ���
		#region ��ȡ��ǰ��¼Ŀ¼�µ������ļ��б�
		public string[] GetFileList()
		{
			return this.GetFileList("*.*");
		}
		#endregion

		#region �г�FTP��Ŀ¼�µ������ļ��б�
		public string[] GetFileList(string mask)
		{
			if ( !this.loggedin )
			{
				this.Login();
			}

			Socket cSocket = createDataSocket();

			/// <summary>
			/// Causes a list to be sent from the server to the passive DTP. If 
			/// the pathname specifies a directory or other group of files, the server should 
			/// transfer a list of files in the specified directory. If the pathname specifies 
			/// a file then the server should send current information on the file. A null 
			/// argument implies the user's current working or default directory.
			/// </summary>
			/// <remarks>Reply codes: 125 150 226 250 425 426 451 450 500 501 502 421 530</remarks>
			this.sendCommand("LIST " + mask);

			//150 �ļ�״̬���ã�����������   125  ���������Ѵ򿪣�׼������ 
			//250 ������ļ��������           226  �ر��������ӣ�������ļ������ɹ�
			if(!(this.resultCode == 150 || this.resultCode == 125 || this.resultCode == 250 || this.resultCode == 226)) 
			{
//				FireException(this.result.Substring(4));
				FireException(this.result);
			}

			this.message = "";

			string l_strOutput = "",l_strTemp = "";
			int l_iRetval = 0;

			receiveBuffer.Initialize();
			l_strTemp = "";
			l_strOutput = "";

			Thread.Sleep(700);

			for ( ; ( l_iRetval = cSocket.Receive(receiveBuffer)) > 0 ;  ) 
			{
				l_strTemp = Encoding.Default.GetString(receiveBuffer,0,l_iRetval);
				l_strOutput += l_strTemp;
				if ( cSocket.Available == 0 )
				{
					break;
				}
			}

			this.message = l_strOutput;
			string[] msg = this.message.Replace("\r","").Split('\n');

			cSocket.Close();

			if ( this.message.IndexOf( "No such file or directory" ) != -1 )
				msg = new string[]{};

			//			this.readResponse();
			//
			//			if ( this.resultCode != 226 )
			//				msg = new string[]{};
			//			//	FireException(result.Substring(4));

			return msg;
		}
		#endregion	
		

		//2.�鿴�ļ�
		#region ��ȡFTPĿ¼�µ��ļ���С
		public long GetFileSize(string fileName)
		{
			if ( !this.loggedin ) 
			{
				this.Login();
			}

			this.sendCommand("SIZE " + fileName);
			long size=0;

			if ( this.resultCode == 213 )
				size = long.Parse(this.result.Substring(4));

			else
				FireException(this.result.Substring(4));

			return size;
		}
		#endregion
		
		#region ��ȡ�ļ����޸�����
		public string GetModDate(string FileName)
		{
			if (!this.loggedin )
			{
				this.Login();
			}

			/// <summary>
			/// Changes the current directory
			/// </summary>
			/// <remarks>Reply codes: 250 500 501 502 421 530 550</remarks>
			this.sendCommand( "MDTM " + FileName );
			if(resultCode!=213)
			{
				return null;
			}
			string szTime = message;
			szTime = message.Remove(0,4);
			szTime = szTime.Substring(0,14);
			return szTime;
		}
		#endregion
//-----------------------------------------------------------------------------//
#endregion			


#region �����������ftp����
        public bool QuoteFileType(string szFileType)
        {
            if (!this.loggedin)
            {
                this.Login();
            }

            this.sendCommand("site filetype=" + szFileType);

            if (this.resultCode == 200)
                return true;

            else
                FireException(this.result.Substring(4));

            return false;
        }
#endregion
//-----------------------------------------------------------------------------//
		/// PARTVI������FTP����
		//����FTP�����Լ���ȡ������Ϣ
		#region  ��ȡ��FTP����������FTP�����ķ�����
		private void readResponse()
		{
			this.message = "";
			this.result = this.readLine();

			if ( this.result.Length > 3 )
				this.resultCode = int.Parse( this.result.Substring(0,3) );
			else
			{
				this.result = null;
				resultCode = 0;
			}
		}
		#endregion

		#region ��ȡ��FTP����������FTP�����ķ�����Ϣ
		private string readLine()
		{
			string l_strOutput = "",l_strTemp = "";
			int l_iRetval = 0;

			receiveBuffer.Initialize();
			l_strTemp = "";
			l_strOutput = "";

			Thread.Sleep(600);

			/* Receive data in a loop until the FTP server sends it */
			//�ɰ汾
//			for ( ; ( l_iRetval = clientSocket.Receive(receiveBuffer)) > 0 ;  ) 
//			{
//				l_strTemp = Encoding.Default.GetString(receiveBuffer,0,l_iRetval);
//				l_strOutput += l_strTemp;
//				if ( clientSocket.Available == 0 )
//				{
//					break;
//				}
//			}

			//�°汾
//			while(clientSocket.Available>0 && ( l_iRetval = clientSocket.Receive(receiveBuffer)) > 0 ) 
//			{
//				l_strTemp = Encoding.Default.GetString(receiveBuffer,0,l_iRetval);
//				l_strOutput += l_strTemp;
//			}
			//�°汾2	2007-08-29
			while(true)
			{
				l_iRetval = clientSocket.Receive(receiveBuffer, receiveBuffer.Length, 0);
				l_strTemp += Encoding.ASCII.GetString(receiveBuffer, 0, l_iRetval);
				l_strOutput += l_strTemp;
				if(l_iRetval < receiveBuffer.Length)
				{
					break;
				}
			}

			this.message = l_strOutput;

			string[] msg = this.message.Split('\n');

			if ( this.message.Length > 2 )
			{
				this.message = msg[ msg.Length - 2 ];
			}
			else
			{
				this.message = msg[0];
			}

			//���û�н��ܵ������룬�ӳ�700���룬���½��ж�ȡ�����롣������һ����ڷ�����Ϣ��ͷ����
			if ( this.message.Length >= 4 && !this.message.Substring(3,1).Equals(" ") ) 
			{
				return this.readLine();
			}

			return message;
		}
		#endregion

		#region ��FTP��������������
		private void sendCommand(string command)
		{
			int l_iRetval = 0;
			try
			{
				command = command.Trim()+ "\r\n";
              
				Byte[] cmdBytes = Encoding.ASCII.GetBytes(command .ToCharArray() );

				//������Ϣ
				l_iRetval = clientSocket.Send( cmdBytes, cmdBytes.Length, 0);
				this.readResponse();
			}
			catch
			{
                throw;
			}
		}
		#endregion
//-----------------------------------------------------------------------------//



//-----------------------------------------------------------------------------//
		//PARTV:�������ݴ����Socket
		#region Ϊ�˴������ݶ��������ٵ�Socketͨ��
		/// <summary>
		/// when doing data transfers, we need to open another socket for it.
		/// </summary>
		/// <returns>Connected socket</returns>
		private Socket createDataSocket()
		{
			/// <summary>
			/// Requests the server-DTP to "listen" on a data port (which is not
			/// its default data port) and to wait for a connection rather than initiate one
			/// upon receipt of a transfer command. The response to this command includes the 
			/// host and port address this server is listening on. 
			/// </summary>
			/// <remarks>Reply codes: 227 500 501 502 421 530</remarks>
			this.sendCommand("PASV");

			//227 ���뱻��ģʽ
			if ( this.resultCode != 227 ) 
			{
//				FireException(this.result.Substring(4));
				FireException(this.result);
			}

			int index1 = this.result.IndexOf('(');
			int index2 = this.result.IndexOf(')');

			string ipData = this.result.Substring(index1+1,index2-index1-1);

			int[] parts = new int[6];

			int len = ipData.Length;
			int partCount = 0;
			string buf="";

			for (int i = 0; i < len && partCount <= 6; i++)
			{
				char ch = char.Parse( ipData.Substring(i,1) );

				//IsDigit�����ж�һ�������Ƿ�Ϊ�ַ�
				if ( char.IsDigit(ch) )
					buf+=ch;

				else if (ch != ',')
					FireException("Malformed PASV result: " + result);

				if ( ch == ',' || i+1 == len )
				{
					try
					{
						parts[partCount++] = int.Parse(buf);
						buf = "";
					}
					catch (Exception ex)
					{
						FireException("Malformed PASV result (not supported?): " + this.result, ex);
					}
				}
			}

			string ipAddress = parts[0] + "."+ parts[1]+ "." + parts[2] + "." + parts[3];

			int port = (parts[4] << 8) + parts[5];

			Socket socket = null;
			IPEndPoint ep = null;

			try
			{
				socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
				ep = new IPEndPoint(Dns.Resolve(ipAddress).AddressList[0], port);
				socket.Connect(ep);
			}
			catch(Exception ex)
			{
				// doubtfull....
				if ( socket != null && socket.Connected ) socket.Close();

				FireException("Can't connect to remote server", ex);
			}

			return socket;
		}
		#endregion
//-----------------------------------------------------------------------------//
		



		
//-----------------------------------------------------------------------------//
		//PATRTVI:�ͷ���Դ����
		#region ��Դ����
		/// <summary>
		/// Always release those sockets.
		/// </summary>
		private void cleanup()
		{
			if ( this.clientSocket!=null )
			{
				this.clientSocket.Close();
				this.clientSocket = null;
			}
			this.loggedin = false;
		}
		#endregion

		#region ��������
		/// <summary>
		/// Destuctor
		/// </summary>
		~FtpClient()
		{
			this.cleanup();
		}
		#endregion
//-----------------------------------------------------------------------------//





//-----------------------------------------------------------------------------//
		//PARTVII:�ǳ�����·����쳣
		#region �׳��쳣
		private void FireException(string message,Exception innerException)
		{
			throw new FtpException(message, innerException);
		}

		private void FireException(string message)
		{
			throw new FtpException(message);
		}
		#endregion
	}
}