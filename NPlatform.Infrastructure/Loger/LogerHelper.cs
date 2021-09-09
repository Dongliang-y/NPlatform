/***********************************************************
**��Ŀ����:	                                                                  				   
**��������:	  ��ժҪ˵��
**��    ��: 	�׶���                                         			   
**�� �� ��:	1.0                                             			   
**�������ڣ� 2015/12/29 16:15:32
**�޸���ʷ��
************************************************************/

namespace NPlatform.Infrastructure.Loger
{
    using System;
    using System.IO;
    using System.Text;
    using log4net;
    using log4net.Config;
    using log4net.Repository;

    /// <summary>
    /// ��־��¼��
    /// ʹ��ǰ�ȼ������� ��
    /// </summary>
   [Obsolete("LogerHelper�ѹ��ڣ�������ʹ�ø������е�Loger���������־��¼��", false)]
    public static class LogerHelper
    {
        /// <summary>
        /// loger reposityory
        /// </summary>
        public static ILoggerRepository LogRepository { get; } = LogManager.CreateRepository("NETCoreRepository");

        /// <summary>
        /// The loger.
        /// </summary>
        public static ILog Log{ get; set; }

        /// <summary>
        /// Initializes static members of the <see cref="LogerHelper"/> class.
        /// ����
        /// </summary>
        static LogerHelper()
        {
            // ����log4net
            log4net.Config.XmlConfigurator.Configure(LogRepository, new System.IO.FileInfo(System.IO.Path.Combine( System.IO.Directory.GetCurrentDirectory(),"Config\\log4net.config")));

            // ����logʵ��
           Log = LogManager.GetLogger(LogRepository.Name, AppDomain.CurrentDomain.FriendlyName);
            var errors = Log.Logger.Repository.ConfigurationMessages;
            if(errors.Count>0)
            {
                StringBuilder str = new StringBuilder();
                foreach(var error in errors)
                {
                    str.AppendLine(error.ToString());
                }
                throw new Exception(str.ToString());
            }
        }

        /// <summary>
        /// ��¼
        /// </summary>
        /// <param name="msg">
        /// The msg.
        /// </param>
        /// <param name="logType">
        /// The log Type.
        /// </param>
        public static void Info(string msg, string logType = "")
        {
            Log.Info(new BusinessLog("", "system", msg,logType,""));
        }

        /// <summary>
        /// ��¼ҵ����־
        /// </summary>
        /// <param name="log">ҵ�����ݶ���</param>
        [Obsolete("BusinessLog ��ͣ�ã�������ʹ�ø������е�Loger���������־��¼��", true)]
        public static void BusinessLog(BusinessLog log)
        {
            Log.Info(log);
        }

        /// <summary>
        /// ������־
        /// </summary>
        /// <param name="msg">��Ϣ</param>
        /// <param name="logType">��Ϣ����</param>
        public static void Debug(string msg, string logType = "")
        {
            Log.Debug(new BusinessLog("", "system", msg, logType, ""));
        }

        /// <summary>
        /// �쳣��־
        /// </summary>
        /// <param name="msg">��Ϣ</param>
        /// <param name="logType">��Ϣ����</param>
        public static void Error(string msg, string logType)
        {
            Log.Error(new BusinessLog("", "system", msg, logType, ""));
        }

        /// <summary>
        /// �쳣��־
        /// </summary>
        /// <param name="ex">�쳣</param>
        /// <param name="logType">��Ϣ����</param>
        public static void Error(Exception ex, string logType="error")
        {
            Log.Error(new BusinessLog("", "system", ex.Message + ex.ToString(), logType, ""), ex);
        }

        /// <summary>
        /// �쳣��־
        /// </summary>
        /// <param name="msg">��Ϣ</param>
        /// <param name="ex">�쳣</param>
        /// <param name="logType">��Ϣ����</param>
        public static void Error(string msg, string logType, Exception ex)
        {
            Log.Error(new BusinessLog("", "system", msg+ex.ToString(), logType, ""),ex);
        }

        /// <summary>
        /// �쳣��־
        /// </summary>
        /// <param name="ex">�쳣</param>
        /// <param name="log">ҵ����Ϣ</param>
        public static void Error(BusinessLog log, Exception ex)
        {
            Log.Error(log, ex);
        }

        /// <summary>
        /// �쳣��־
        /// </summary>
        /// <param name="ex">�쳣</param>
        /// <param name="msg">��Ϣ</param>
        /// <param name="module">ģ��</param>
        public static void Fatal(string msg, string module, Exception ex)
        {
            Log.Fatal(new BusinessLog("", module, msg, "Fatal", "system"), ex);
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="msg">��Ϣ</param>
        /// <param name="logType">��Ϣ����</param> 
        public static void Warn(string msg, string logType)
        {
            Log.Warn(new BusinessLog("", "", msg, logType, ""));
        }
        /// <summary>
        /// ����
        /// </summary>
        /// <param name="log">ҵ����Ϣ</param>
        public static void Warn(BusinessLog log)
        {
            Log.Warn(log);
        }
    }
}