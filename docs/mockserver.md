# Mock servers

## What is it?
When you're building an API, sometimes you want to design the API first before writing any code. But when you design the API first, you might want the front-end development to begin in parallel based on the design (even though the API hasn't been built yet). How can frontend development begin when the API doesn't exist? Enter mock servers.

Nightingale's mock server feature will take your API design and spawn a localhost HTTP server which you can reach using something like `http://localhost:1337`. When you ping this address, you will get back mock data that you defined in your collections. This lets frontend development (or development of anything that consumes the API) to continue even without the API being fully built!

## Get started

TODO