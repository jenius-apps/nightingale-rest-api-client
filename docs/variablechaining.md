# Variable chaining

## What is it?
Sometimes, you want to send an authentication request manually, retrieve the token, and then use that token for a different request. 

Nightingale supports this ability to have values from the response of one request used when sending a different request. We call this variable chaining. This is made possible using `environment variables` combined with `chaining rules`. 

## Outline
The basic outline to enable variable chaining is as follows:
1. Create an environment variable.
2. Create a request whose response will be used to update that environment variable.
3. Create a chain rule to overwrite the variable.
4. Create a second request that will consume that variable.
5. And... you're finished!

## Step-by-step
1. Create an environment variable. Click `Manage` on the toolbar to get started. Then once the `Manage Environments` pop up has appeared, create a new variable. Here we've set the name as `token`. 

![](/images/manage-environments.png)

2. Create a request. For this example, our base URL is `jsonplaceholder.typicode.com/posts`.

![](/images/new-request.png)

3. Create a chain rule. In order to overwrite our environment variable, we need to write `token` in the text field saying `Variable to update`. Then we specify what part of the response we wish to use. Here we're using the `title` property of the second object in `response.Body`. See below for other valid properties for chain rules.

![](/images/chain-rule.png)

4. Create a second request that consumes `token`. Here we set up a `POST` request and we provide a body which consumes `token`. The double curly braces in `{{token}}` instructs Nightingale to replace that string with the variable's value when sending the request. Once this is set up, run the first request, which should update the `token` variable. Then run this second request. You'll find that `token` was updated. Hooray!

![](/images/consumer-request.png)

5. And that's how you use Nightingale's variable chaining feature!

## Valid properties for chain rules

When creating chain rules, you can use values from the following properties of the response:
- `Body`
- `ContentType`
- `Headers`
- `Cookies`
- `Successful`
- `StatusCode`
- `StatusDescription`
- `TimeElapsed`

Here are some examples on how to extract specific parts of the response for use in chain rules.

Description | Syntax
--- | ---
Use content type | `response.ContentType`
Use a certain header value | `response.Headers["certain-header-value"]`
Use a certain cookie value | `response.Cookies["certain-cookie-value"]`
Use something from the body | `response.Body.someJsonObject.token`
Use something from the second object in the body's array | `response.Body[1].myProperty`
