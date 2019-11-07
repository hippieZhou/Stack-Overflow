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

- 打包虚拟环境 (.bat)

```bat
SET WD=%CD%
SET PACKAGEPATH=%WD%\win-build
SET "VIRTUALENV=py3_tx_venv"
SET "VIRTUALENVPATH=D:\Desktop\4dogs\TX_Py3"
SET "PYTHON_HOME=D:\Python37"
SET "PYTHON_DLL=D:\Python37\python37.dll"
 
:: get Python version for the runtime build ex. 2.7.1 will be 27
FOR /f "tokens=1 DELims=." %%G IN ('%PYTHON_HOME%/python3.exe -c "import sys; print(sys.version.split(' ')[0])"') DO SET PYTHON_MAJOR=%%G
FOR /f "tokens=2 DELims=." %%G IN ('%PYTHON_HOME%/python3.exe -c "import sys; print(sys.version.split(' ')[0])"') DO SET PYTHON_MINOR=%%G
SET "PYTHON_VERSION=%PYTHON_MAJOR%%PYTHON_MINOR%"
 
 
MKDIR "%PACKAGEPATH%\%VIRTUALENV%" > nul || EXIT /B 1
MKDIR "%PACKAGEPATH%\bin" > nul || EXIT /B 1
 
ECHO Copying python DLLs...
XCOPY /S /I /E /H /Y "%PYTHON_HOME%\DLLs" "%PACKAGEPATH%\%VIRTUALENV%\DLLs" > nul || EXIT /B 1
 
ECHO Copying python Lib...
XCOPY /S /I /E /H /Y "%PYTHON_HOME%\Lib" "%PACKAGEPATH%\%VIRTUALENV%\Lib" > nul || EXIT /B 1
RD /Q /S "%PACKAGEPATH%\%VIRTUALENV%\Lib\site-packages" 1> nul 2>&1 || RD /Q /S "%PACKAGEPATH%\%VIRTUALENV%\Lib\site-packages" 1> nul 2>&1
 
ECHO Copying virtual environment...
XCOPY /S /I /E /H /Y "%VIRTUALENVPATH%\%VIRTUALENV%\Lib" "%PACKAGEPATH%\%VIRTUALENV%\Lib" > nul || EXIT /B 1
 
IF %PYTHON_MAJOR% == 2 (
    :: 如果site-packages中有backports模块，则需要执行此语句，这是因为python3以前该模块少个__init__.py文件
    ECHO Fixing backports.csv for Python 2 by adding missing __init__.py...
    type nul >> "%PACKAGEPATH%\%VIRTUALENV%\Lib\site-packages\backports\__init__.py"
)
 
ECHO Cleaning up unnecessary *.pyc *.pyo and tests files...
FOR /R "%PACKAGEPATH%\%VIRTUALENV%" %%f in (*.pyc *.pyo) do DEL /q "%%f" >nul 2>&1
FOR /R "%PACKAGEPATH%\%VIRTUALENV%" %%f in (tests) do RD /S /Q "%%f" >nul 2>&1
 
ECHO Staging Python...
COPY %PYTHON_DLL% "%PACKAGEPATH%\bin" > nul || EXIT /B 1
COPY %PYTHON_HOME%\python3.exe "%PACKAGEPATH%\bin" > nul || EXIT /B 1
COPY %PYTHON_HOME%\pythonw.exe "%PACKAGEPATH%\bin" > nul || EXIT /B 1
 
EXIT /B 0
```
