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

# 多语言文件的生成
django-admin makemessages --all
# 编译消息文件
django-admin compilemessages
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

- account
    - [使用内置验证视图(from django.contrib.auth import views as auth_views)](https://docs.djangoproject.com/en/2.2/topics/auth/default/#all-authentication-views)

```bash
LoginView：处理登录表单填写和登录功能
- path('login/', auth_views.LoginView.as_view(), name='login'),
- login.html

LogoutView：退出登录
- path('logout/', auth_views.LogoutView.as_view(), name='logout'),
- logged_out.html

PaswordChangeView：处理一个修改密码的表单，然后修改密码
- path('password_change/', auth_views.PasswordChangeView.as_view(), name='password_change'),
- password_change_form.html

PasswordChangeDoneView：成功修改密码后执行的视图
- path('password_change/done/', auth_views.PasswordChangeDoneView.as_view(), name='password_change_done'),
- password_change_done.html

PasswordResetView：用户选择重置密码功能执行的视图，生成一个一次性重置密码链接和对应的验证token，然后发送邮件给用户
- path('password_reset/', auth_views.PasswordResetView.as_view(), name='password_reset'),
- password_reset_form.html

PasswordResetDoneView：通知用户已经发送给了他们一封邮件重置密码
- path('password_reset/done/', auth_views.PasswordResetDoneView.as_view(), name='password_reset_done'),
- password_reset_done.html

PasswordResetConfirmView：用户设置新密码的页面和功能控制
- path('reset/<uidb64>/<token>/', auth_views.PasswordResetConfirmView.as_view(), name='password_reset_confirm'),
- password_reset_email.html

PasswordResetCompleteView：成功重置密码后执行的视图
- path('reset/done/', auth_views.PasswordResetCompleteView.as_view(), name='password_reset_complete'),
- password_reset_complete.html
```

> 如果想自定义视图页面，上述视图需要放到 `templates/registration` 目录下；需要在 `settings.py` 中将 `INSTALLED_APPS` 配置中的对应 *app* 放到 `django.contrib.admin` 之前；如果需要使用 `PasswordResetView` 则需要进行邮箱服务器配置；

- [消息框架(django.contrib.messages.middleware.MessageMiddleware)](https://docs.djangoproject.com/en/2.2/ref/contrib/messages/)
    - success()：一个动作成功之后发送的消息
    - info()：通知性质的消息
    - warning()：警告性质的内容，所谓警告就是还没有失败但很可能失败的情况
    - error()：错误信息，通知操作失败
     -debug()：除错信息，给开发者展示，在生产环境中需要被移除

```python
# views.py
from django.contrib import messages

messages.success(request, 'Profile updated successfully')
messages.error(request, 'Something went wrong')
```

- 自定义验证后端（可用于多种账号登录方式）

```python
# authentication.py

from django.contrib.auth.models import User

class EmailAuthBakcend:
    """
    Authenticate using an e-mail address.
    """

    def authenticate(self, request, username=None, password=None):
        try:
            user = User.objects.get(email=username)
            if user.check_password(password):
                return user
            return None
        except User.DoesNotExist:
            return None

    def get_user(self, user_id):
        try:
            return User.objects.get(id=user_id)
        except User.DoesNotExist:
            return None

# settings.py

AUTHENTICATION_BACKENDS = [
    'django.contrib.auth.backends.ModelBackend',
    'account.authentication.EmailAuthBackend',
]

# 上述代码段用于登录界面支持用户名和注册邮箱登录
```

> AUTHENTICATION_BACKENDS中的顺序很重要，如果一个用户信息对于多个验证后端都有效，Django会停止在第一个成功验证的后端处。

- 第三方认证登录
 - [Python Social Auth](https://github.com/python-social-auth)

- 缩略图
    - [sorl-thumbnail](https://sorl-thumbnail.readthedocs.io/)

- AJAX 中使用 CSRF

```python
# 创建自定义装饰器
# common/
#     __init__.py
#     decorators.py

# common/decorators.py
from django.http import HttpResponseBadRequest

def ajax_required(f):
    def wrap(request, *args, **kwargs):
        if not request.is_ajax():
            return HttpResponseBadRequest()
        return f(request, *args, **kwargs)
    wrap.__doc__=f.__doc__
    wrap.__name__=f.__name__
    return wrap


# views.py
from common.decorators import ajax_required
from django.contrib.auth.decorators import login_required
from django.views.decorators.http import require_POST

@ajax_required
@login_required
@require_POST
def image_like(request):
    image_id = request.POST.get('id')
    action = request.POST.get('action')
    if image_id and action:
        try:
            image = Image.objects.get(id=image_id)
            if action == 'like':
                image.users_like.add(request.user)
            else:
                image.users_like.remove(request.user)
            return JsonResponse({'status':'ok'})
        except:
            pass
    return JsonResponse({'status':'ko'})
```

```html
<!-- base.html -->

<script src="https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/js-cookie@2/src/js.cookie.min.js"></script>
<script>
    let csrftoken = Cookies.get('csrftoken');

    function csrfSafeMethon(method) {
        // 如下的HTTP请求不需要设置CRSF信息
        return (/^(GET|HEAD|OPTIONS|TRACE)$/.test(method));
    }

    $.ajaxSetup({
        beforeSend: function (xhr, settings) {
            if (!csrfSafeMethon(settings.type) && !this.crossDomain) {
                xhr.setRequestHeader("X-CSRFToken", csrftoken);
            }
        }
    });
        $(document).ready(function () {
        {% block domready %}
        {% endblock %}
    });
</script>

<!-- views.html -->
{% block domready %}
$('a.like').click(function (e) {
    e.preventDefault();
    $.post('{% url 'images:like' %}',
        {
            id: $(this).data('id'),
            action: $(this).data('action'),
        },
        function (data) {
            if (data['status'] === 'ok') {
                let previous_action = $('a.like').data('action');
                //切换 data-action 属性
                $('a.like').data('action', previous_action === 'like' ? 'unlike' : 'like');
                //切换按钮文本
                $('a.like').text(previous_action === 'like' ? 'Unlike' : 'Like');
                //更新总的喜欢人数
                let previous_likes = parseInt($('span.count.total').text());
                $('span.count.total').text(previous_action === 'like' ? previous_likes + 1 : previous_likes - 1);
            }
        }
    );
});
{% endblock %}
```

- Ajax 分页

```python
# views.py
from django.http import HttpResponse
from django.core.paginator import Paginator, EmptyPage, PageNotAnInteger

@login_required
def image_list(request):
    images = Image.objects.all()
    paginator = Paginator(images, 8)
    page = request.GET.get('page')
    try:
        images = paginator.page(page)
    except PageNotAnInteger:
        # 如果页数不是整数，就返回第一页
        images = paginator.page(1)
    except EmptyPage:
        # 如果是不存在的页数，而且请求是AJAX请求，返回空字符串
        if request.is_ajax():
            return HttpResponse('')
        # 如果页数超范围，显示最后一页
        images = paginator.page(paginator.num_pages)
    if request.is_ajax():
        return render(request, 'images/image/list_ajax.html', {'section': 'images', 'images': images})
    return render(request, 'images/image/list.html', {'section': 'images', 'images': images})

# urls.py
path('', views.image_list, name='list'),
```

```html
<!-- list_ajax.html -->
{% block domready %}
let page = 1;
let empty_page = false;
let block_request = false;
$(window).scroll(
    function () {
        let margin = $(document).height() - $(window).height() - 200;
        if ($(window).scrollTop() > margin && empty_page === false && block_request === false) {
            block_request = true;
            page += 1;
            $.get("?page=" + page, function (data) {
                if (data === "") {
                    empty_page = true;
                }
                else {
                    block_request = false;
                    $('#image-list').append(data)
                }
            });
        }
    }
);
{% endblock %}
```