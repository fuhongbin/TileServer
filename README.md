## 说明

使用 Owin WebAPI + OAuth2 的例子， 重现一个在 jexus 标准版下运行正常， 但是在 jexus 独立
版下运行出错的 bug ， 找不到错误信息。

## 测试方式

项目是个命令行程序， 可以使用 VS2013， VS for Mac 打开， 可以直接运行， 启动之后在浏览器输
入地址 `http://127.0.0.1:8088/wwwroot/index.html` ;

> 由于配置了 OAuth 认证， 所以上面的 URL 不能修改

等待加载完成， 出现 `GDEP Login` 按钮之后， 点击这个按钮， 重定向到客户的 OAuth 认证页面，
用户名 baojun ， 密码 huitianbaojun123 ， 再输入验证码之后， oauth 认证完成后自动返回。

在调试状态下， 使用的 Nowin 做服务器， 可以返回正确的认证信息， 地址是：

```
http://127.0.0.1:8088/rest/account/external-login-callback
```

浏览器显示：

```
"User: baojun , is authenticated: True"
```

在 Jexus 独立版运行时， 回调地址是：

```
http://127.0.0.1:8088/rest/account/external-login-callback?error=access_denied
```

浏览器显示：

```json
{"Message":"External Login Error!"}
```
