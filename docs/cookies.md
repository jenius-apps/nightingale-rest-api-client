# Cookie Manager

## What is it?
The cookie manager in Nightingale lets you add and delete HTTP cookies from your workspaces. [Click here to learn more about HTTP cookies](https://developer.mozilla.org/en-US/docs/Web/HTTP/Cookies).

Each cookie is associated with a domain. When you send a request, Nightingale will automatically attach a cookie to your request if the request URL's domain matches the cookie.

## Adding cookies

1. Click the cookie icon at Nightingale's top menu bar to open the cookie manager.
    
    ![](../images/cookie-icon.png)
2. There is an area at the top of the cookie manager to add new cookies. Provide the domain and the cookie. Then, press `Add`.
    
    ![](../images/cookie-add-new.png)
3. Your cookie is now associated to your workspace. When you send a request with the same domain, Nightingale will attach the cookie.