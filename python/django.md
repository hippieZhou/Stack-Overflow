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

```python
# settings.py

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

```python
# settings.py

EMAIL_BACKEND = 'django.core.mail.backends.smtp.EmailBackend' # 仅在调试模式下启用该配置

EMAIL_HOST = 'smtp.qq.com'
EMAIL_PORT = 25
EMAIL_HOST_USER = '1736252185@qq.com'  # QQ 账号
EMAIL_HOST_PASSWORD = 'spvabkoffesabidb' # 授权码
EMAIL_USE_TLS = True
EMAIL_FROM = '1736252185@qq.com'  #  QQ 账号

# views.py

from django.core.mail import send_mail
from django.conf import settings

send_mail('subject', 'message', settings.EMAIL_FROM, ['hippiezhou@outlook.com','admin@outlook.com'], fail_silently=False)
```

> send_mail 中的发送者邮箱要和代码中使用的发送者邮箱保持一致，否则无法发送

- sitemap

```python
# settings.py

SITE_ID = 1
INSTALLED_APPS = [
    'django.contrib.sites',
    'django.contrib.sitemaps',
]

# sitemaps.py

from django.contrib.sitemaps import Sitemap
from .models import Post

class PostSitemap(Sitemap):
    changefreq = 'weekly'
    priority = 0.9

    def items(self):
        return Post.published.all()

    def lastmod(self, obj):
        return obj.updated

# urls.py

from django.contrib.sitemaps.views import sitemap
from blog.sitemaps import PostSitemap

urlpatterns = [
    path('sitemap.xml', sitemap, {'sitemaps': sitemaps},
         name='django.contrib.sitemaps.views.sitemap')
]
```