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

EMAIL_BACKEND = 'django.core.mail.backends.console.EmailBackend' # 启用项可在本地模拟邮件接收

EMAIL_BACKEND = 'django.core.mail.backends.smtp.EmailBackend'
EMAIL_HOST = 'smtp.qq.com'                # 邮件主机，默认是localhost
EMAIL_PORT = 25                           # SMTP服务端口，默认是25
EMAIL_HOST_USER = '1736252185@qq.com'     # SMTP服务器的用户名
EMAIL_HOST_PASSWORD = 'spvabkoffesabidb'  # SMTP服务器的密码，授权码
DEFAULT_FROM_EMAIL = EMAIL_HOST_USER      # 避免出现 501：smtplib.SMTPSenderRefused 错误
EMAIL_USE_TLS = True                      # 是否使用TLS进行连接
# EMAIL_USE_SSL = True                    # 是否使用SSL进行连接

# views.py

from django.core.mail import send_mail
from django.conf import settings

# 同步方式
send_mail('subject', 'message', settings.DEFAULT_FROM_EMAIL, ['hippiezhou@outlook.com','admin@outlook.com'], fail_silently=False) 

# 异步方式
th = Thread(target=send_mail, args=['subject', 'message', settings.DEFAULT_FROM_EMAIL, ['hippiezhou@outlook.com','admin@outlook.com'], fail_silently=False])
th.start()
```

> fail_silently=False 表示如果发送失败就抛出异常。如果看到返回1，就说明邮件成功发送；

- [标签功能：django-taggit](https://github.com/alex/django-taggit)

- 自定义模板标签和过滤器
    - simple_tag: 处理数据并且返回字符串
    - inclusion_tag: 处理数据并返回一个渲染的模板

```python

# 组织结构
# blog/
#     __init__.py
#     models.py
#     ...
#     templatetags/
#         __init__.py
#         blog_tags.py

# blog_tags.py

from django import template
from django.db.models import Count
from django.utils.safestring import mark_safe

import markdown

from ..models import Post

register = template.Library()


@register.simple_tag
def total_posts():
    return Post.published.count()


@register.inclusion_tag('blog/post/laest_posts.html')
def show_latest_posts(count=5):
    latest_posts = Post.published.order_by('-publish')[:count]
    return {'latest_posts': latest_posts}


@register.simple_tag
def get_most_commented_posts(count=5):
    return Post.published.annotate(total_comments=Count('comments')).order_by('-total_comments')[:count]


@register.filter(name='markdown')
def markdown_format(text):
    return mark_safe(markdown.markdown(text))
```

```html
{% load blog_tags %}

<p>This is my blog. I've written {% total_posts %} posts so far.</p>

<h3>Latest posts</h3>
{% show_latest_posts %}

<h3>Most commented posts</h3>
{% get_most_commented_posts as most_commented_posts %}
<ul>
    {% for post in most_commented_posts %}
    <li>
        <a href="{{ post.get_absolute_url }}">{{ post.title }}</a>
    </li>
    {% endfor %}
</ul>

{{ post.body|markdown|truncatewords_html:30 }}
```

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

sitemaps = {'posts': PostSitemap,}

urlpatterns = [
    path('sitemap.xml', sitemap, {'sitemaps': sitemaps},
         name='django.contrib.sitemaps.views.sitemap')
]
```

> 执行 python manage.py migrate 后会在后台创建对应的管理模型

- feeds

```python
# feeds.py

from django.contrib.syndication.views import Feed
from django.template.defaultfilters import truncatewords
from .models import Post


class LastestPostFeed(Feed):
    title = 'My Blog'
    link = '/blog/'
    description = 'New posts of my blog.'

    def items(self):
        return Post.published.all()[:5]

    def item_title(self, item):
        return item.title

    def item_description(self, item):
        return truncatewords(item.body, 30)

# urls.py

from .feeds import LastestPostFeed

urlpatterns = [
    path('feed/', LastestPostFeed(), name='post_feed'),
]
```
- 全文搜索
    - [PostgreSQL](https://www.postgresql.org/)
    - [Elasticsearch](http://es-guide-preview.elasticsearch.cn/)
    - [Haystack](http://haystacksearch.org/)

- 自定义中间件

```python
# RemoteAddrFromForwardedForMiddleware.py

from django.utils.deprecation import MiddlewareMixin


class RemoteAddrFromForwardedForMiddleware(MiddlewareMixin):
    def process_request(self, request):
        x_forwarded_for = request.META.get('HTTP_X_FORWARDED_FOR')
        ip = x_forwarded_for.split(',')[0] if x_forwarded_for else request.META.get('REMOTE_ADDR')
        request.META['REMOTE_ADDR'] = ip

# settings.py

MIDDLEWARE = [
    'website.http.RemoteAddrFromForwardedForMiddleware',
]

# views.py

ip = request.META.get('REMOTE_ADDR',None)
```

- 自定义异常页面

```python
# errors.py

from django.shortcuts import render
from django.http import JsonResponse


def handler404(request, *args, **argv):
    if request.content_type.find('application/json') > -1:
        response = JsonResponse({'error': 'Not found'}, status=404)
    else:
        response = render(request, '404.html', status=404)
    return response


def handler500(request, *args, **argv):
    if request.content_type.find('application/json') > -1:
        response = JsonResponse({'error': 'Server internal error'}, status=500)
    else:
        response = render(request, '500.html', status=500)
    return response

# urls.py

handler404 = 'website.error_views.handler404'
handler500 = 'website.error_views.handler500'
```