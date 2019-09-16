<div align="center">

# Python - Django

</div>

- 基本命令

```bash
python -m django --version
django-admin startproject mysite .
python manage.py runserver

python manage.py startapp appname .
python manage.py makemigrations appname
python manage.py sqlmigrate appname migration_id
python manage.py migrate

python manage.py shell 

python manage.py createsuperuser

python manage.py test appname

python manage.py collectstatic

python manage.py check --deploy
```

- MySQL 配置

`settings.py`

```python
DATABASES = {
     'default': {
        'ENGINE': 'django.db.backends.mysql',
         'NAME': 'hippiezhou_fun',
         'USER': 'root',
         'PASSWORD': 'root',
         'HOST': '192.168.0.58',
         'PORT': '3306',
     }
 }
```

- Email 配置

`settings.py`

```python
EMAIL_BACKEND = 'django.core.mail.backends.smtp.EmailBackend' # 仅在调试模式下启用该配置

EMAIL_HOST = 'smtp.qq.com'
EMAIL_PORT = 25
EMAIL_HOST_USER = '1736252185@qq.com'  # QQ 账号
EMAIL_HOST_PASSWORD = 'spvabkoffesabidb' # 授权码
EMAIL_USE_TLS = True
EMAIL_FROM = '1736252185@qq.com'  #  QQ 账号
```

```python
from django.core.mail import send_mail
from django.conf import settings

send_mail('subject', 'message', settings.EMAIL_FROM, ['hippiezhou@outlook.com','admin@outlook.com'], fail_silently=False)
```

> send_mail 中的发送者邮箱要和代码中使用的发送者邮箱保持一致，否则无法发送