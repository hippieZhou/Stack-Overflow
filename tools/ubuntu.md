<div align="center">

# Tools - Ubuntu

</div>

- 查看系统版本

```bash
cat /proc/version
```

- Ubuntu 系统美化

```bash
# 安装 新立得软件包管理器
sudo apt-get install synaptic

# 安装桌面管理工具及主题
sudo apt install gnome-tweaks

# 安装 numix 主题
sudo add-apt-repository ppa:numix/ppa
sudo apt-get update
sudo apt-get install numix-gtk-theme numix-icon-theme-circle
sudo apt-get install numix-wallpaper-*
```

- 修改分辨率

```bash
sudo vim /etc/default/grub

添加：GRUB_CMDLINE_LINUX_DEFAULT="quiet splash video=hyperv_fb:1920x1080"

执行 sudo update-grub

执行 reboot 即可
```

- 进程管理

```bash
# 查看自己的进程
ps -l
# 查看系统所有进程
ps aux
# 查看特定的进程
ps aux | grep threadx 
# 查看所有进程树
pstree -A
# 实时显示进程信息（每2秒）
top -d 2
# 查看特点j端口的进程
netstat -anp | grep port
```

> 僵尸进程通过 ps 命令显示出来的状态为 Z（zombie）（要消灭系统中大量的僵尸进程，只需要将其父进程杀死，此时僵尸进程就会变成孤儿进程，从而被 init 进程所收养，这样 init 进程就会释放所有的僵尸进程所占有的资源，从而结束僵尸进程。）

- WSL
    - [HyperV 启用增强模式](https://github.com/Microsoft/linux-vm-tools/issues/76)
    - [网络连接问题](https://github.com/microsoft/WSL/issues/5)

- 酸酸乳

```
wget -N --no-check-certificate https://raw.githubusercontent.com/ToyoDAdoubi/doubi/master/ssr.sh && chmod +x ssr.sh && bash ssr.sh
./ssr.sh

wget -N --no-check-certificate https://raw.githubusercontent.com/ToyoDAdoubiBackup/doubi/master/ss-go.sh && chmod +x ss-go.sh && bash ss-go.sh

wget --no-check-certificate -O shadowsocks-all.sh https://raw.githubusercontent.com/teddysun/shadowsocks_install/master/shadowsocks-all.sh
chmod +x shadowsocks-all.sh
./shadowsocks-all.sh

wget -N --no-check-certificate https://raw.githubusercontent.com/FunctionClub/YankeeBBR/master/bbr.sh && bash bbr.sh install
bash bbr.sh start
```

[小火箭](https://小火箭.ink/)

- TG Proxy

```bash
https://github.com/TelegramMessenger/MTProxy
./mtproto-proxy -u nobody -p 8888 -H 7777 -S 693bcc00760d58c850f684a197f0db05 --aes-pwd proxy-secret proxy-multi.conf -M 1、

wget -N --no-check-certificate https://raw.githubusercontent.com/ToyoDAdoubiBackup/doubi/master/mtproxy_go.sh && chmod +x mtproxy_go.sh && bash mtproxy_go.sh
```

- 远程连接

```bash
# 查看本机ip
ipconfig（sudo apt install net-tools）

# 服务器需要安装 open-ssh
sudo apt-get install openssh-server

# ssh 远程登录
ssh [要控制的用户名]@[它的IP地址]

# 重新生成 ssh-key
ssh-keygen -R 要 ssh 去的 ip 

# 省略密码直接登录 
ssh-keygen
ssh-copy-id user@ip
```

- 远程推送

```bash
# 一个文件
 ssh morvan@192.168.0.114 python3 < ~/Desktop/machine_learning.py

# 多个文件
scp ~/Desktop/{a,b}.py morvan@192.168.0.114:~/Desktop
scp -r ~/Desktop/helloworld/ morvan@192.168.0.114:~/Desktop/helloworld/
```

- 设置共享文件夹
   - 创建共享文件夹，设置属性为共享和可读写属性
   - 设置共享文件夹密码：sudo smbpasswd -a 用户名


- 常用命令
	- who：在关机前需要先使用 who 命令查看有没有其它用户在线。
	- sync：为了加快对磁盘文件的读写速度，位于内存中的文件数据不会立即同步到磁盘上，因此关机之前需要先进行 sync 同步操作。
	
- shutdown

```bash
shutdown [-krhc] 时间 [信息]
-k ： 不会关机，只是发送警告信息，通知所有在线的用户
-r ： 将系统的服务停掉后就重新启动
-h ： 将系统的服务停掉后就立即关机
-c ： 取消已经在进行的 shutdown 指令内容
```

- vim

```bash
i      进入插入模式
:w     写入磁盘
:w!    当文件为只读时，强制写入磁盘。到底能不能写入，与用户对该文件的权限有关
:q     离开
:q!    强制离开不保存
:wq  写入磁盘后离开
:wq! 强制写入磁盘后离开
```

- 查看分区大小
	
```bash
du -h
sudo fdisk -l
```

- ls & cd

```bash
cd - ：返回刚刚所在的目录
cd ../../ ：cd ../../
cd ~ ：去往 Home
```

- ls

```bash
-l：显示详细信息
-d：仅列出目录本身
-a：显示所有文件
-lh：比 -l 显示更详细
--help：帮助
```

- touch & cp & mv

```bash
touch [-acdmt] filename
-a ： 更新 atime
-c ： 更新 ctime，若该文件不存在则不建立新文件
-m ： 更新 mtime
-d ： 后面可以接更新日期而不使用当前日期，也可以使用 --date="日期或时间"
-t ： 后面可以接更新时间而不使用当前时间，格式为[YYYYMMDDhhmm]

cp file1 file2：复制文件，如果文件以存在则覆盖
cp -i file1 file2：复制文件1到文件2，若文件存在则提示
cp -R  folder1/ folder2/ ：复制文件夹1 到文件夹2
cp file* folder2/ ：复制多个文件. 复制名字部分相同的多个文件, * 是说”你就找文件前面是 file 的文件, 后面是什么名字无所谓”
cp file1 file2 folder1/
-a ：相当于 -dr --preserve=all 的意思，至于 dr 请参考下列说明
-d ：若来源文件为链接文件，则复制链接文件属性而非文件本身
-i ：若目标文件已经存在时，在覆盖前会先询问
-p ：连同文件的属性一起复制过去
-r ：递归持续复制
-u ：destination 比 source 旧才更新 destination，或 destination 不存在的情况下才复制
--preserve=all ：除了 -p 的权限相关参数外，还加入 SELinux 的属性, links, xattr 等也复制了

mv file folder1/
mv file1 file1rename：重命名
```

- mkdir & rmdir & rm

```bash
mkdir -m：配置目录权限
mkdir -p ：递归创建目录

rm -i：带提示的删除
rm -p：递归删除目录
rm -r folder1：递归删除非空文件夹
```

- nano & cat

```bash
cat file1 ：查看文件内容
cat file1 > file2 ：将文件1的内容放到文件2中
cat file1 >> file2 ：将文件1的内容追加到文件2中
```

- chmod 修改权限

```bash
chmod [谁][怎么修改] [哪个文件]

[谁]
u: 对于 User 修改
g: 对于 Group 修改
o: 对于 Others 修改
a: (all) 对于所有人修改

[怎么修改]
+, -, =: 作用的形式, 加上, 减掉, 等于某些权限
r, w, x 或者多个权限一起, 比如 rx

[哪个文件]
施加操作的文件, 可以为多个
```