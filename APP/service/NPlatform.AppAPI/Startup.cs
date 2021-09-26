using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NPlatform.ComValue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NPlatform.AppAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            string serviceName = Configuration.GetValue<string>("ServiceConfig:ServiceName");
            services.AddHealthChecks().AddCheck<MyHealthChecks>(serviceName); ;

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "NPlatform.AppAPI", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Microsoft.Extensions.Hosting.IHostApplicationLifetime aft)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NPlatform.AppAPI v1"));
            }
            aft.ApplicationStarted.Register(() =>

            {

                Console.WriteLine("Started");
                RegisterConsul();
            });

            aft.ApplicationStopped.Register(() =>

            {
                DeregisterConsul();
                Console.WriteLine("ApplicationStopped");

            });
            app.UseHttpsRedirection();
            app.UseHealthChecks("/healthChecks");
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public static string RegistrationID = "";

        public void DeregisterConsul()
        {

            //����ע��� Consul ��ַ ConsulClient ��ַ//��������ip ���Ǳ�����ip������˿�8500 �����Ĭ��ע�����˿� 
            ConsulClient consulClient = new ConsulClient(p => { p.Address = new Uri(Configuration.GetValue<string>("ConsulClient")); });
            consulClient.Agent.ServiceDeregister(RegistrationID);
        }
        public void RegisterConsul()
        {
            //����ע��� Consul ��ַ ConsulClient ��ַ//��������ip ���Ǳ�����ip������˿�8500 �����Ĭ��ע�����˿� 

            ConsulClient consulClient = new ConsulClient(p => { p.Address = new Uri(Configuration.GetValue<string>("ConsulClient")); });

            var serviceConfig = Configuration.GetSection("ServiceConfig");
            ServiceConfig svcConfig = new ServiceConfig();
            svcConfig.ListenIP = serviceConfig.GetValue<string>("ListenIP");
            svcConfig.Port = serviceConfig.GetValue<int>("Port");
            svcConfig.ServiceName = serviceConfig.GetValue<string>("ServiceName");
            svcConfig.MachineID = serviceConfig.GetValue<string>("MachineID");
            svcConfig.ServiceID = serviceConfig.GetValue<string>("ServiceID");

            var httpCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),//����������ú�ע��
                Interval = TimeSpan.FromSeconds(10),//����̶���ʱ�����һ�Σ�https://localhost:44308/api/Health
                HTTP = $"{svcConfig.ListenIP}:{svcConfig.Port}/healthChecks",//��������ַ 44308��visualstudio�����Ķ˿�
                Timeout = TimeSpan.FromSeconds(5)
            };

            var registration = new AgentServiceRegistration()
            {
                Checks = new[] { httpCheck },
                ID = svcConfig.MachineID + ":" + svcConfig.ServiceID,
                Name = svcConfig.ServiceName,
                Address = svcConfig.ListenIP,
                Port = svcConfig.Port,

            };
            RegistrationID = registration.ID;
            consulClient.Agent.ServiceRegister(registration).Wait();//ע����� 

            //consulClient.Agent.ServiceDeregister(registration.ID).Wait();//registration.ID��guid
            //������ֹͣʱ��Ҫȡ������ע�ᣬ��Ȼ���´���������ʱ������ע��һ������
            //���ǣ�����÷����ڲ���������consul���Զ�ɾ��������񣬴�Լ2��3���Ӿͻ�ɾ�� 
        }
    }
}
