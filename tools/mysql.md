<div align="center">

# Tools - MySQL

</div>

- 安装

```bash
sudo apt update && sudo apt install mysql-server
sudo service mysql status
sudo service mysql stop
sudo service mysql start
sudo mysql_secure_installation
> 第一步：是否安装验证密码插件：N
> 第二不：为 root 设置密码 (执行 mysql -u root -p 时会用到)
> 第三步：是否删除匿名用户
> 第四步：是否允许 root 远程登录
> 第五步：是否删除测试数据库
> 第六步：是否对授权表立即生效
```

- 卸载

```bash
sudo -i
service mysql stop
killall -KILL mysql mysqld_safe mysqld
apt-get --yes purge mysql-server mysql-client
apt-get --yes autoremove --purge
apt-get autoclean
deluser --remove-home mysql
delgroup mysql
rm -rf /etc/apparmor.d/abstractions/mysql /etc/apparmor.d/cache/usr.sbin.mysqld /etc/mysql /var/lib/mysql /var/log/mysql* /var/log/upstart/mysql.log* /var/run/mysqld
updatedb
exit
```

- 配置远程连结

```bash
sudo vim /etc/mysql/mysql.conf.d/mysqld.cnf

# 在 [mysqld] 的节点区域最后( (大概在 40 行))添加 skip-grant-tables
# 将 bind-address = 127.0.0.1 (大概在 43 行)  -> bind-address = 0.0.0.0

# 保存退出后执行下述操作
sudo mysql -u root -p

# 修改 root 用户登录方式
use mysql;
select host,user from user;
update user set host = '%' where user = 'root';

# 如果 root 用户的 authentication_string 为空且plugin 不为 'mysql_native_password' 则需要修改该用户的认证方式

grant all on *.* to root@'%' identified by 'password' with grant option;
flush privileges;    # 刷新权限
exit;

# 退出后重启服务
sudo systemctl restart mysql # 或者（sudo /etc/init.d/mysql restart）
```

- 调整 root 用户认证方式和权限

```bash
sudo mysql
select user,authentication_string,plugin,host from mysql.user;
update user set authentication_string=PASSWORD("<password>") where user='root';
update user set plugin="mysql_native_password";
create user '<user-name>'@'%' identified by '<password>';
grant all privileges on *.* to '<user-name>'@'%' with grant option; # 全局权限
revoke all privileges on *.* from '<user-name>'@'%'; # 回收上面 grant 语句赋予的权限
grant all privileges on <db-name>.* to '<user-name>'@'%' with grant option; # db 权限
grant all privileges on <db-name>.<table-name> to '<user-name>'@'%' with grant option; # 表权限
GRANT SELECT(id), INSERT (id,a) ON mydb.mytbl TO 'ua'@'%' with grant option; # 列权限
flush privileges;
quit;
```

-  设置字符编码(修改完毕后需要重启服务)

```bash
sudo vim /etc/mysql/conf.d/mysql.cnf 修改如下：
[mysql]
no-auto-rehash
default-character-set=utf8
[mysqld]
socket = /var/run/mysqld.sock
port =3306
character-set-server=utf8
```

- 常用命令

```bash
mysql -u root -p
mysql -h$ip -P$port -u$user -p

# 查看当前密码策略
SHOW VARIABLES LIKE 'validate_password%';

# 创建用户
CREATE USER 'hippie'@'localhost' IDENTIFIED BY 'password';
CREATE USER 'hippie'@'%' IDENTIFIED BY 'password'; # 8.0之后
grant all privileges on db_name.* to db_user@'%' identified by 'password'; # 8.0之前

# 删除用户
DROP USER 'database_user'@'localhost';

# 修改密码
ALTER USER 'root'@'localhost' IDENTIFIED WITH mysql_native_password BY '<your-password>'; # 8.0之后
update mysql.user set authentication_string=password("<your-password>") where user="root"; # 8.0之前

# 数据库相关操作
create database `<db-name>` character set 'utf8' collate 'utf8_bin';
show databases;
drop database <db-name>;

use <db-name>;

# 导入数据库
source /data/backup.sql(数据库绝对路径)

# 查看数据库用户信息
SELECT DISTINCT CONCAT('User: ''',user,'''@''',host,''';') AS query FROM mysql.user;

# 数据表相关操作
create table <table-name> ENGINE=InnoDB;
create table <table-new-name> like <table-name>;
show create table <table-name> t\G;
desc <table-name>;

# 查看正在运行的执行i列表
show processlists;

# 查看存储引擎
show engines;

# 查看编码格式
show variables like 'character%';

# 查看隔离级别
show variables like 'transaction_isolation';

exit; 
```

> 如果通过 Navicat 来进行创建数据库的话，建议设置对应的数据库字符集为 mtf8mb4，对应的排序规则为：utf8mb4_general_ci

- 关闭公网IP
 - 启动参数或者配置文件中设置bind-address= IP绑定内部IP
 - 以root账号连接数据库，排查user表中用户的host字段值为%或者非localhost的用户，修改host为localhost或者指定IP或者删除没必要用户。

- 长短连接

尽量使用长连接（短连接易消耗资源)，定期断开长连接（重连）

- 查询l流程

连接器 -> 查询缓存(8.0以后被移除) -> 分析器(语法分析) -> 优化器 -> 执行器 -> 存储引擎

- 更新流程

连接器 -> 查询h内存(如果没有则从磁盘读取到内存) -> 返回行数据 -> 写入新行 -> 更新内存 -> 写入 redolog(处于 prepare 阶段) -> 写入 binlog -> 提交事务(处于 commit 状态)

- 日志系统

 物理日志 redo log
>redo log用于保证crash-safe能力。innodb_flush_log_at_trx_commit这个参数设置成1的时候，表示每次事务的redo log都直接持久化到磁盘。这个参数我建议你设置成1，这样可以保证MySQL异常重启之后数据不丢失。

逻辑日志 binlog
> sync_binlog这个参数设置成1的时候，表示每次事务的binlog都持久化到磁盘。这个参数我也建议你设置成1，这样可以保证MySQL异常重启之后binlog不丢失。

> MySQL日志系统密切相关的“两阶段提交”。两阶段提交是跨系统维持数据逻辑一致性时常用的一个方案，即使你不做数据库内核开发，日常开发中也有可能会用到。

- 事务

【ACID（Atomicity、Consistency、Isolation、Durability，即原子性、一致性、隔离性、持久性）】 是保证一组数据库操作，要么全部成功，要么全部失败；在MySQL，事务支持是在引擎层实现的。

- 索引

常见模型：哈希表(适用于等值查询的场景)、有序数组(适用于静态存储引擎)、搜索树
InnoDB 采用的是 B+ 树索引模型，是索引组织表，B+树能够很好地配合磁盘的读写特性，减少单次查询的磁盘访问次数。建议使用自增主键

> 如果语句是select * from T where ID=500，即主键查询方式，则只需要搜索ID这棵B+树；
如果语句是select * from T where k=5，即普通索引查询方式，则需要先搜索k索引树，得到ID的值为500，再到ID索引树搜索一次。这个过程称为回表。

- 锁

根据加锁的范围，MySQL里面的锁大致可以分成全局锁(主要用在逻辑备份过程中)、表级锁(表锁和元数据锁)和行锁三类

如果事务中需要锁多个行，要把最可能造成锁冲突、最可能影响并发度的锁尽量往后放。
对于唯一索引来说，需要将数据页读入内存，判断到没有冲突，插入这个值，语句执行结束；
对于普通索引来说，则是将更新记录在change buffer，语句执行就结束了。
redo log 主要节省的是随机写磁盘的IO消耗（转成顺序写），而change buffer主要节省的则是随机读磁盘的IO消耗。

> 直接创建完整索引，这样可能比较占用空间；创建前缀索引，节省空间，但会增加查询扫描次数，并且不能使用覆盖索引；倒序存储，再创建前缀索引，用于绕过字符串本身前缀的区分度不够的问题；
创建hash字段索引，查询性能稳定，有额外的存储和计算消耗，跟第三种方式一样，都不支持范围扫描。

InnoDB 引擎把数据放在主键索引上，其他索引上保存的是主键id。这种方式，我们称之为索引组织表（Index Organizied Table）；而 Memory 引擎采用的是把数据单独存放，索引上保存数据位置的数据组织形式，我们称之为堆组织表（Heap Organizied Table）。

InnoDB 表的数据总是有序存放的，而内存表的数据就是按照写入顺序存放的；

> 表默认的自增主键id最大是 2的32次方-1（4294967295)
