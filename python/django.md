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
EMAIL_HOST = 'smtp.qq.com' 
EMAIL_HOST_USER = "admin" 
EMAIL_HOST_PASSWORD = "xxxxxxxxxxxxx" 
EMAIL_PORT = 587
EMAIL_USE_TLS = True
DEFAULT_FROM_EMAIL = "admin@qq.com"
```

```python
 from django.core.mail import send_mail 

 send_mail('hippiezhou','helloworld','1736252185@qq.com',['hippiezhou@outlook.com'],fail_silently=False)
```