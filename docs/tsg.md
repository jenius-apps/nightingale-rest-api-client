# Request Error - Troubleshooting Guide

## Trying to connnect to localhost?

1. Try turning off SSL validation in Nightingale's settings. Click the gear icon at the top right corner of Nightingale and turn off the SSL validation toggle.

2. If the above doesn't work, you might be hitting a known limitation for modern Windows apps. [try the workaround command in here](https://github.com/jenius-apps/nightingale-rest-api-client/blob/master/docs/localhost.md).

## Are you sure your URL is valid?

Double check
- that you're using `http://`, `https://`, or another valid scheme
- that your URL's spelling is correct

## Seeing a different error?

Click "stack trace" in Nightingale, then create a new issue here including details and the stack trace: https://github.com/jenius-apps/nightingale-rest-api-client/issues/new. 
