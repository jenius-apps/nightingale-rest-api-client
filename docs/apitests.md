# API Testing using C# boolean expressions

Nightingale supports the ability to test the responses of your requests. A test in Nightingale is a C# boolean expression which you can add in the `TESTS` section of your request. 

`response.StatusCode == 200`

Above is an example of a valid test in Nightingale where you confirm that the status code of the response is 200. Note you do not need to end the expression with a semicolon (`;`). It just has to be a C# expression that resolves to a boolean.

`response` is the name of the object you will use in your tests. Its properties are as follows:

Property name | Type | Example
--- | --- | ---
`Successful` | `bool` | `response.Successful == true`
`StatusCode` | `int` | `response.StatusCode == 403`
`StatusDescription` | `string` | `response.StatusDescription.Contains("foobar")`
`TimeElapsed` | `long` | `response.TimeElapsed < 3000`
`RawBytes` | `byte[]` | `response.RawBytes != null`
`Body` | `string` | `response.Body.Contains("nightingale is great")`
`ContentType` | `string` | `response.Body == "application/json"`

