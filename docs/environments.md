# Environments

## What is it?
Sometimes, you may want to reuse a string such as an authentication across different requests. With Nightingale's environment feature, you can do just that.

An environment is a collection of variables that can be reused across a workspace. You can have any number of environments, and you can select one of them to be the "active" environment.

Each time you send a request or you execute a collection, Nightingale will use variables that are stored in your active environment or in your base environment.

## How do I use variables?
To use environment variables, simply wrap a variable name with double curly braces, like this: `{{myVariableName}}`.

When Nightingale sees this, it will replace the variable with the corresponding value from your environment when you execute your request.

Here are the different areas in Nightingale where you can use variables:

Area | Example
--- | ---
Base URL | `https://{{myHostVariable}}/path?foo=bar`
Body | `{ "hello": {{worldVariable}} }`
Headers | `{{contentType}}`
Queries | `{{queryVariable}}`
Bearer token authentication | `{{someToken}}`

## Active and base environments
Each time you execute a request, Nightingale will replace any variables in your request using the content of your active environment. 

Each workspace always has a base environment. By default, it is the "active" environment. If you select a different active environment, the base environment may still be used as a fallback if the active environment does not contain a particular variable name. 

If neither active nor base environments contain a particular variable name that needs to be resolved, Nightingale will not modify the ``{{variable}}`` in the request.
