# Mock servers - BETA

_This feature is in beta, so please create a GitHub issue and send us feedback!_

## What is it?
When you're building an API, sometimes you want to design the API first before writing any code. But when you design the API first, you might want the front-end development to begin in parallel based on the design (even though the API hasn't been built yet). How can frontend development begin when the API doesn't exist? Enter mock servers.

Nightingale's mock server feature will take your API design and spawn a localhost HTTP server which you can reach using something like `http://localhost:1337`. When you ping this address, you will get back mock data that you defined in your collections. This lets frontend development (or development of anything that consumes the API) to continue even without the API being fully built!

## Get started

1. Add a new request and open it
1. Add a valid URL to the request, such as `http://localhost:1337/my/mock/api`
1. Navigate to the `Mock` section of the request
1. Fill in the information that you want the request to return. For example, enter a body like this:
    ```json
    {
      "key": "this a mock body"
    }
    ```
1. Once you're ready, click the `Deploy mock server` button
1. In the pop up, you'll see what workspace and environment will be used for the mock server. Click deploy
1. A console window will show up and it will initialize your mock data
1. Once it's ready, go to your web browser and navigate to `http://localhost:1337/my/mock/api`
1. You should see your mock body returned!

## Notes

- Nightingale currently uses `http://localhost:1337/` as the default mock server address. If customers want it, support for customizable port numbers can be added.
- The path to your mock data is based on a requests's URL path. Nightingale ignores the `http://domain.com` section of the URL and only uses the path `/this/is/a/path?query=1`.
- Queries are not yet handled by the mock server. I hope to add it some time soon.
