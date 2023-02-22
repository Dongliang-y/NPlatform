### 产品名称： Not Platform
### 
### 作者：Dongliang Yi
### 基本说明：
 Not Platform 底层脚手架
 约束与规范了：
 applications
 DI
 Domains
 Repositories
 Result
 Dto

 ### CQRS 说明：
 基于MediatR 实现。
 继承链： IDTO->ICommand(IEvent)-> CommandBase(EventBase) ->CommandObject(EventObject) .
 业务实现中的修改链：封装CommandObject(Controller层)--> Bus.SendCommand(Controller层) --> CommandHander(DommainService OR application )-->调用仓储实现序列化
 业务实现中的查询链：封装 Query 对象  (Controller层)--> 调用查询服务(DommainService OR application )-->调用仓储。

### 更新记录：
2021/9/8 NPlatform 底层脚手架 start！