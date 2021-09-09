/***********************************************************
**��Ŀ����:	                                                                  				   
**��������:	  ��ժҪ˵��
**��    ��: 	�׶���                                         			   
**�� �� ��:	1.0                                             			   
**�������ڣ� 2015/12/29 16:15:32
**�޸���ʷ��
************************************************************/

namespace NPlatform.Domains.Service
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using log4net;
    using log4net.Config;
    using log4net.Repository;
    using ServiceStack;
    using NPlatform.Config;
    using NPlatform.Domains.Entity;
    using NPlatform.Domains.IRepositories;
    using NPlatform.Domains.Service;
    using NPlatform.Infrastructure;
    using NPlatform.Infrastructure.Loger;
    using NPlatform.Result;

    /// <summary>
    /// ��־��¼��
    /// ʹ��ǰ�ȼ������� ��
    /// </summary>
    /// <summary> 
    ///    Loger  �ִ�����
    /// </summary> 
    public partial class LogerService : ILogerService
    {
        /// <summary>
        /// loger reposityory
        /// </summary>
     //   public ILoggerRepository Log4netRepository { get; } = LogerHelper.LogRepository;

        /// <summary>
        /// ��־�����ֵ�
        /// </summary>
        public Dictionary<int, string> LogLeves
        {
            get {
                return EnumExtend.GetDictionary<LogLevel>();
            }
        }
        /// <summary>
        /// The loger.
        /// </summary>
        private ILog Log { get; set; }

        private IPlatformHttpContext platformHttpContext;
        /// <summary>
        /// ƽ̨����
        /// </summary>
        private static NPlatformConfig platConfig = new ConfigFactory<NPlatformConfig>().Build();
        /// <summary>
        /// Initializes static members of the <see cref="LogerHelper"/> class.
        /// ����
        /// </summary>
        public LogerService(IPlatformHttpContext httpContext)
        {
            Log = LogerHelper.Log;
            //// ����log4net
            //log4net.Config.XmlConfigurator.Configure(Log4netRepository, new System.IO.FileInfo(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Config\\log4net.config")));

            //// ����logʵ��
            //Log = LogManager.GetLogger(Log4netRepository.Name, AppDomain.CurrentDomain.FriendlyName);
            //var errors = Log.Logger.Repository.ConfigurationMessages;
            //if (errors.Count > 0)
            //{
            //    StringBuilder str = new StringBuilder();
            //    foreach (var error in errors)
            //    {
            //        str.AppendLine(error.ToString());
            //    }
            //    throw new Exception(str.ToString());
            //}
            platformHttpContext = httpContext;

        }
        private BusinessLog SetLog(BusinessLog log)
        {

            var save = platConfig.PlatformConfigs.ContainsKey("EnableDetail") ? platConfig.PlatformConfigs["EnableDetail"] : string.Empty;
            if (!save.IsNullOrEmpty() && !bool.Parse(save))
            {
                return log;
            }
            if (log == null) return log;
            if (platformHttpContext == null || platformHttpContext.Claims==null)
            {
                log.Creator = "δ��ȡ����¼��Ϣ";
                return log;
            }
            var claims = platformHttpContext.Claims;
            AuthInfoVO authInfo = new AuthInfoVO();
            if (claims.Any(t => t.Type.Contains("givenname")))
            {
                // Claims.FirstOrDefault(t => t.Subject.Name);
                authInfo.CnName = claims.FirstOrDefault(t => t.Type.Contains("givenname")).Value;
            }
            if (claims.Any(t => t.Type == "name"))
            {
                authInfo.Account = claims.FirstOrDefault(t => t.Type == "name").Value;
            }

            if (claims.Any(t => t.Type == "client_id"))
            {
                authInfo.ClientId = claims.FirstOrDefault(t => t.Type == "client_id").Value;
            }
            var path = $"{platformHttpContext.Context.Request.Method} {platformHttpContext.Context.Request.Path.Value} query: {platformHttpContext.Context.Request.QueryString}";

            log.Message = $"ClientID:{authInfo.ClientId}, {System.Environment.NewLine} Message:{ log.Message}";
            log.Creator = $"{authInfo.Account}:{authInfo.CnName}";
            log.LogLocation = path;
            log.LogBrowser = platformHttpContext.Context.Request.Headers["User-Agent"];
            return log;
        }

        /// <summary>
        /// ��¼ҵ����־
        /// </summary>
        public void BLLSaveLog<T>(T entity, string message = "", LogType logType = LogType.Business) where T : IEntity
        {
            var save = platConfig.PlatformConfigs.ContainsKey("EnableBLLSaveLog") ? platConfig.PlatformConfigs["EnableBLLSaveLog"] : string.Empty;
            if (!save.IsNullOrEmpty()&&!bool.Parse(save))
            {
                return;
            }
            Type type = typeof(T);
            var val = type.CustomAttributes.FirstOrDefault(t => t.AttributeType == typeof(Domains.TableName));
            string moduleName = "";
            if (val != null)
                moduleName = val.NamedArguments.First().TypedValue.Value.ToString();
            else
                moduleName = type.Name;

            var business = new BusinessLog(entity.GetID(), moduleName.Split('_')[0], $"{moduleName}����{message}", logType.ToString(), "");
            Log.Info(SetLog(business));
        }
        /// <summary>
        /// ��¼ҵ����־
        /// </summary>
        public void BLLDeleteLog<T>(string Id, string message = "", LogType logType = LogType.Business) where T :IEntity
        {
            var save = platConfig.PlatformConfigs.ContainsKey("EnableBLLDeleteLog") ? platConfig.PlatformConfigs["EnableBLLDeleteLog"] : string.Empty;
            if (!save.IsNullOrEmpty() && !bool.Parse(save))
            {
                return;
            }
            Type type = typeof(T);
            var val = type.CustomAttributes.FirstOrDefault(t => t.AttributeType == typeof(Domains.TableName));
            string moduleName = "";
            if (val != null)
                moduleName = val.NamedArguments.First().TypedValue.Value.ToString();
            else
                moduleName = type.Name;

            var business = new BusinessLog(Id, moduleName.Split('_')[0], $"ɾ����{moduleName}���{Id}����,{message}", logType.ToString(), "");
            Log.Info(SetLog(business));
        }

        /// <summary>
        /// ��¼
        /// </summary>
        /// <param name="msg">
        /// The msg.
        /// </param>
        /// <param name="module">
        /// The log module.
        /// </param>
        public void Info(string msg, string module = "system")
        {
            var log = SetLog(new BusinessLog("", module, msg, LogType.Trace.ToString(), ""));
            Log.Info(log);
        }


        /// <summary>
        /// ������־
        /// </summary>
        /// <param name="msg">��Ϣ</param>
        /// <param name="module">ҵ��ģ��</param>
        public void Debug(string msg, string module = "system")
        {

            Log.Debug(SetLog(new BusinessLog("", module, msg, LogType.Trace.ToString(), "")));
        }
        /// <summary>
        /// �쳣��־
        /// </summary>
        /// <param name="msg">��Ϣ</param>
        /// <param name="ex">�쳣</param>
        /// <param name="module">ģ��</param>
        public void Error(string msg, Exception ex, string module = "system")
        {
            Log.Error(SetLog(new BusinessLog("", module, msg, LogType.Trace.ToString(), "")), ex);
        }

        /// <summary>
        /// �쳣��־
        /// </summary>
        /// <param name="ex">�쳣</param>
        /// <param name="msg">��Ϣ</param>
        /// <param name="module">ģ��</param>
        public void Fatal(string msg, Exception ex, string module = "system")
        {
            Log.Fatal(SetLog(new BusinessLog("", module, msg, LogType.Trace.ToString(), "")), ex);
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="msg">��Ϣ</param>
        /// <param name="module">ģ��</param> 
        public void Warn(string msg, string module = "system")
        {
            Log.Warn(SetLog(new BusinessLog("", module, msg, LogType.Trace.ToString(), "")));
        }

        /// <summary>
        /// ��ѯ��־�б�
        /// </summary>
        /// <param name="key">��ѯ�ؼ���</param>
        /// <param name="moduleName">ģ����</param>
        /// <param name="logLeve">��־����</param>
        /// <param name="account">�˺�</param>
        /// <param name="beginDate">��ʼʱ��</param>
        /// <param name="endDate">����ʱ��</param>
        /// <param name="page">ҳ��</param>
        /// <param name="pageSize">ҳ��С</param>
        /// <returns></returns>
        public virtual IListResult<Loger> GetLogers(string key, string moduleName, string logLeve, string account, DateTime? beginDate, DateTime? endDate, int page, int pageSize)
        {
            throw new NotImplementedException("����δʵ�ִ˷���");
        }
    }
}