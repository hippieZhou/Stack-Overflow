<div align="center">

# Tools - Distributed

</div>

- redis

```bash
# 使用场景：
# 计数：从我们的例子可以看出，使用Redis管理计数非常便捷，incr()和incrby()方法可以方便的实现计数功能。
# 存储最新的项目：使用lpush()和rpush()可以向一个队列的开头和末尾追加数据，lpop()和rpop()则是从队列开始和末尾弹出元素。如果操作造成队列长度改变，还可以用ltrim()保持队列长度。
# 队列：除了上边的pop和push系列方法，Redis还提供了阻塞队列的方法
# 缓存：expire()和expireat()方法让用户可以把Redis当做缓存来使用，还可以找到一些第三方开发的将Redis配置为Django缓存后端的模块。
# 订阅/发布：Redis提供订阅/发布消息模式，可以向一些频道发送消息，订阅该频道的Redis客户端可以接受到该消息。
# 排名和排行榜：Redis的有序集合可以方便的创建排名相关的数据。
# 实时跟踪：Redis的高速I/O可以用在实时追踪并更新数据方面。

# 安装
sudo apt-get install redis-server

# 启动进程
redis-server

# 进入
redis-cli
```

- mq

```bash
# 安装
sudo apt-get install rabbitmq-server

# 启动
sudo rabbitmq-server
```

- celery

