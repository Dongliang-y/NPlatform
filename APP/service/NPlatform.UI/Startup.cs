using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NPlatform.ComValue;
using NPlatform.UI.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NPlatform.UI
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
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Microsoft.Extensions.Hosting.IHostApplicationLifetime aft)
        {
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

            aft.ApplicationStopping.Register(() =>

            {

                Console.WriteLine("ApplicationStopping");

            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHealthChecks("/healthChecks");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCnblogsHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
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
                ID = svcConfig.MachineID+":"+svcConfig.ServiceID,
                Name = svcConfig.ServiceName,
                Address = svcConfig.ListenIP ,
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
