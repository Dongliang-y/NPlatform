
### ��Ʒ���ƣ���˾�������
### ��˾����ɳ�н���γ
### ���ߣ�Dongliang Yi
### ����˵����
API/�������嵥


### ���¼�¼��
* 2021-2-3 V 5.0.1-alpha  ��3.0�汾�� ���Ӷ� .NET5 ,.NET 2.2��֧��,ʵ�� 2.2,3.1 5.0��Ŀ����֧��.
* 2020-11-4��V3.0.0.16  http client �����������ļ��ϴ�
* V3.0.0.14  ������������л��������� bool descriptionAsHeard = false ���������ã����Ƿ�ʹ����������Ϊ���л���ı�������Դ��ͷ����  ֵΪ true�����Զ�ȡ�����л����������ϵ� [System.ComponentModel.Description]������ָ��������������Ϊ��ͷ��
ReportHelper.ObjectToTabString(IEnumerable<T> datas, bool descriptionAsHeard = false)  
ReportHelper.EnumerableToTabString(IEnumerable<T> datas, bool descriptionAsHeard = false)
* 2020-10-26 V 3.0.0.13 ���� ReportHelper 
``` C#
            var rpt = new TestReport();
            rpt.ID = Guid.NewGuid().ToString();
            rpt.Name = new IdGenerator().GenerateId().ToString();
            rpt.Ct = DateTime.Now;
            rpt.Count = 109999;

            var rst=ReportHelper.ObjectToTabString(rpt);
            Console.WriteLine(rst);

            var listRst = ReportHelper.EnumerableToTabString(new TestReport[] { rpt, rpt, rpt, rpt, rpt });
            Console.WriteLine(listRst);
```
����Ƿ���֧�ֵġ��Զ�����Ϊ�ָ����ָ����ݡ��ĸ�ʽ��
```
ID,Name,Ct,Count,
e9f8622e-1fd8-4d7c-9333-5f4af8415114,1320690536964820992,2020/10/26 19:35:35,109999,

ID,Name,Ct,Count,
e9f8622e-1fd8-4d7c-9333-5f4af8415114,1320690536964820992,2020/10/26 19:35:35,109999,
e9f8622e-1fd8-4d7c-9333-5f4af8415114,1320690536964820992,2020/10/26 19:35:35,109999,
e9f8622e-1fd8-4d7c-9333-5f4af8415114,1320690536964820992,2020/10/26 19:35:35,109999,
e9f8622e-1fd8-4d7c-9333-5f4af8415114,1320690536964820992,2020/10/26 19:35:35,109999,
e9f8622e-1fd8-4d7c-9333-5f4af8415114,1320690536964820992,2020/10/26 19:35:35,109999,
```

* 2020-12-26 V3.0.0.21 �޸�һ�������ʽ����bug
* 2020-12-18 V3.0.0.20 redis �������
* 2020-11-4: V3.0.0.18  excel ����ʱ���ݳ��ȳ���ʱ�����BUG��
* 2020-11-4��V3.0.0.18  http client �����ദ��heards����bug
* 2020-11-4��V3.0.0.17  http client �����������ļ��ϴ�,�ļ��ϴ��ſ�heards����
* 2020-10-06 V 3.0.0.12 ����Redis�����ࡣ
* 2020-8-26 v3.0.0.11 ����ID������IdGenerator����Ϊ ͳһ�������ࡣ
* 2020-8-26 v3.0.0.9 ������־  LogLocation LogBrowser��user-agent) �ֶ�
* 2020-8-25 V 3.0.0.7
����  loghelper ������Ϊ�������͡�ʹ�÷��������
* 2020-8-22
�޸�log4net �����ļ��Ĵ���
* 2020-8-6
�������� config.MachineID+ config.ServiceID.ToString(); ��Ϊǰ׺��
* 2020-7-29
���Ʊ���淶����
* 2020-7-22 
����  logerHelper error����
* 2020-7-16 3.0.0.2
SnowFlake ���� SnowFlake.core������ʽ���á�
* 2020-7-10
����Ϊ .net core 3.1
* 2020-05-09 ����SnowFlake�㷨����id.
```            
���Ľ����һ��64bit��С��������
1bit�����ã���Ϊ�����������λ�Ƿ���λ��1��ʾ������0��ʾ���������ɵ�idһ�㶼�����������������λ�̶�Ϊ0��
41bit-ʱ�����������¼ʱ��������뼶��
- 41λ���Ա�ʾ�����֣�
- ���ֻ������ʾ�����������������������0�������Ա�ʾ����ֵ��Χ�ǣ�0 �� ����1����Ϊ�ɱ�ʾ����ֵ��Χ�Ǵ�0��ʼ��ģ�������1��
- Ҳ����˵41λ���Ա�ʾ�������ֵ��ת���ɵ�λ��������

10bit-��������id��������¼��������id��
- ���Բ����ڸ��ڵ㣬����5λdatacenterId��5λworkerId
- 5λ��bit�����Ա�ʾ������������ǣ���������0��1��2��3��....31��32�����֣�����ʾ��ͬ��datecenterId��workerId

12bit-���кţ����кţ�������¼ͬ�����ڲ����Ĳ�ͬid��
- 12λ��bit�����Ա�ʾ������������ǣ���������0��1��2��3��....4094��4095�����֣�����ʾͬһ����ͬһʱ��أ�����)�ڲ�����4095��ID��š�

������Java��64bit��������long���ͣ�������Java��SnowFlake�㷨���ɵ�id����long���洢�ġ�

SnowFlake���Ա�֤��

�������ɵ�id��ʱ�����Ƶ���
�����ֲ�ʽϵͳ�ڲ�������ظ�id����Ϊ��datacenterId��workerId�������֣�
```
* 2020-04-28
����EPlatformConfig �ṹ

* 2020-06-01
log4net ��������ת��push ��Ϣ���͵�����
* 2020-06-02
log ö�������ռ������
* 2020-06-03
���� log4net, adonetappender ��
* 2020-06-16
����·���ж�������// ���Ǻ����ۡ�
* 2020-7-1
�ۻ��Ը���