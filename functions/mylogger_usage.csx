#load "mylogger.csx"

public static async Task<IActionResult> Run(HttpRequest req, ILogger log)
{
    var mylogger = new MyLogger("fake"); 
    await mylogger.Write(0, "start", "");

    // ...
    // ...
    // ...
}
