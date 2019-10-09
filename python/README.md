<div align="center">

# Python

</div>

- 修改 pip 下载源

1. Windows
```bash
进入 %APPDATA% 目录 创建 pip 文件夹，在里面创建 pip.ini 文件并保存如下内容：

[global]
timeout = 6000
index-url = http://pypi.douban.com/simple
trusted-host = pypi.douban.com
```

2. Ubuntu

```bash
在 /home/<user-name>/ 目录下创建 pip.conf 文件，并保存如下内容：

[global]
timeout = 6000
index-url = http://pypi.douban.com/simple
trusted-host = pypi.douban.com
```


- pipenv

```bash
pip install pipenv
pipenv --help
pipenv --version

# 没有 Pipfile 的情况下
pipenv --python 3
pipenv install flask
pipenv install --dev requests

# 有 Pipfile 的情况下
pipenv install
pipenv install --python=/usr/bin/python3.6
pipenv install --dev


# 进入当前虚拟环境
pipenv shell
# 退出当前虚拟环境
exit

# 查看当前虚拟环境所绑定的物理路径
pipenv --venv

# 删除当前虚拟环境对应的物理文件
pipenv --rm

# 查看当前虚拟环境中依赖包的依赖关系
pipenv graph

# 自定义命令
# Pipfile 文件中增加如下示例命令
# [scripts]
# start = "python main.py"
# test = "pytest"
# list = "pip list"
# 运行上述自定义命令
# pipenv run python main.py
# pipenv run start
# pipenv run test
# pipenv run list
```
