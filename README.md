# DotNetDAL
dotnet core 分库dal  转自携程dal https://github.com/ctripcorp/dal/tree/master/Arch.Data

配置文件在 https://github.com/wangchengqun/DotNetDAL/blob/master/Demo/bin/Debug/netcoreapp2.0/conf.json 

测试sql文件 https://github.com/wangchengqun/DotNetDAL/blob/master/sql.sql


分布式日志数据库,直接F5调试 Raven.Server项目

Raven.Server项目的配置文件 https://github.com/wangchengqun/DotNetDAL/blob/master/LogDataBase/RavenDB/bin/Debug/netcoreapp2.0/settings.json

启动成功后 在浏览器收入 http://127.0.0.1:8888 并创建数据库Test

![image1](https://github.com/wangchengqun/DotNetDAL/blob/master/Image/image1.png)

![image2](https://github.com/wangchengqun/DotNetDAL/blob/master/Image/image2.png)

![image3](https://github.com/wangchengqun/DotNetDAL/blob/master/Image/image3.png)

一个单机版的日志数据库就部署好了。


Demo 项目配置文件 在bin\Debug\netcoreapp2.0\conf.json 
字段logDataBaseUrl 是 Raven.Server 项目的配置文件的ServerUrl字段, 默认(http://127.0.0.1:8888)
字段logDatabase 创建数据库的名字 Test


计划添加同步ES和mongodb功能







