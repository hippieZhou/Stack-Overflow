<div align="center">

# Tools - Nginx

</div>

- http:no-www -> www

```bash
server {
        listen *:80;
        listen [::]:80;
        server_name example.com;
        return 301 http://www.example.com$request_uri;
}

server {
        listen *:80;
        listen [::]:80;
        server_name www.example.com

        location / {
                 #这里指定服务器跳转首页的路径
                 #一般来说代码如下
                 #root 你的网站根目录;
                 #index index.html;
        }
}
```


- http:www -> no-www

```bash
server {
        listen *:80;
        listen [::]:80;
        server_name www.example.com;
        return 301 http://example.com$request_uri;
}

server {
        listen *:80;
        listen [::]:80;
        server_name example.com

        location / {
                 #这里指定服务器跳转首页的路径
                 #一般来说代码如下
                 #root 你的网站根目录;
                 #index index.html;
        }
}
```

- https:no-www -> www

```bash
server {
        listen *:80;
        listen *:443 ssl; 
        listen [::]:80;
        listen [::]:443 ssl; 
        server_name example.com;

        ssl_certificate ssl证书路径 
        ssl_certificate_key ssl密钥路径 
        return 301 https://www.example.com$request_uri;
}

server {
        listen *:80;
        listen [::]:80;
        server_name www.example.com;
        return 301 https://www.example.com$request_uri;
}

server {
        listen *:443 ssl; 
        listen [::]:443 ssl; 
        server_name www.example.com;      
              
        ssl_certificate ssl证书路径 
        ssl_certificate_key ssl密钥路径 
        location / {
                 #这里指定服务器跳转首页的路径
                 #一般来说代码如下
                 #root 你的网站根目录;
                 #index index.html;
        }
}
```

- https:www -> no-www

```bash
server {
        listen *:80;
        listen *:443 ssl; 
        listen [::]:80;
        listen [::]:443 ssl; 
        server_name www.example.com;

        ssl_certificate ssl证书路径 
        ssl_certificate_key ssl密钥路径 
        return 301 https://example.com$request_uri;
}

server {
        listen *:80;
        listen [::]:80;
        server_name example.com;
        return 301 https://example.com$request_uri;
}

server {
        listen *:443 ssl; 
        listen [::]:443 ssl; 
        server_name example.com;    
                
        ssl_certificate ssl证书路径 
        ssl_certificate_key ssl密钥路径 
        location / {
                 #这里指定服务器跳转首页的路径
                 #一般来说代码如下
                 #root 你的网站根目录;
                 #index index.html;
        }
}
```